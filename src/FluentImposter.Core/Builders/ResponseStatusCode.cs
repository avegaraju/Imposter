using FluentImposter.Core.Entities;

namespace FluentImposter.Core.Builders
{
    public class ResponseStatusCode
    {
        private readonly Response _response;
        private readonly ResponseDefinition _responseDefinition;

        public ResponseStatusCode(Response response,
                                  ResponseDefinition responseDefinition)
        {
            _response = response;
            _responseDefinition = responseDefinition;
        }

        public ResponseDefinition WithResponseStatusCode(int statusCode)
        {
            _response.StatusCode = statusCode;

            return _responseDefinition;
        }
    }
}
