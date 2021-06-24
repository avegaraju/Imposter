using System.Collections.Generic;
using System.Linq;

using FluentImposter.Core.Entities;

using ImpostersServiceSample.Stubs.Customers;
using ImpostersServiceSample.Stubs.Orders;
using ImpostersServiceSample.Stubs.PurchaseOrders;

namespace ImpostersServiceSample.Stubs
{
    public class StubImpostersBuilder
    {
        private readonly IList<RestImposter> _imposters = new List<RestImposter>();

        public RestImposter[] CreateStubImposters()
        {
            _imposters.Add(new OrdersImposter().Build());
            _imposters.Add(new CustomerImposter().Build());
            _imposters.Add(new PurchaseOrdersWithQueryStringImposter().Build());

            return _imposters.ToArray();
        }
    }
}
