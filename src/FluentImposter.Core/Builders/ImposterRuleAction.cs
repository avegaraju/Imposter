using FluentImposter.Core.Entities;

namespace FluentImposter.Core.Builders
{
    public class ImposterRuleAction
    {
        private readonly Imposter _imposter;

        public ImposterRuleAction(Imposter imposter)
        {
            _imposter = imposter;
        }

        public ImposterRule Then(IResponseCreator responseCreator)
        {
            _imposter.CreateRuleAction(responseCreator);

            return new ImposterRule(_imposter);
        }
    }
}
