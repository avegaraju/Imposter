using System.Net.Http;
using FluentImposter.Core.Entities;

namespace ImpostersServiceSample.Mocks.Products
{
    public class ProductsImposter
    {
        public RestImposter Build()
        {
            return new ImposterDefinition("ProductsMock")
                .ForRest()
                .DeclareResource("/api/Products", HttpMethod.Post)
                .When(r => r.Content.Contains("Name:Test product"))
                .Then(new ProductResponseCreator())
                .Build();
        }
    }
}
