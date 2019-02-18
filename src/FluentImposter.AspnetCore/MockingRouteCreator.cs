using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using FluentImposter.Core;
using FluentImposter.Core.Entities;
using FluentImposter.Core.Exceptions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;

namespace FluentImposter.AspnetCore
{
    public class MockingRouteCreator: IRouteCreator<IApplicationBuilder>
    {
        private static Guid _currentSession = Guid.Empty;
        private readonly ImpostersAsMockConfiguration _configuration;
        private readonly ImposterRulesEvaluator _rulesEvaluator;

        public MockingRouteCreator(ImpostersAsMockConfiguration configuration,
                                   ImposterRulesEvaluator rulesEvaluator)
        {
            _configuration = configuration;
            _rulesEvaluator = rulesEvaluator;
        }

        public void CreateRoutes(IApplicationBuilder applicationBuilder)
        {
            CreateRoutesForMocking(applicationBuilder);
            CreateImposterResources(applicationBuilder);
        }

        private void CreateImposterResources(IApplicationBuilder applicationBuilder)
        {
            foreach (var imposter in _configuration.Imposters)
            {
                applicationBuilder
                        .UseRouter(routeBuilder =>
                                   {
                                       routeBuilder.MapVerb(imposter.Method.ToString(),
                                                            imposter.Resource,
                                                            EvaluateImposterRules(imposter));
                                   });
            }

            RequestDelegate EvaluateImposterRules(Imposter imposter)
            {
                return async context =>
                       {
                           await EvaluateRules(imposter, context);
                       };
            }

            async Task EvaluateRules(Imposter imposter,
                                     HttpContext context)
            {
                var request = BuildRequest(context);

                var (response, matchedCondition) = _rulesEvaluator.EvaluateRules(imposter, context);

                StoreRequestAndResponse(imposter, request, response, matchedCondition);

                context.Response.StatusCode = response.StatusCode;
                await context.Response.WriteAsync(response.Content);
            }
        }

        private static Request BuildRequest(HttpContext context)
        {
            var stream = context.Request.Body;
            using (var streamReader = new StreamReader(stream))
            {
                var request = new Request()
                              {
                                  Content = streamReader.ReadToEnd(),
                                  RequestHeader = BuildRequestHeader(),
                                  SessionId = _currentSession
                              };

                return request;
            }

            RequestHeader BuildRequestHeader()
            {
                var requestHeader = new RequestHeader();

                foreach (KeyValuePair<string, StringValues> keyValuePair in context.Request.Headers)
                {
                    requestHeader.Add(keyValuePair.Key, keyValuePair.Value);
                }

                return requestHeader;
            }
        }

        private void StoreRequestAndResponse(Imposter imposter,
                                             Request request,
                                             Response response,
                                             Expression<Func<Request, bool>> matchedCondition)
        {
            var requestId = _configuration.DataStore.StoreRequest(_currentSession,
                                                                  imposter.Resource,
                                                                  imposter.Method,
                                                                  Encoding.ASCII.GetBytes(request.Content));

            _configuration.DataStore.StoreResponse(requestId,
                                                   imposter.Name,
                                                   matchedCondition?.ToString(),
                                                   Encoding.ASCII.GetBytes(response.Content));
        }

        private void CreateRoutesForMocking(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder
                                .UseRouter(routeBuilder =>
                                {
                                    routeBuilder.MapVerb("Post",
                                             "mocks/session",
                                             CreateMockingSessionRequestHandler());

                                    routeBuilder.MapVerb("Get",
                                             "mocks/{sessionId}/verify",
                                             VerifyMockingRequestHandler());
                                });
        }

        private RequestDelegate CreateMockingSessionRequestHandler()
        {
            return async context =>
                   {
                       await CreateNewMockingSession(context);
                   };
        }

        private async Task CreateNewMockingSession(HttpContext context)
        {
            if (ActiveSessionExists)
                _configuration.DataStore.EndSession(_currentSession);

            _currentSession = _configuration.DataStore.CreateSession();

            context.Response.StatusCode =
                    (int)HttpStatusCode.Created;
            await context.Response.WriteAsync(_currentSession.ToString());
        }

        private RequestDelegate VerifyMockingRequestHandler()
        {
            return async context =>
            {
                if (ActiveSessionExists)
                    _configuration.DataStore.EndSession(_currentSession);

                try
                {
                    var verificationResponses = GetVerificationResponses(context);

                    context.Response.StatusCode = (int)HttpStatusCode.OK;

                    await context.Response
                                 .WriteAsync(JsonConvert.SerializeObject(verificationResponses));
                }
                catch (Exception ex)
                {
                    if (ex is InvalidSessionIdInUriException
                        || ex is InvalidVerificationRequestException)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                        await context.Response.WriteAsync(ex.Message);
                    }
                }
            };

            IEnumerable<VerificationResponse> GetVerificationResponses(HttpContext context)
            {
                var sessionId = GetSessionIdFromUri(context);
                var resource = GetResourceFromBody(context);

                return _configuration.DataStore.GetVerificationResponse(Guid.Parse(sessionId), resource);
            }

            string GetSessionIdFromUri(HttpContext context)
            {
                var path = context.Request.Path.Value;

                var startIndex = "/mocks/".Length;
                var endIndex = path.LastIndexOf("/verify", StringComparison.Ordinal) - "/verify".Length;

                var sessionId = path.Substring(startIndex, endIndex);

                if (!Guid.TryParse(sessionId, out _))
                    throw new InvalidSessionIdInUriException($"{sessionId} is not a valid session Id.");

                return sessionId;
            }

            string GetResourceFromBody(HttpContext context)
            {
                using (StreamReader streamReader = new StreamReader(context.Request.Body))
                {
                    var body = streamReader.ReadToEnd();

                    try
                    {
                        return JsonConvert
                                .DeserializeObject<VerificationRequest>(body)
                                .Resource;
                    }
                    catch (JsonReaderException e)
                    {
                        throw new InvalidVerificationRequestException("Verification request is not a valid JSON",
                                                                      e);
                    }
                }
            }
        }

        private static bool ActiveSessionExists=> _currentSession != Guid.Empty;
    }
}
