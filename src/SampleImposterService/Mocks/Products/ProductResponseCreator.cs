using System;
using System.Net;

using FluentImposter.Core;
using FluentImposter.Core.Builders;
using FluentImposter.Core.Entities;

namespace ImpostersServiceSample.Mocks.Products
{
    public class ProductResponseCreator: IResponseCreator
    {
        public Response CreateResponse()
        {
            var productCreatedResponse = new ProductCreatedResponse
                                       {
                                           Message = "Product created.",
                                           ProductId = (uint)new Random().Next(int.MaxValue)
                                       };

            return new ResponseBuilder().WithContent(productCreatedResponse, new JsonContentSerializer())
                                        .WithStatusCode(HttpStatusCode.Created)
                                        .Build();
        }
    }
}
