using System;
using UnityEngine;
using Validosik.Core.Ioc.Attributes;
using Validosik.Core.Ioc.Mono.Interfaces;

namespace Validosik.Core.Ioc.Mono.Samples.Manual
{
    public sealed class SampleExistingComponent : MonoBehaviour, IAfterInject
    {
        private IGreetingService _greetingService;
        private Lazy<IClockService> _clockService;
        private bool _isInjected;

        [Inject]
        internal void Construct(IGreetingService greetingService, Lazy<IClockService> clockService)
        {
            _greetingService = greetingService;
            _clockService = clockService;
        }

        public void OnAfterInject()
        {
            _isInjected = true;
            Debug.Log("[Containable.Mono.Sample] Existing scene component received injection.", this);
        }

        public void PrintState(string label)
        {
            if (!_isInjected)
            {
                Debug.LogWarning("[Containable.Mono.Sample] " + label + ": component was not injected.", this);
                return;
            }

            Debug.Log(
                "[Containable.Mono.Sample] " + label + ": " +
                _greetingService.BuildGreeting(name) +
                " (lazy clock: " + _clockService.Value.GetTimestamp() + ")",
                this);
        }
    }
}
