using System;

namespace FluentImposter.Core.DSL
{
    public interface IImposterBuilder
    {
        ImposterHostBuilder HasAnImposter(string name);
    }

    public class ImposterBuilder: IImposterBuilder
    {
        private readonly ImposterHostBuilder _imposterHostBuilder;

        public ImposterBuilder(ImposterHostBuilder imposterHostBuilder)
        {
            _imposterHostBuilder = imposterHostBuilder;
        }
        public ImposterHostBuilder HasAnImposter(string name)
        {
            _imposterHostBuilder
                    .AddImposter(string.IsNullOrEmpty(name)
                                     ? new Entities.Imposter(Guid.NewGuid().ToString())
                                     : new Entities.Imposter(name));

            return _imposterHostBuilder;
        }
    }
}
