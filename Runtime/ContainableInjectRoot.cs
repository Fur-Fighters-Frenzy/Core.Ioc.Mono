using System;
using System.Collections.Generic;
using UnityEngine;
using Validosik.Core.Ioc;
using Validosik.Core.Ioc.Mono.Interfaces;

namespace Validosik.Core.Ioc.Mono
{
    [DefaultExecutionOrder(-31900)]
    [DisallowMultipleComponent]
    public sealed class ContainableInjectRoot : MonoBehaviour
    {
        private static readonly UnityObjectInjector _sharedInjector = new UnityObjectInjector();

        [SerializeField] private MonoBehaviour _providerOverride;
        [SerializeField] private MonoBehaviour[] _targets = Array.Empty<MonoBehaviour>();

        private readonly List<MonoBehaviour> _providersBuffer = new List<MonoBehaviour>(4);
        private bool _isInjected;

        private void Awake()
        {
            TryInject();
        }

        private void Start()
        {
            if (!_isInjected)
            {
                TryInject();
            }
        }

        public bool TryInject()
        {
            if (_isInjected)
            {
                return false;
            }

            var manager = ResolveManager();
            if (manager == null)
            {
                return false;
            }

            Inject(manager);
            return true;
        }

        public int Inject(ServiceContainerManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (_isInjected)
            {
                return 0;
            }

            _isInjected = true;
            return _sharedInjector.Inject(_targets, manager);
        }

        private ServiceContainerManager ResolveManager()
        {
            if (_providerOverride is IServiceContainerManagerProvider overrideProvider
                && overrideProvider.ServiceContainerManager != null)
            {
                return overrideProvider.ServiceContainerManager;
            }

            var current = transform;
            while (current != null)
            {
                current.GetComponents(_providersBuffer);
                for (var i = 0; i < _providersBuffer.Count; ++i)
                {
                    var candidate = _providersBuffer[i];
                    if (candidate is IServiceContainerManagerProvider provider
                        && provider.ServiceContainerManager != null)
                    {
                        _providersBuffer.Clear();
                        return provider.ServiceContainerManager;
                    }
                }

                _providersBuffer.Clear();
                current = current.parent;
            }

            if (ServiceContainerManagerSource.TryGet(out var manager))
            {
                return manager;
            }

            Debug.LogWarning(
                "[Containable.Mono] ServiceContainerManager provider was not found for '" + name + "'.",
                this);
            return null;
        }
    }
}
