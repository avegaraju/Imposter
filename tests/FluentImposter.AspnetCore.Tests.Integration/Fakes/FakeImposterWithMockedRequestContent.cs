using System.Net.Http;

using FluentImposter.Core.Entities;

namespace FluentImposter.AspnetCore.Tests.Integration.Fakes
{
    internal class FakeImposterWithMockedRequestContent
    {
        private readonly HttpMethod _httpMethod;

        public FakeImposterWithMockedRequestContent(HttpMethod httpMethod)
        {
            _httpMethod = httpMethod;
        }
        public Imposter Build()
        {
            return new ImposterDefinition("test")
                    .DeclareResource("test", _httpMethod)
                    .When(r => r.Content.Contains("dummy"))
                    .Then(new DummyResponseCreator())
                    .Build();
        }
    }
}
