using UnityEditor;
using UnityEditor.Callbacks;
using Validosik.Core.Ioc.Mono.Editor.CodeGeneration;

namespace Validosik.Core.Ioc.Mono.Editor
{
    [InitializeOnLoad]
    internal static class ContainableMonoCodegenBootstrap
    {
        private static bool _generationScheduled;

        static ContainableMonoCodegenBootstrap()
        {
            ScheduleGeneration();
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            ScheduleGeneration();
        }

        [MenuItem("Tools/Containable/Mono/Generate Injectors")]
        private static void GenerateInjectorsMenu()
        {
            ComponentInjectorCodeGenerator.GenerateAll();
        }

        [MenuItem("Tools/Containable/Mono/Rebuild All Prefab Inject Roots")]
        private static void RebuildPrefabRootsMenu()
        {
            ContainableInjectRootEditorUtility.RebuildAllPrefabs();
        }

        private static void ScheduleGeneration()
        {
            if (_generationScheduled)
            {
                return;
            }

            _generationScheduled = true;
            EditorApplication.delayCall += GenerateOnDelay;
        }

        private static void GenerateOnDelay()
        {
            _generationScheduled = false;
            ComponentInjectorCodeGenerator.GenerateAll();
        }
    }
}
