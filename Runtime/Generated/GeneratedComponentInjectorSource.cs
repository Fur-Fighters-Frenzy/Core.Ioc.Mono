using System;
using System.Collections.Generic;

namespace Validosik.Core.Ioc.Mono.Generated
{
    public static class GeneratedComponentInjectorSource
    {
        private static readonly Dictionary<Type, IGeneratedComponentInjector> _injectors =
            new Dictionary<Type, IGeneratedComponentInjector>();

        public static void Register(IEnumerable<IGeneratedComponentInjector> injectors)
        {
            if (injectors == null)
            {
                return;
            }

            foreach (var injector in injectors)
            {
                if (injector == null || injector.TargetType == null)
                {
                    continue;
                }

                _injectors[injector.TargetType] = injector;
            }
        }

        public static bool TryGet(Type componentType, out IGeneratedComponentInjector injector)
        {
            if (componentType == null)
            {
                injector = null;
                return false;
            }

            return _injectors.TryGetValue(componentType, out injector);
        }
    }
}
