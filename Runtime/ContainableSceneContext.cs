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
        [SerializeField] private bool _includeChildInstallers = true;
        [SerializeField] private MonoInstaller[] _monoInstallers = Array.Empty<MonoInstaller>();

        private ServiceContainerManager _serviceContainerManager;
        private bool _monoInstallersApplied;

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
            ApplyMonoInstallers();
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

        public void InstallBindings()
        {
            EnsureInitialized();
            ApplyMonoInstallers();
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

        private void ApplyMonoInstallers()
        {
            if (_monoInstallersApplied)
            {
                return;
            }

            var installers = GetMonoInstallers();
            for (var i = 0; i < installers.Length; i++)
            {
                var installer = installers[i];
                if (installer == null)
                {
                    continue;
                }

                installer.InstallBindings(_serviceContainerManager);
            }

            _monoInstallersApplied = true;
        }

        private MonoInstaller[] GetMonoInstallers()
        {
            if (_monoInstallers != null && _monoInstallers.Length > 0)
            {
                return _monoInstallers;
            }

            return _includeChildInstallers
                ? GetComponentsInChildren<MonoInstaller>(true)
                : GetComponents<MonoInstaller>();
        }
    }
}
