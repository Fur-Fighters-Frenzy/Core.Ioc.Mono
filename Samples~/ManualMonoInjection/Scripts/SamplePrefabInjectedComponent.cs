using UnityEngine;
using Validosik.Core.Ioc.Attributes;
using Validosik.Core.Ioc.Mono.Interfaces;

namespace Validosik.Core.Ioc.Mono.Samples.Manual
{
    public sealed class SamplePrefabInjectedComponent : MonoBehaviour, IAfterInject
    {
        private IGreetingService _greetingService;
        private bool _isInjected;

        [Inject]
        internal void Construct(IGreetingService greetingService)
        {
            _greetingService = greetingService;
        }

        public void OnAfterInject()
        {
            _isInjected = true;
        }

        private void Start()
        {
            if (_isInjected)
            {
                Debug.Log(
                    "[Containable.Mono.Sample] Prefab component started after successful early injection: " +
                    _greetingService.BuildGreeting(name),
                    this);
                return;
            }

            Debug.LogWarning(
                "[Containable.Mono.Sample] Prefab component Start ran without injection. Check that ContainableInjectRoot was added to the prefab root.",
                this);
        }
    }
}
