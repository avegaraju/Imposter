using FluentImposter.Core.Builders;

namespace FluentImposter.Core.Entities
{
    public class ResponseDefinition
    {
        private readonly Response _response;

        public ResponseDefinition()
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
