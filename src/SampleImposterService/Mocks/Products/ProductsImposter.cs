using System.Net.Http;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

using ImpostersServiceSample.Stubs.Orders;

namespace ImpostersServiceSample.Mocks.Products
{
    public class ProductsImposter : IImposter
    {
        public Imposter Build()
        {
            return new ImposterDefinition("ProductsMock")
                    .DeclareResource("/api/Products", HttpMethod.Post)
                    .When(r => r.Content.Contains("Name:Test product"))
                    .Then(new ProductResponseCreator())
                    .Build();
        }
    }
}
