using FluentImposter.Core.Entities;

using Microsoft.Extensions.Primitives;

namespace FluentImposter.Core.Tests.Unit
{
    internal class RequestBuilder
    {
        private readonly Request _request = new Request();

        public Request Build()
        {
            return _request;
        }

        public RequestBuilder WithRequestHeader(string key, StringValues values)
        {
            var requestHeader = new RequestHeader
                                {
                                    {
                                        key, values
                                    }
                                };


            _request.RequestHeader = requestHeader;

            return this;
        }

        public RequestBuilder WithRequestContent(string content)
        {
            _request.Content = content;

            return this;
        }
    }
}
