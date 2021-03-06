﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using FluentImposter.Core;
using FluentImposter.Core.Entities;
using FluentImposter.Core.Exceptions;
using FluentImposter.Core.Models;

using ServiceStack.Aws.DynamoDb;

namespace FluentImposter.DataStore.AwsDynamoDb
{
    public class AwsDynamoDbDataStore: IDataStore
    {
        private readonly PocoDynamo _dynamo;

        public AwsDynamoDbDataStore(IAmazonDynamoDB client)
        {
            _dynamo = new PocoDynamo(client);

            InitializeSchema();
        }

        public Guid StoreRequest(string resource, HttpMethod method, byte[] requestPayload)
        {
            var requestId = Guid.NewGuid();

            var requestExists
                    = GetStoredRequestIfExists(resource,
                                               method,
                                               requestPayload,
                                               out Requests storedRequest);

            if (requestExists)
            {
                _dynamo.UpdateItemNonDefaults(new Requests
                                              {
                                                  Id = storedRequest.Id,
                                                  InvocationCount = storedRequest.InvocationCount + 1
                                              });

                requestId = storedRequest.Id;
            }
            else
            {
                _dynamo.PutItem(new Requests()
                                {
                                    HttpMethod = method.ToString(),
                                    Id = requestId,
                                    Resource = resource,
                                    RequestPayloadBase64 = Convert.ToBase64String(requestPayload),
                                    InvocationCount = 1
                                });
            }

            return requestId;
        }

        public void StoreResponse(Guid requestId, string imposterName, string matchedCondition, byte[] responsePayload)
        {
            if (!RequestExists(requestId))
                throw new RequestDoesNotExistException($"Request with id {requestId} does not exist.");

            if (!ResponseForRequestExists(requestId))
            {
                var responseId = Guid.NewGuid();
                _dynamo.PutItem(new Responses()
                                {
                                    Id = responseId,
                                    ImposterName = imposterName,
                                    MatchedCondition = matchedCondition,
                                    RequestId = requestId,
                                    ResponsePayloadBase64 = Convert.ToBase64String(responsePayload)
                                });
            }
        }

        private bool ResponseForRequestExists(Guid requestId)
        {
            return _dynamo.GetAll<Responses>()
                          .ToList()
                          .FirstOrDefault(r => r.RequestId == requestId)
                   != null;
        }

        public VerificationResponse GetVerificationResponse(string resource, HttpMethod method, byte[] requestPayload)
        {
            var requestExists
                    = GetStoredRequestIfExists(resource,
                                               method,
                                               requestPayload,
                                               out Requests storedRequest);

            if (requestExists)
            {
                return new VerificationResponse
                       {
                           RequestPayload
                                   = GetRequestPayloadAsString(storedRequest
                                                                       .RequestPayloadBase64),
                           Resource = storedRequest.Resource,
                           InvocationCount = storedRequest.InvocationCount
                       };
            }

            throw new InvalidVerificationRequestException("Could not verify call. No prior request"
                                                      + $"with Resource {resource}, Http method {method} "
                                                      + $"and request payload {Encoding.UTF8.GetString(requestPayload)} found.");
        }

        private string GetRequestPayloadAsString(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);

            return Encoding.UTF8.GetString(bytes);
        }

        public void PurgeData<T>()
        {
            _dynamo.DeleteTable<T>();
            _dynamo.RegisterTable<T>();
            _dynamo.InitSchema();
        }

        private bool RequestExists(Guid requestId)
        {
            return _dynamo.GetItem<Requests>(requestId.ToString()) != null;
        }

        private void InitializeSchema()
        {
            _dynamo.RegisterTable<Requests>();
            _dynamo.RegisterTable<Responses>();

            _dynamo.InitSchema();
        }

        private bool GetStoredRequestIfExists(string resource,
                                              HttpMethod method,
                                              byte[] requestPayload,
                                              out Requests storedRequest)
        {
            var request = _dynamo
                    .GetAll<Requests>()
                    .ToList()
                    .FirstOrDefault(r => r.Resource.Equals(resource, StringComparison.OrdinalIgnoreCase)
                                         && r.HttpMethod.Equals(method.Method.ToString(),
                                                                StringComparison.OrdinalIgnoreCase)
                                         && r.RequestPayloadBase64
                                             .Equals(Convert.ToBase64String(requestPayload)));

            storedRequest = request;

            return request != null;
        }
    }
}


