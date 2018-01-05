using System.IO;
using System.Threading.Tasks;

using FluentAssertions;

using FluentImposter.Core.Entities;

using Xunit;

namespace FluentImposter.Core.Tests.Unit
{
    public class BodyEvaluatorTests
    {
        [Fact]
        public async void Evaluate_WhenRequestDoesNotMatchCondition_ReturnsInternalServerErrorWithAppropriateMessage()
        {
            string responseContent = "if match found, this text will be returned.";
            using (var stream = await GetRequestContentStreamAsync("dummy request content"))
            {
                var imposter = new ImposterDefinition("test")
                        .IsOfType(ImposterType.REST)
                        .StubsResource("/test")
                        .When(r => r.Content.Contains("none of the imposter conditions will be able to "
                                                      + "match this text"))
                        .Then(new DummyResponseCreator(responseContent))
                        .Build();

                var request = BuildRequest(stream);

                var evaluationResult = new BodyEvaluator().Evaluate(imposter, request);

                evaluationResult.Response.Content.Should().Be("None of the imposter conditions matched.");
                evaluationResult.Response.StatusCode.Should().Be(500);
            }
        }

        [Fact]
        public async void Evaluate_WhenRequestMatchesACondition_ReturnsAppropriateResponseContent()
        {
            string requestContent = "This content will match one of the imposters";
            string responseContent = "dummy response";

            using (var stream = await GetRequestContentStreamAsync(requestContent))
            {
                var imposter = new ImposterDefinition("test")
                        .IsOfType(ImposterType.REST)
                        .StubsResource("/test")
                        .When(r => r.Content.Contains(requestContent))
                        .Then(new DummyResponseCreator(responseContent))
                        .Build();

                var request = BuildRequest(stream);

                var evaluationResult = new BodyEvaluator().Evaluate(imposter, request);

                evaluationResult.Response.Content.Should().Be(responseContent);
                evaluationResult.Response.StatusCode.Should().Be(200);
            }
        }

        private static async Task<MemoryStream> GetRequestContentStreamAsync(string content)
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            await streamWriter.WriteAsync(content);
            streamWriter.Flush();
            stream.Position = 0;

            return stream;
        }

        private Request BuildRequest(MemoryStream stream)
        {
            stream.Position = 0;

            using (var streamReader = new StreamReader(stream))
            {
                return new Request()
                       {
                           Content = streamReader.ReadToEnd()
                       };
            }
        }
    }

    internal class DummyResponseCreator: IResponseCreator
    {
        private readonly string _content;

        public DummyResponseCreator(string content)
        {
            _content = content;
        }
        public Response CreateResponse()
        {
            return new Response()
                   {
                       Content = _content,
                       StatusCode = 200
                   };
        }
    }
}
