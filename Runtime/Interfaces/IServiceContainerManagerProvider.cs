using Validosik.Core.Ioc;

namespace Validosik.Core.Ioc.Mono.Interfaces
{
    public interface IServiceContainerManagerProvider
    {
        ServiceContainerManager ServiceContainerManager { get; }
    }
}
