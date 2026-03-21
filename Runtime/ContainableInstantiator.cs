using System;
using UnityEngine;
using Validosik.Core.Ioc;
using Validosik.Core.Ioc.Mono.Interfaces;

namespace Validosik.Core.Ioc.Mono
{
    public sealed class ContainableInstantiator
    {
        private readonly ServiceContainerManager _manager;
        private readonly IUnityObjectInjector _injector;

        public ContainableInstantiator(ServiceContainerManager manager)
            : this(manager, new UnityObjectInjector())
        {
        }

        public ContainableInstantiator(ServiceContainerManager manager, IUnityObjectInjector injector)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _injector = injector ?? throw new ArgumentNullException(nameof(injector));
        }

        public static ContainableInstantiator FromDefaultSource(IUnityObjectInjector injector = null)
        {
            return new ContainableInstantiator(
                ServiceContainerManagerSource.GetOrThrow(),
                injector ?? new UnityObjectInjector());
        }

        public GameObject Instantiate(GameObject prefab)
        {
            return Instantiate(prefab, null, false);
        }

        public GameObject Instantiate(GameObject prefab, Transform parent, bool instantiateInWorldSpace = false)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }

            var clone = parent == null
                ? UnityEngine.Object.Instantiate(prefab)
                : UnityEngine.Object.Instantiate(prefab, parent, instantiateInWorldSpace);

            InjectHierarchy(clone);
            return clone;
        }

        public TComponent Instantiate<TComponent>(TComponent prefab) where TComponent : Component
        {
            return Instantiate(prefab, null, false);
        }

        public TComponent Instantiate<TComponent>(TComponent prefab, Transform parent, bool instantiateInWorldSpace = false)
            where TComponent : Component
        {
            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }

            var clone = parent == null
                ? UnityEngine.Object.Instantiate(prefab)
                : UnityEngine.Object.Instantiate(prefab, parent, instantiateInWorldSpace);

            InjectHierarchy(clone.gameObject);
            return clone;
        }

        public TComponent AddComponent<TComponent>(GameObject gameObject) where TComponent : Component
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException(nameof(gameObject));
            }

            var component = gameObject.AddComponent<TComponent>();
            _injector.TryInject(component, _manager);
            return component;
        }

        public Component AddComponent(GameObject gameObject, Type componentType)
        {
            if (gameObject == null)
            {
                throw new ArgumentNullException(nameof(gameObject));
            }

            if (componentType == null)
            {
                throw new ArgumentNullException(nameof(componentType));
            }

            var component = gameObject.AddComponent(componentType);
            _injector.TryInject(component, _manager);
            return component;
        }

        private void InjectHierarchy(GameObject root)
        {
            var injectRoot = root.GetComponent<ContainableInjectRoot>();
            if (injectRoot != null)
            {
                injectRoot.Inject(_manager);
                return;
            }

            _injector.Inject(root, _manager);
        }
    }
}
