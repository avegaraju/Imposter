using FluentImposter.Core.Entities;

namespace FluentImposter.Core.DSL
{
    public interface IBuilder
    {
        Imposter Build();
    }

    public class Builder: IBuilder
    {
        private readonly Imposter _imposter;
        public Builder(Imposter imposter)
        {
            _imposter = imposter;
        }
        public Imposter Build()
        {
            return _imposter;
        }
    }
}
