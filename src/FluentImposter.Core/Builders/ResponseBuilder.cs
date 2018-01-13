using System.IO;
using System.Text;

using FluentImposter.Core.Entities;

namespace FluentImposter.Core.Builders
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

        public ResponseStatusCode WithContent(object content, IContentSerializer contentSerializer)
        {
            _response.Content = contentSerializer.Serialize(content);

            return new ResponseStatusCode(_response, this);
        }

        public Response Build()
        {
            return _response;
        }
    }
}
