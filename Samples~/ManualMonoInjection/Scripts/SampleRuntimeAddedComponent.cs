using System;
using UnityEngine;
using Validosik.Core.Ioc.Attributes;
using Validosik.Core.Ioc.Mono.Interfaces;

namespace Validosik.Core.Ioc.Mono.Samples.Manual
{
    public sealed class SampleRuntimeAddedComponent : MonoBehaviour, IAfterInject
    {
        private IGreetingService _greetingService;
        private Func<IClockService> _clockFactory;
        private bool _isInjected;

        [Inject]
        internal void Construct(IGreetingService greetingService, Func<IClockService> clockFactory)
        {
            _greetingService = greetingService;
            _clockFactory = clockFactory;
        }

        public void OnAfterInject()
        {
            _isInjected = true;
            Debug.Log("[Containable.Mono.Sample] Runtime-added component received injection.", this);
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
                " (factory clock: " + _clockFactory().GetTimestamp() + ")",
                this);
        }
    }
}
