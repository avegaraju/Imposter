using FluentAssertions;

using FluentImposter.Core.Builders;
using FluentImposter.Core.Entities;

using Xunit;

namespace FluentImposter.Core.Tests.Unit
{
    public class ResponseBuilderTests
    {
        [Fact]
        public void WithContent_SetsTheContentOnResponse()
        {
            var responseContent = "test content";

            var response = new ResponseBuilder()
                    .WithContent("test content")
                    .WithStatusCode(200)
                    .Build();

            response.Content.Should().Be(responseContent);
        }

        [Fact]
        public void WithContent_ReturnsResponseStatusCodeType()
        {
            var result = new ResponseBuilder()
                    .WithContent("test content");

            result.Should().BeOfType<ResponseStatusCode>();
        }

        [Fact]
        public void Can_SetResponseStatusCodeUsingResponseBuilder()
        {
            var statusCode = 200;

            var response = new ResponseBuilder()
                    .WithContent("test content")
                    .WithStatusCode(statusCode)
                    .Build();

            response.StatusCode.Should().Be(statusCode);
        }
    }
}
