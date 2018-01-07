using FluentImposter.Core.Entities;

namespace FluentImposter.Core.Builders
{
    public class ResponseStatusCode
    {
        private readonly Response _response;
        private readonly ResponseBuilder _responseDefinition;

        public ResponseStatusCode(Response response,
                                  ResponseBuilder responseDefinition)
        {
            _response = response;
            _responseDefinition = responseDefinition;
        }

        public ResponseBuilder WithStatusCode(int statusCode)
        {
            _response.StatusCode = statusCode;

            return _responseDefinition;
        }
    }
}
