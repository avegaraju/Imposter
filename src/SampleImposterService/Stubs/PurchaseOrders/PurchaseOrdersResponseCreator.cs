using System;
using System.Net;
using FluentImposter.Core;
using FluentImposter.Core.Builders;
using FluentImposter.Core.Entities;

namespace ImpostersServiceSample.Stubs.PurchaseOrders
{
    public class PurchaseOrdersResponseCreator: IResponseCreator
    {
        public Response CreateResponse()
        {
            var purchaseOrderCreatedResponse = new PurchaseOrderCreatedResponse()
            {
                Message = "Purchase order of type demand order created.",
                PurchaseOrderId = (uint)new Random().Next(int.MaxValue)
            };

            return new ResponseBuilder().WithContent(purchaseOrderCreatedResponse, new JsonContentSerializer())
                .WithStatusCode(HttpStatusCode.Created)
                .Build();
        }
    }
}
