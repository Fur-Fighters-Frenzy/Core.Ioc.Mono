using UnityEngine;
using Validosik.Core.Ioc.Mono;

namespace Validosik.Core.Ioc.Mono.Samples.Manual
{
    public sealed class SampleMonoInjectionDemo : MonoBehaviour
    {
        [SerializeField] private SampleManualContainerContext _context;
        [SerializeField] private SampleExistingComponent _existingComponent;
        [SerializeField] private bool _runOnStart = true;

        private void Reset()
        {
            if (_context == null)
            {
                _context = GetComponent<SampleManualContainerContext>();
            }

            if (_existingComponent == null)
            {
                _existingComponent = GetComponentInChildren<SampleExistingComponent>(true);
            }
        }

        private void Start()
        {
            if (_runOnStart)
            {
                RunDemo();
            }
        }

        [ContextMenu("Run Demo")]
        public void RunDemo()
        {
            if (_context == null)
            {
                _context = GetComponent<SampleManualContainerContext>() ?? GetComponentInParent<SampleManualContainerContext>();
            }

            if (_context == null)
            {
                Debug.LogError("[Containable.Mono.Sample] SampleManualContainerContext is required.", this);
                return;
            }

            var manager = _context.ServiceContainerManager;
            var injector = new UnityObjectInjector();

            if (_existingComponent != null)
            {
                injector.Inject(_existingComponent.gameObject, manager);
                _existingComponent.PrintState("Existing scene object via UnityObjectInjector");
            }
            else
            {
                Debug.LogWarning("[Containable.Mono.Sample] Existing component reference is not assigned.", this);
            }

            var instantiator = new ContainableInstantiator(manager);

            var wrappedObject = new GameObject("Wrapped AddComponent Object");
            wrappedObject.transform.SetParent(transform, false);
            var wrappedComponent = instantiator.AddComponent<SampleRuntimeAddedComponent>(wrappedObject);
            wrappedComponent.PrintState("Runtime object via ContainableInstantiator.AddComponent");

            var manualObject = new GameObject("Manual Inject Object");
            manualObject.transform.SetParent(transform, false);
            var manualComponent = manualObject.AddComponent<SampleRuntimeAddedComponent>();
            injector.Inject(manualObject, manager);
            manualComponent.PrintState("Runtime object via plain AddComponent + manual Inject");
        }
    }
}
