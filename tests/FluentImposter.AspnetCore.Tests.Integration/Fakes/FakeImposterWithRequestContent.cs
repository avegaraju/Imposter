using System.Net.Http;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

namespace FluentImposter.AspnetCore.Tests.Integration.Fakes
{
    internal class FakeImposterWithRequestContent : IImposter
    {
        private readonly HttpMethod _httpMethod;

        public FakeImposterWithRequestContent(HttpMethod httpMethod)
        {
            _httpMethod = httpMethod;
        }
        public Imposter Build()
        {
            return new ImposterDefinition("test")
                    .StubsResource("test", _httpMethod)
                    .When(r => r.Content.Contains("dummy"))
                    .Then(new DummyResponseCreator())
                    .Build();
        }
    }
}
