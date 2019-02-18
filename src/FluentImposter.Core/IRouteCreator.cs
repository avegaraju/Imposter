namespace FluentImposter.Core
{
    public interface IRouteCreator<in T>
    {
        void CreateRoutes(T builder);
    }
}
