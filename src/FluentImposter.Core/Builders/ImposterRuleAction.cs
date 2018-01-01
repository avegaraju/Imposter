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
        public ImposterRule Then(Response response)
        {
            _imposter.CreateRuleAction(response);

            return new ImposterRule(_imposter);
        }
    }
}
