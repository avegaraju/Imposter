using System.Net.Http;

using FluentImposter.Core.Entities;

namespace FluentImposter.AspnetCore.Tests.Integration.Fakes
{
    internal class FakeImposterStubbingAResourceWithAPatternInUri
    {
        private readonly HttpMethod _httpMethod;

        public FakeImposterStubbingAResourceWithAPatternInUri(HttpMethod httpMethod)
        {
            _httpMethod = httpMethod;
        }

        public Imposter Build()
        {
            return new ImposterDefinition("test")
                    .DeclareResource("products/{productId}", _httpMethod)
                    .When(r => r.Content.Contains("dummy"))
                    .Then(new DummyResponseCreator())
                    .Build();
        }
    }
}
