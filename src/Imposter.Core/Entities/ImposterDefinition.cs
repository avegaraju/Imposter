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

        public ImposterDefinition(Imposter imposter)
        {
            _imposter = imposter;
        }
        public ImposterDefinition IsOfType(ImposterType type)
        {
            _imposter.SetType(type);

            return this;
        }
    }
}
