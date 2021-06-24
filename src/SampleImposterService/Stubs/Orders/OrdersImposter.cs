using System.Net.Http;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

namespace ImpostersServiceSample.Stubs.Orders
{
    public class OrdersImposter: IImposter
    {
        public RestImposter Build()
        {
            return new ImposterDefinition("OrdersStub")
                    .DeclareResource("/api/Orders", HttpMethod.Post)
                    .When(r => r.Content.Contains("Product:1234"))
                    .Then(new OrdersResponseCreator())
                    .Build();
        }
    }
}
