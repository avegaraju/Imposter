using System;
using System.Net;

using FluentImposter.Core;
using FluentImposter.Core.Builders;
using FluentImposter.Core.Entities;

namespace ImpostersServiceSample.Stubs.Orders
{
    public class OrdersResponseCreator : IResponseCreator
    {
        public Response CreateResponse()
        {
            var orderCreatedResponse = new OrderCreatedResponse
                                       {
                                           Message = "Order created.",
                                           OrderId = (uint)new Random().Next(int.MaxValue)
                                       };

            return new ResponseBuilder().WithContent(orderCreatedResponse, new JsonContentSerializer())
                                        .WithStatusCode(HttpStatusCode.Created)
                                        .Build();
        }
    }
}
