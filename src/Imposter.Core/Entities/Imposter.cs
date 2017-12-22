namespace FluentImposter.Core.Entities
{
    public class Imposter
    {
        public string Name { get; }
        public ImposterType Type { get; private set; }

        public Imposter(string name)
        {
            Name = name;
        }

        internal void SetType(ImposterType type)
        {
            Type = type;
        }
    }
}
