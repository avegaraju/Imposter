using System.Net.Http;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

namespace SampleClientService
{
    public class UserIdentityImposter: IImposter
    {
        public Imposter Build()
        {
            return new ImposterDefinition("test")
                    .StubsResource("users", HttpMethod.Post)
                    .When(r => r.Content.Contains("abc@xyz.com"))
                    .Then(new DummyResponseCreator())
                    .Build();
        }
    }

    internal class DummyResponseCreator : IResponseCreator
    {
        public Response CreateResponse()
        {
            return new ResponseBuilder()
                    .WithContent("user created")
                    .WithStatusCode(201)
                    .Build();
        }
    }
}
