using FluentImposter.Core.Entities;

namespace FluentImposter.Core.DSL
{
    public abstract class ResponseCreator
    {
        protected abstract Response CreateResponse();

        public Response Execute()
        {
            return CreateResponse();
        }
    }
}
