using UnityEngine;
using Validosik.Core.Ioc;

namespace Validosik.Core.Ioc.Mono
{
    public abstract class MonoInstaller : MonoBehaviour
    {
        public abstract void InstallBindings(ServiceContainerManager container);
    }
}
