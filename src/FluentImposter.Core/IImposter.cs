using FluentImposter.Core.Entities;

namespace FluentImposter.Core
{
    public interface IImposter
    {
        Imposter Build(ImposterDefinition imposterDefinition);
    }

    //public class TestImposter: IImposter
    //{
    //    public Response CreateResponse()
    //    {
    //        throw new System.NotImplementedException();
    //    }

    //    public Imposter Build(ImposterDefinition imposterDefinition)
    //    {
    //        imposterDefinition.IsOfType(ImposterType.REST)
    //                          .When(r => r.Body.Content.Contains(""))
    //                          .Then(a => new TestResponseCreator()
    //                                        .CreateResponse())
    //                                        .When;
    //    }
    //}

    //public class TestResponseCreator: IResponseCreator
    //{
    //    public Response CreateResponse()
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
}
