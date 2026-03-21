using System;
using UnityEngine;
using Validosik.Core.Ioc;
using Validosik.Core.Ioc.Generated;
using Validosik.Core.Ioc.Mono;
using Validosik.Core.Ioc.Mono.Interfaces;

namespace Validosik.Core.Ioc.Mono.Samples.Manual
{
    [DisallowMultipleComponent]
    public sealed class SampleManualContainerContext : MonoBehaviour, IServiceContainerManagerProvider
    {
        [SerializeField] private string _containerKey = "sample-mono-manual";
        [SerializeField] private bool _publishAsDefaultSource = true;

        private ServiceContainerManager _serviceContainerManager;

        public ServiceContainerManager ServiceContainerManager => _serviceContainerManager;

        private void Awake()
        {
            if (_serviceContainerManager != null)
            {
                return;
            }

            _serviceContainerManager = new ServiceContainerManager();
            _serviceContainerManager.CreateContainer(
                _containerKey,
                new Binding[]
                {
                    new Binding(typeof(IClockService), typeof(SampleClockService), ServiceLifetime.Shared),
                    new Binding(typeof(IGreetingService), typeof(SampleGreetingService), ServiceLifetime.Shared)
                },
                null);
            _serviceContainerManager.SwitchContainer(_containerKey, null);

            if (_publishAsDefaultSource)
            {
                ServiceContainerManagerSource.Provider = () => _serviceContainerManager;
            }

            Debug.Log("[Containable.Mono.Sample] Manual container created: '" + _containerKey + "'.", this);
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
    }
}
