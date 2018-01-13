using System.IO;
using System.Text;
using System.Xml.Serialization;

using FluentAssertions;

using FluentImposter.Core.Builders;
using FluentImposter.Core.Tests.Unit.Dummies;
using FluentImposter.Core.Tests.Unit.Helpers;

using Newtonsoft.Json;

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
        public void WithContent_AccpetsAnObjectAndAJsonSerializer_SerializesTheObjectCorrectly()
        {
            var dummyUserResponseObject = new DummyUserResponseObject
                                          {
                                              EmailAddress = "test@test.com",
                                              FistName = "Bob",
                                              LastName = "Martin",
                                              MonthlyIncome = 10000
                                          };

            var response = new ResponseBuilder()
                    .WithContent(dummyUserResponseObject, new JsonContentSerializer() )
                    .WithStatusCode(200)
                    .Build();

            
            string serializedString = JsonConvert.SerializeObject(dummyUserResponseObject);

            response.Content
                    .Should().Be(serializedString);
        }

        [Fact]
        public void WithContent_AccpetsAnObjectAndAnAmlSerializer_SerializesTheObjectCorrectly()
        {
            var dummyUserResponseObject = new DummyUserResponseObject
                                          {
                                              EmailAddress = "test@test.com",
                                              FistName = "Bob",
                                              LastName = "Martin",
                                              MonthlyIncome = 10000
                                          };

            var response = new ResponseBuilder()
                    .WithContent(dummyUserResponseObject, new XmlContentSerializer())
                    .WithStatusCode(200)
                    .Build();

            var stringBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(stringBuilder))
            {
                var xmlSerializer = new XmlSerializer(dummyUserResponseObject.GetType());
                xmlSerializer.Serialize(stringWriter, dummyUserResponseObject);
            }

            response.Content
                    .Should().Be(stringBuilder.ToString());
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
