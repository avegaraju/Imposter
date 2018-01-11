using FluentImposter.Core;
using FluentImposter.Core.Entities;

namespace FluentImposter.AspnetCore.Tests.Integration.Fakes
{
    internal class FakeImposterWithRequestHeader: IImposter
    {
        public Imposter Build()
        {
            return new ImposterDefinition("test")
                    .StubsResource("/test")
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
