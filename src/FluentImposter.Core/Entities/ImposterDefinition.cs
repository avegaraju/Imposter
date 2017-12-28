using FluentImposter.Core.Builders;

namespace FluentImposter.Core.Entities
{
    public enum ImposterType
    {
        REST,
        SOAP
    }
    public class ImposterDefinition
    {
        private readonly Imposter _imposter;

        public ImposterDefinition(string imposterName)
        {
            _imposter = new Imposter(imposterName);
        }
        public ImposterStub IsOfType(ImposterType type)
        {
            _imposter.SetType(type);

            return new ImposterStub(_imposter);
        }
    }
}
