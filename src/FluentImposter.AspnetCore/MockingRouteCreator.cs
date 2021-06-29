using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
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
    public class MockingRouteCreator:  IRouteCreator<IApplicationBuilder>
    {
        private readonly RestImposter[] _imposters;
        private readonly IDataStore _dataStore;
        private readonly ImposterRulesEvaluator _rulesEvaluator;
        private readonly IImposterRoute _imposterRoute;

        public MockingRouteCreator(
            RestImposter[] imposters,
            ImposterRulesEvaluator rulesEvaluator,
            IImposterRoute imposterRoute,
            IDataStore dataStore
            )
        {
            _imposters = imposters;
            _rulesEvaluator = rulesEvaluator;
            _imposterRoute = imposterRoute;
            _dataStore = dataStore;
        }

        public void CreateRoutes(IApplicationBuilder applicationBuilder)
        {
            CreateRoutesForMocking(applicationBuilder);

            _imposterRoute.CreateImposterResourceRoutes(
                applicationBuilder,
                _imposters,
                EvaluateImposterRules
            );
        }

        private RequestDelegate EvaluateImposterRules(RestImposter imposter)
        {
            return async context =>
            {
                await EvaluateRules(imposter, context);
            };
        }

        private async Task EvaluateRules(RestImposter imposter,
            HttpContext context)
        {
            var request = BuildRequest(context);

            var (response, matchedCondition) = _rulesEvaluator.EvaluateRules(imposter,
                context,
                request);

            StoreRequestAndResponse(imposter, request, response, matchedCondition);

            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsync(response.Content);
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

        private void StoreRequestAndResponse(RestImposter imposter,
                                             Request request,
                                             Response response,
                                             Expression<Func<Request, bool>> matchedCondition)
        {
            var requestId = _dataStore.StoreRequest(imposter.Resource,
                                                                  imposter.Method,
                                                                  Encoding.ASCII.GetBytes(request.Content));

            _dataStore.StoreResponse(requestId,
                                                   imposter.Name,
                                                   matchedCondition?.ToString(),
                                                   Encoding.ASCII.GetBytes(response.Content));
        }

        private void CreateRoutesForMocking(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder
                    .UseRouter(routeBuilder =>
                               {
                                   routeBuilder.MapVerb("Get",
                                                        "mocks/verify",
                                                        VerifyMockingRequestHandler());
                               });
        }

        private RequestDelegate VerifyMockingRequestHandler()
        {
            return async context =>
                   {
                       try
                       {
                           var verificationResponses = GetVerificationResponse(context);

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

            VerificationResponse GetVerificationResponse(HttpContext context)
            {
                var verificationRequest = GetVerificationRequest(context);

                return _dataStore.GetVerificationResponse(verificationRequest.Resource,
                                                                        new HttpMethod(verificationRequest.HttpMethod),
                                                                        Encoding
                                                                                .ASCII
                                                                                .GetBytes(verificationRequest
                                                                                                  .RequestPayload));
            }

            VerificationRequest GetVerificationRequest(HttpContext context)
            {
                using (StreamReader streamReader = new StreamReader(context.Request.Body))
                {
                    var body = streamReader.ReadToEnd();

                    try
                    {
                        return JsonConvert
                                .DeserializeObject<VerificationRequest>(body);
                    }
                    catch (JsonReaderException e)
                    {
                        throw new InvalidVerificationRequestException("Verification request is not a valid JSON",
                                                                      e);
                    }
                }
            }
        }
    }
}
