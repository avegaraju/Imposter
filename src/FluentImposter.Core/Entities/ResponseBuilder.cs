using FluentImposter.Core.Builders;

namespace FluentImposter.Core.Entities
{
    public class ResponseBuilder
    {
        private readonly Response _response;

        public ResponseBuilder()
        {
            _response = new Response();
        }
        public ResponseStatusCode WithContent(string content)
        {
            _response.Content = content;

            return new ResponseStatusCode(_response, this);
        }

        public Response Build()
        {
            return _response;
        }
    }
}
