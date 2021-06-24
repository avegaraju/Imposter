using System.Net.Http;
using FluentImposter.Core;
using FluentImposter.Core.Entities;
using ImpostersServiceSample.Mocks.Products;

namespace ImpostersServiceSample.Stubs.PurchaseOrders
{
    public class PurchaseOrdersWithQueryStringImposter: IImposter
    {
        public RestImposter Build()
        {
            //For url pattern : /api/PurchaseOrders/?isDemandOrder=true
            return new ImposterDefinition("PurchaseOrderWithQueryStringImposter")
                .DeclareResource("/api/{PurchaseOrders}", HttpMethod.Post)
                .When(request => request.Content.Contains(""))
                .Then(new PurchaseOrdersResponseCreator())
                .Build();
        }
    }
}
