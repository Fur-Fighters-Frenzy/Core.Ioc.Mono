using System;
using System.Collections.Generic;
using UnityEditor;

namespace Validosik.Core.Ioc.Mono.Editor
{
    internal sealed class ContainablePrefabAssetPostprocessor : AssetPostprocessor
    {
        private static readonly HashSet<string> _pendingPrefabPaths = new HashSet<string>(StringComparer.Ordinal);
        private static bool _processingScheduled;
        private static bool _isProcessing;

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            Enqueue(importedAssets);
            Enqueue(movedAssets);

            if (_processingScheduled || _pendingPrefabPaths.Count == 0)
            {
                return;
            }

            _processingScheduled = true;
            EditorApplication.delayCall += ProcessPendingPrefabs;
        }

        private static void Enqueue(string[] assetPaths)
        {
            if (assetPaths == null)
            {
                return;
            }

            for (var i = 0; i < assetPaths.Length; ++i)
            {
                var assetPath = assetPaths[i];
                if (string.IsNullOrEmpty(assetPath)
                    || !assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)
                    || !assetPath.StartsWith("Assets/", StringComparison.Ordinal))
                {
                    continue;
                }

                _pendingPrefabPaths.Add(assetPath);
            }
        }

        private static void ProcessPendingPrefabs()
        {
            if (_isProcessing)
            {
                return;
            }

            _processingScheduled = false;
            _isProcessing = true;
            try
            {
                while (_pendingPrefabPaths.Count > 0)
                {
                    var batch = new List<string>(_pendingPrefabPaths);
                    _pendingPrefabPaths.Clear();

                    for (var i = 0; i < batch.Count; ++i)
                    {
                        ContainableInjectRootEditorUtility.UpdatePrefabAsset(batch[i]);
                    }
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }
}
