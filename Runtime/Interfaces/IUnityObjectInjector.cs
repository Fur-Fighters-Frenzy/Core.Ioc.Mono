using UnityEngine;
using Validosik.Core.Ioc;

namespace Validosik.Core.Ioc.Mono.Interfaces
{
    public interface IUnityObjectInjector
    {
        bool TryInject(Component component, ServiceContainerManager manager);
        int Inject(GameObject root, ServiceContainerManager manager);
        int Inject(MonoBehaviour[] targets, ServiceContainerManager manager);
    }
}
