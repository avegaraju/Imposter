﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;

namespace FluentImposter.AspnetCore
{
    public static class ApplicationBuilderExtension
    {
        private static IDataStore _dataStore;

        public static void UseImposters(this IApplicationBuilder applicationBuilder,
                                        ImposterConfiguration imposterConfiguration)
        {
            _dataStore = imposterConfiguration.DataStore;

            MapHandlers(imposterConfiguration.Imposters, applicationBuilder);
        }

        private static void MapHandlers(Imposter[] imposters, IApplicationBuilder applicationBuilder)
        {
            MapMockRequestsAndMockHandlers(applicationBuilder);

            MapImposterResourceAndRequestHandler(imposters, applicationBuilder);
        }

        private static void MapMockRequestsAndMockHandlers(IApplicationBuilder applicationBuilder)
        {
            MapMockingSessionRequestsAndHandlers(applicationBuilder);
        }

        private static void MapMockingSessionRequestsAndHandlers(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseRouter(r =>
                                         {
                                             r.MapVerb("Post",
                                                       "mock/createsession",
                                                       async context =>
                                                       {
                                                           if (IsHttpPostRequest(context))
                                                           {
                                                               var sessionId = _dataStore.CreateSession();

                                                               context.Response.StatusCode =
                                                                       (int)HttpStatusCode.Created;
                                                               await context.Response.WriteAsync(sessionId.ToString());
                                                           }
                                                           else
                                                           {
                                                               context.Response.StatusCode =
                                                                       (int)HttpStatusCode.BadRequest;
                                                               await context
                                                                       .Response
                                                                       .WriteAsync("The resource can only accept POST requests.");
                                                           }
                                                       });

                                         });
            //applicationBuilder.Map("/mock/createsession",
            //                       app => HandleCreateMockingSessionRequest());

            void HandleCreateMockingSessionRequest()
            {
                applicationBuilder.Run(CreateMockingSession());
            }

            RequestDelegate CreateMockingSession()
            {
                return async context =>
                {
                    if (IsHttpPostRequest(context))
                    {
                        var sessionId = _dataStore.CreateSession();

                        context.Response.StatusCode = (int)HttpStatusCode.Created;
                        await context.Response.WriteAsync(sessionId.ToString());
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync("The resource can only accept POST requests.");
                    }
                };
            }
        }

        private static bool IsHttpPostRequest(HttpContext context)
        {
            return context
                    .Request
                    .Method
                    .Equals(HttpMethods.Post, StringComparison.OrdinalIgnoreCase);
        }

        private static void MapImposterResourceAndRequestHandler(Imposter[] imposters,
                                                                 IApplicationBuilder applicationBuilder)
        {
            foreach (var imposter in imposters)
            {
                applicationBuilder.Map(imposter.Resource,
                                       app => HandleRequest(app, imposter));
            }
        }

        private static void HandleRequest(IApplicationBuilder applicationBuilder, Imposter imposter)
        {
            applicationBuilder.Run(EvaluateImposterRules(imposter));
        }

        private static RequestDelegate EvaluateImposterRules(Imposter imposter)
        {
            return async context =>
                   {
                       await EvaluateRules(imposter, context);
                   };
        }

        private static async Task EvaluateRules(Imposter imposter,
                                                HttpContext context)
        {
            var request = BuildRequest(context);

            var response = RulesEvaluator.Evaluate(imposter, request);

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
                    RequestHeader = BuildRequestHeader()
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
