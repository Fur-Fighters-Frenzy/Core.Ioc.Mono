using UnityEngine;
using Validosik.Core.Ioc.Mono;

namespace Validosik.Core.Ioc.Mono.Samples.Manual
{
    public sealed class SamplePrefabSpawnDemo : MonoBehaviour
    {
        [SerializeField] private SampleManualContainerContext _context;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private bool _spawnOnStart;

        private void Reset()
        {
            if (_context == null)
            {
                _context = GetComponent<SampleManualContainerContext>();
            }
        }

        private void Start()
        {
            if (_spawnOnStart)
            {
                SpawnPrefab();
            }
        }

        [ContextMenu("Spawn Prefab")]
        public void SpawnPrefab()
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

            if (_prefab == null)
            {
                Debug.LogWarning(
                    "[Containable.Mono.Sample] Assign a prefab with SamplePrefabInjectedComponent to test ContainableInjectRoot.",
                    this);
                return;
            }

            var instantiator = new ContainableInstantiator(_context.ServiceContainerManager);
            var clone = instantiator.Instantiate(_prefab, transform, false);
            clone.name = _prefab.name + " (Injected Clone)";

            Debug.Log(
                "[Containable.Mono.Sample] Prefab instantiated. Check SamplePrefabInjectedComponent logs for early injection confirmation.",
                clone);
        }
    }
}
