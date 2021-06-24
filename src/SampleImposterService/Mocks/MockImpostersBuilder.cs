using System.Collections.Generic;
using System.Linq;

using FluentImposter.Core.Entities;

using ImpostersServiceSample.Mocks.Products;

namespace ImpostersServiceSample.Mocks
{
    public class MockImpostersBuilder
    {
        private readonly IList<RestImposter> _imposters = new List<RestImposter>();

        public RestImposter[] CreateMockImposters()
        {
            _imposters.Add(new ProductsImposter().Build());

            return _imposters.ToArray();
        }
    }
}
