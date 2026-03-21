using System;
using Validosik.Core.Ioc;

namespace Validosik.Core.Ioc.Mono
{
    public static class ServiceContainerManagerSource
    {
        public static Func<ServiceContainerManager> Provider;

        public static bool TryGet(out ServiceContainerManager manager)
        {
            manager = Provider != null ? Provider() : null;
            return manager != null;
        }

        public static ServiceContainerManager GetOrThrow()
        {
            if (TryGet(out var manager))
            {
                return manager;
            }

            throw new InvalidOperationException(
                "ServiceContainerManagerSource.Provider is not assigned. " +
                "Provide a manager explicitly or add ContainableSceneContext.");
        }
    }
}
