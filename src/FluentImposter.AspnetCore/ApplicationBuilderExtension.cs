using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace FluentImposter.AspnetCore
{
    public static class ApplicationBuilderExtension
    {
        private static IDataStore _dataStore;
        private static Guid _currentSession = Guid.Empty;

        public static void UseImposters(this IApplicationBuilder applicationBuilder,
                                        ImposterConfiguration imposterConfiguration)
        {
            _dataStore = imposterConfiguration.DataStore;

            CreateRoutes(imposterConfiguration.Imposters, applicationBuilder);
        }

        private static void CreateRoutes(Imposter[] imposters,
                                        IApplicationBuilder applicationBuilder)
        {
            CreateRoutesForMocking(applicationBuilder);

            CreateRoutesForImposterResources(imposters, applicationBuilder);
        }

        private static void CreateRoutesForMocking(IApplicationBuilder applicationBuilder)
        {
            CreateMockingSessionRoute(applicationBuilder);
        }

        private static void CreateMockingSessionRoute(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder
                    .UseRouter(routeBuilder =>
                               {
                                   routeBuilder.MapVerb("Post",
                                                        "mocks/session",
                                                        CreateMockingSessionRequestHandler());
                               });

            RequestDelegate CreateMockingSessionRequestHandler()
            {
                return async context =>
                {
                    if (DataStoreIsConfigured())
                    {
                        await CreateNewMockingSession(context);
                    }
                    else
                    {
                        await ReturnErrorResponse(context);
                    }
                };
            }

            async Task CreateNewMockingSession(HttpContext context)
            {
                if (ActiveSessionExists())
                    _dataStore.EndSession(_currentSession);

                _currentSession = _dataStore.CreateSession();

                context.Response.StatusCode =
                        (int)HttpStatusCode.Created;
                await context.Response.WriteAsync(_currentSession.ToString());
            }

            async Task ReturnErrorResponse(HttpContext context)
            {
                context.Response.StatusCode =
                        (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("No data store configured to enable mocking.");
            }
        }

        private static bool DataStoreIsConfigured()
        {
            return !(_dataStore is NullDataStore);
        }

        private static bool ActiveSessionExists()
        {
            return _currentSession != Guid.Empty;
        }

        private static void CreateRoutesForImposterResources(Imposter[] imposters,
                                                             IApplicationBuilder applicationBuilder)
        {
            foreach (var imposter in imposters)
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

                var response =
                        RulesEvaluator.Evaluate(imposter,
                                                request,
                                                out Expression<Func<Request, bool>> matchedCondition);

                StoreRequestAndResponse(imposter, request, response, matchedCondition);

                context.Response.StatusCode = response.StatusCode;
                await context.Response.WriteAsync(response.Content);
            }
        }

        private static void StoreRequestAndResponse(Imposter imposter,
                                                    Request request,
                                                    Response response,
                                                    Expression<Func<Request, bool>> matchedCondition)
        {
            if (!MockingEnabled(imposter))
                return;

            var requestId = _dataStore.StoreRequest(_currentSession,
                                                    imposter.Resource,
                                                    imposter.Method,
                                                    Encoding.ASCII.GetBytes(request.Content));

            _dataStore.StoreResponse(requestId,
                                     imposter.Name,
                                     matchedCondition?.ToString(),
                                     Encoding.ASCII.GetBytes(response.Content));
        }

        private static bool MockingEnabled(Imposter imposter)
        {
            return imposter.Behavior == ImposterBehavior.Mock;
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
    }
}
