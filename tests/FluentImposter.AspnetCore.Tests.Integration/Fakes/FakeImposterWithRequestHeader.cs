using System.Net.Http;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

namespace FluentImposter.AspnetCore.Tests.Integration.Fakes
{
    internal class FakeImposterWithRequestHeader: IImposter
    {
        public RestImposter Build()
        {
            return new ImposterDefinition("test")
                    .DeclareResource("test", HttpMethod.Post)
                    .When(r => r.RequestHeader.ContainsKeyAndValues("Accept",
                                                                    new[]
                                                                    {
                                                                        "text/plain",
                                                                        "text/xml"
                                                                    }))
                    .Then(new DummyResponseCreator())
                    .Build();
        }
    }
}
