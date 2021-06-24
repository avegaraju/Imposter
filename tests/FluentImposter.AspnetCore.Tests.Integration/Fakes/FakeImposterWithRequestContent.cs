using System.Net.Http;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

namespace FluentImposter.AspnetCore.Tests.Integration.Fakes
{
    internal class FakeImposterWithRequestContent
    {
        private readonly HttpMethod _httpMethod;

        public FakeImposterWithRequestContent(HttpMethod httpMethod)
        {
            _httpMethod = httpMethod;
        }
        public RestImposter Build()
        {
            return new ImposterDefinition("test")
                .ForRest()
                .DeclareResource("test", _httpMethod)
                .When(r => r.Content.Contains("dummy"))
                .Then(new DummyResponseCreator())
                .Build();
        }
    }
}
