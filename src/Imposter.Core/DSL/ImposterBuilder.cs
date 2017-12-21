namespace Imposter.Core.DSL
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
            return _imposterHostBuilder;
        }
    }
}
