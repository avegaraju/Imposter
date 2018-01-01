using System.IO;
using System.Threading.Tasks;

using FluentAssertions;

using FluentImposter.Core.Entities;

using Xunit;

namespace FluentImposter.Core.Tests.Unit
{
    public class RulesEvaluatorTests
    {
        [Fact]
        public async void Evaluate_WhenRequestStreamDoesNotMatchCondition_ReturnsAppropriateResponseContent()
        {
            using (var stream = await GetRequestContentStreamAsync("dummy request content"))
            {
                var imposter = new ImposterDefinition("test")
                        .IsOfType(ImposterType.REST)
                        .StubsResource("/test")
                        .When(r => r.Content.Contains("none of the imposter conditions will be able to "
                                                      + "match this text"))
                        .Then(new Response()
                              {
                                  Content = "if match found, this text will be returned."
                              })
                        .Build();

                var response = RulesEvaluator.Evaluate(imposter, stream);

                response.Content.Should().Be("None of the imposter conditions matched.");
            }
        }

        [Fact]
        public async void Evaluate_WhenRequestStreamDoesNotMatchCondition_ReturnsInternalServerError()
        {
            using (var stream = await GetRequestContentStreamAsync("dummy request content"))
            {
                var imposter = new ImposterDefinition("test")
                        .IsOfType(ImposterType.REST)
                        .StubsResource("/test")
                        .When(r => r.Content.Contains("none of the imposter conditions will be able to "
                                                      + "match this text"))
                        .Then(new Response()
                              {
                                  Content = "if match found, this text will be returned."
                              })
                        .Build();

                var response = RulesEvaluator.Evaluate(imposter, stream);

                response.StatusCode.Should().Be(500);
            }
        }

        [Fact]
        public async void Evaluate_WhenRequestStreamtMatchesACondition_ReturnsAppropriateResponseContent()
        {
            string requestContent = "This content will match one of the imposters";
            string responseContent = "dummy response";

            using (var stream = await GetRequestContentStreamAsync(requestContent))
            {
                var imposter = new ImposterDefinition("test")
                        .IsOfType(ImposterType.REST)
                        .StubsResource("/test")
                        .When(r => r.Content.Contains(requestContent))
                        .Then(new Response()
                              {
                                  Content = "dummy response"
                              })
                        .Build();

                var response = RulesEvaluator.Evaluate(imposter, stream);

                response.Content.Should().Be(responseContent);
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
    }
}
