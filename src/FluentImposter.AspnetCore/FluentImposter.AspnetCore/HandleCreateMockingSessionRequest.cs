using System.Threading.Tasks;

using Microsoft.AspNetCore.Routing;

namespace FluentImposter.AspnetCore
{
    internal class HandleCreateMockingSessionRequest: IRouter
    {
        public Task RouteAsync(RouteContext context)
        {
            throw new System.NotImplementedException();
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
