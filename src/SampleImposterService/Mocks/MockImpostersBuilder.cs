using System.Collections.Generic;
using System.Linq;

using FluentImposter.Core.Entities;

using ImpostersServiceSample.Mocks.Products;

namespace ImpostersServiceSample.Mocks
{
    public class MockImpostersBuilder
    {
        private readonly IList<Imposter> _imposters = new List<Imposter>();

        public Imposter[] CreateMockImposters()
        {
            _imposters.Add(new ProductsImposter().Build());

            return _imposters.ToArray();
        }
    }
}
