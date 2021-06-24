using System.Net.Http;

using FluentImposter.Core;
using FluentImposter.Core.Entities;

namespace ImpostersServiceSample.Stubs.Customers
{
    public class CustomerImposter: IImposter
    {
        public RestImposter Build()
        {
            return new ImposterDefinition("CustomersStub")
                    .DeclareResource("/api/Customers", HttpMethod.Post)
                    .When(r => r.Content.Contains("Name:Jack"))
                    .Then(new FailedToCreateCustomerResponseCreator())
                    .Build();
        }
    }
}
