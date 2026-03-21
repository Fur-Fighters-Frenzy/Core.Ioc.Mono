using System;
using UnityEngine;
using Validosik.Core.Ioc;

namespace Validosik.Core.Ioc.Mono.Generated
{
    public abstract class GeneratedComponentInjector<TComponent> : IGeneratedComponentInjector
        where TComponent : Component
    {
        public Type TargetType => typeof(TComponent);

        void IGeneratedComponentInjector.Inject(ServiceContainerManager manager, Component component)
        {
            InjectTyped(manager, (TComponent)component);
        }

        protected abstract void InjectTyped(ServiceContainerManager manager, TComponent component);
    }
}
