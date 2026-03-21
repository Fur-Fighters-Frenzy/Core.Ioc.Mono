using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Validosik.Core.Ioc.Mono.Editor.CodeGeneration;

namespace Validosik.Core.Ioc.Mono.Editor
{
    internal static class ContainableInjectRootEditorUtility
    {
        public static void RebuildAllPrefabs()
        {
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            for (var i = 0; i < prefabGuids.Length; ++i)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
                if (string.IsNullOrEmpty(assetPath))
                {
                    continue;
                }

                UpdatePrefabAsset(assetPath);
            }
        }

        public static bool UpdatePrefabAsset(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath)
                || !assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)
                || !assetPath.StartsWith("Assets/", StringComparison.Ordinal))
            {
                return false;
            }

            var prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
            try
            {
                var targets = CollectInjectableTargets(prefabRoot);
                var injectRoot = prefabRoot.GetComponent<ContainableInjectRoot>();
                var changed = false;

                if (targets.Count > 0)
                {
                    if (injectRoot == null)
                    {
                        injectRoot = prefabRoot.AddComponent<ContainableInjectRoot>();
                        changed = true;
                    }

                    changed |= ApplyTargets(injectRoot, targets);
                }
                else if (injectRoot != null)
                {
                    changed |= ApplyTargets(injectRoot, Array.Empty<MonoBehaviour>());
                }

                if (!changed)
                {
                    return false;
                }

                PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
                return true;
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }
        }

        private static List<MonoBehaviour> CollectInjectableTargets(GameObject prefabRoot)
        {
            var result = new List<MonoBehaviour>();
            var components = prefabRoot.GetComponentsInChildren<MonoBehaviour>(true);
            for (var i = 0; i < components.Length; ++i)
            {
                var component = components[i];
                if (component == null)
                {
                    continue;
                }

                if (InjectableComponentScanner.IsInjectable(component.GetType()))
                {
                    result.Add(component);
                }
            }

            return result;
        }

        private static bool ApplyTargets(ContainableInjectRoot injectRoot, IReadOnlyList<MonoBehaviour> targets)
        {
            var serializedObject = new SerializedObject(injectRoot);
            var targetsProperty = serializedObject.FindProperty("_targets");
            if (targetsProperty == null)
            {
                return false;
            }

            var changed = targetsProperty.arraySize != targets.Count;
            if (!changed)
            {
                for (var i = 0; i < targets.Count; ++i)
                {
                    if (targetsProperty.GetArrayElementAtIndex(i).objectReferenceValue != targets[i])
                    {
                        changed = true;
                        break;
                    }
                }
            }

            if (!changed)
            {
                return false;
            }

            targetsProperty.arraySize = targets.Count;
            for (var i = 0; i < targets.Count; ++i)
            {
                targetsProperty.GetArrayElementAtIndex(i).objectReferenceValue = targets[i];
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(injectRoot);
            return true;
        }
    }
}
