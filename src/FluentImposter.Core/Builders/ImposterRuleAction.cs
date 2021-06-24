using FluentImposter.Core.Entities;

namespace FluentImposter.Core.Builders
{
    public class ImposterRuleAction
    {
        private readonly RestImposter _imposter;

        public ImposterRuleAction(RestImposter imposter)
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
