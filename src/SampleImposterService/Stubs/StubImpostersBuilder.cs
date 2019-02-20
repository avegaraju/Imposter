using System.Collections.Generic;
using System.Linq;

using FluentImposter.Core.Entities;

using ImpostersServiceSample.Stubs.Customers;
using ImpostersServiceSample.Stubs.Orders;

namespace ImpostersServiceSample.Stubs
{
    public class StubImpostersBuilder
    {
        private readonly IList<Imposter> _imposters = new List<Imposter>();

        public Imposter[] CreateStubImposters()
        {
            _imposters.Add(new OrdersImposter().Build());
            _imposters.Add(new CustomerImposter().Build());

            return _imposters.ToArray();
        }
    }
}
