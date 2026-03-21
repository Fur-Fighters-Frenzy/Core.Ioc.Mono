using System;
using System.Collections.Generic;
using UnityEngine;
using Validosik.Core.Ioc;
using Validosik.Core.Ioc.Mono.Generated;
using Validosik.Core.Ioc.Mono.Interfaces;

namespace Validosik.Core.Ioc.Mono
{
    public sealed class UnityObjectInjector : IUnityObjectInjector
    {
        private readonly List<Component> _componentsBuffer = new List<Component>(8);
        private readonly Stack<Transform> _transformStack = new Stack<Transform>(16);

        public bool TryInject(Component component, ServiceContainerManager manager)
        {
            if (component == null)
            {
                return false;
            }

            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (!GeneratedComponentInjectorSource.TryGet(component.GetType(), out var injector))
            {
                return false;
            }

            injector.Inject(manager, component);
            if (component is IAfterInject afterInject)
            {
                afterInject.OnAfterInject();
            }

            return true;
        }

        public int Inject(GameObject root, ServiceContainerManager manager)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            var rootInjectRoot = root.GetComponent<ContainableInjectRoot>();
            if (rootInjectRoot != null)
            {
                return rootInjectRoot.Inject(manager);
            }

            var injected = 0;
            _transformStack.Clear();
            _transformStack.Push(root.transform);

            while (_transformStack.Count > 0)
            {
                var current = _transformStack.Pop();
                if (current != root.transform)
                {
                    var nestedInjectRoot = current.GetComponent<ContainableInjectRoot>();
                    if (nestedInjectRoot != null)
                    {
                        injected += nestedInjectRoot.Inject(manager);
                        continue;
                    }
                }

                current.GetComponents(_componentsBuffer);
                for (var i = 0; i < _componentsBuffer.Count; ++i)
                {
                    var component = _componentsBuffer[i];
                    if (component == null)
                    {
                        continue;
                    }

                    if (TryInject(component, manager))
                    {
                        injected++;
                    }
                }

                _componentsBuffer.Clear();

                for (var i = current.childCount - 1; i >= 0; --i)
                {
                    _transformStack.Push(current.GetChild(i));
                }
            }

            return injected;
        }

        public int Inject(MonoBehaviour[] targets, ServiceContainerManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (targets == null || targets.Length == 0)
            {
                return 0;
            }

            var injected = 0;
            for (var i = 0; i < targets.Length; ++i)
            {
                if (targets[i] == null)
                {
                    continue;
                }

                if (TryInject(targets[i], manager))
                {
                    injected++;
                }
            }

            return injected;
        }
    }
}
