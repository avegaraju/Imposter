using FluentAssertions;

using FluentImposter.Core.Builders;
using FluentImposter.Core.Entities;

using Xunit;

namespace FluentImposter.Core.Tests.Unit
{
    public class ResponseDefinitionTests
    {
        [Fact]
        public void WithContent_SetsTheContentOnResponse()
        {
            var responseContent = "test content";

            var response = new ResponseDefinition()
                    .WithContent("test content")
                    .WithResponseStatusCode(200)
                    .Build();

            response.Content.Should().Be(responseContent);
        }

        [Fact]
        public void WithContent_ReturnsResponseStatusCodeType()
        {
            var result = new ResponseDefinition()
                    .WithContent("test content");

            result.Should().BeOfType<ResponseStatusCode>();
        }

        [Fact]
        public void Can_SetResponseStatusCodeUsingResponseDefinition()
        {
            var statusCode = 200;

            var response = new ResponseDefinition()
                    .WithContent("test content")
                    .WithResponseStatusCode(statusCode)
                    .Build();

            response.StatusCode.Should().Be(statusCode);
        }
    }
}
