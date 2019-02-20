using System.Net;

using FluentImposter.Core;
using FluentImposter.Core.Builders;
using FluentImposter.Core.Entities;

namespace ImpostersServiceSample.Stubs.Customers
{
    public class FailedToCreateCustomerResponseCreator : IResponseCreator
    {
        public Response CreateResponse()
        {
            var orderCreatedResponse = new CreateCustomerResponse()
                                       {
                                           Message = "Customer creation failed.",
                                       };

            return new ResponseBuilder().WithContent(orderCreatedResponse, new JsonContentSerializer())
                                        .WithStatusCode(HttpStatusCode.InternalServerError)
                                        .Build();
        }
    }
}
