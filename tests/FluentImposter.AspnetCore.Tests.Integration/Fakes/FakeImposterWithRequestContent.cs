using FluentImposter.Core;
using FluentImposter.Core.Entities;

namespace FluentImposter.AspnetCore.Tests.Integration.Fakes
{
    internal class FakeImposterWithRequestContent : IImposter
    {
        public Imposter Build()
        {
            return new ImposterDefinition("test")
                    .StubsResource("/test")
                    .When(r => r.Content.Contains("dummy"))
                    .Then(new DummyResponseCreator())
                    .Build();
        }
    }
}
