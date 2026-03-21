using System;
using UnityEngine;
using Validosik.Core.Ioc;
using Validosik.Core.Ioc.Mono.Interfaces;

namespace Validosik.Core.Ioc.Mono
{
    [DefaultExecutionOrder(-32000)]
    [DisallowMultipleComponent]
    public sealed class ContainableSceneContext : MonoBehaviour, IServiceContainerManagerProvider
    {
        [SerializeField] private string _containerKey = string.Empty;
        [SerializeField] private bool _publishAsDefaultSource = true;

        private ServiceContainerManager _serviceContainerManager;

        public ServiceContainerManager ServiceContainerManager
        {
            get
            {
                EnsureInitialized();
                return _serviceContainerManager;
            }
        }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void OnDestroy()
        {
            if (!_publishAsDefaultSource || ServiceContainerManagerSource.Provider == null)
            {
                return;
            }

            if (ServiceContainerManagerSource.TryGet(out var current)
                && ReferenceEquals(current, _serviceContainerManager))
            {
                ServiceContainerManagerSource.Provider = null;
            }
        }

        private void EnsureInitialized()
        {
            if (_serviceContainerManager != null)
            {
                return;
            }

            _serviceContainerManager = new ServiceContainerManager();
            if (!string.IsNullOrEmpty(_containerKey)
                && !string.Equals(_serviceContainerManager.CurrentKey, _containerKey, StringComparison.Ordinal))
            {
                _serviceContainerManager.SwitchContainer(_containerKey, null);
            }

            if (_publishAsDefaultSource)
            {
                ServiceContainerManagerSource.Provider = () => _serviceContainerManager;
            }
        }
    }
}
