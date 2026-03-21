using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Validosik.Core.Ioc.Attributes;

namespace Validosik.Core.Ioc.Mono.Editor.CodeGeneration
{
    internal static class InjectableComponentScanner
    {
        private const string GeneratedFolderName = "Containable.Mono";
        private static readonly Dictionary<Type, InjectableComponentDescriptor> _cache =
            new Dictionary<Type, InjectableComponentDescriptor>();
        private static bool _cacheBuilt;

        public static IReadOnlyList<InjectableComponentDescriptor> Scan(out List<string> warnings)
        {
            warnings = new List<string>();
            var result = new List<InjectableComponentDescriptor>();
            var scriptPaths = BuildScriptPathLookup();

            foreach (var componentType in TypeCache.GetTypesDerivedFrom<Component>())
            {
                if (!IsSupportedComponentType(componentType))
                {
                    continue;
                }

                var injectMethod = FindInjectMethod(componentType, out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    warnings.Add(error);
                    continue;
                }

                if (injectMethod == null)
                {
                    continue;
                }

                if (!scriptPaths.TryGetValue(componentType, out var scriptPath))
                {
                    warnings.Add("[Containable.Mono] Skipped '" + componentType.FullName + "' because its script path was not found.");
                    continue;
                }

                var normalizedScriptPath = NormalizeAssetPath(scriptPath);
                if (!normalizedScriptPath.StartsWith("Assets/", StringComparison.Ordinal))
                {
                    warnings.Add(
                        "[Containable.Mono] Skipped '" + componentType.FullName + "' because injector generation currently supports only Assets/ scripts.");
                    continue;
                }

                var outputDir = ResolveOutputDir(componentType.Assembly.GetName().Name);
                result.Add(new InjectableComponentDescriptor(componentType, injectMethod, normalizedScriptPath, outputDir));
            }

            BuildCache(result);
            return result;
        }

        public static bool IsInjectable(Type componentType)
        {
            if (componentType == null)
            {
                return false;
            }

            EnsureCacheBuilt();
            return _cache.ContainsKey(componentType);
        }

        private static void EnsureCacheBuilt()
        {
            if (_cacheBuilt)
            {
                return;
            }

            Scan(out _);
        }

        private static void BuildCache(IEnumerable<InjectableComponentDescriptor> descriptors)
        {
            _cache.Clear();
            foreach (var descriptor in descriptors)
            {
                _cache[descriptor.ComponentType] = descriptor;
            }

            _cacheBuilt = true;
        }

        private static bool IsSupportedComponentType(Type componentType)
        {
            if (componentType == null
                || componentType.IsAbstract
                || componentType.IsGenericTypeDefinition
                || componentType.ContainsGenericParameters)
            {
                return false;
            }

            var assemblyName = componentType.Assembly.GetName().Name ?? string.Empty;
            return !assemblyName.EndsWith(".Editor", StringComparison.Ordinal);
        }

        private static MethodInfo FindInjectMethod(Type componentType, out string error)
        {
            error = null;
            var matches = new List<MethodInfo>(2);

            for (var current = componentType; current != null && current != typeof(Component); current = current.BaseType)
            {
                var methods = current.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                                 BindingFlags.DeclaredOnly);
                for (var i = 0; i < methods.Length; ++i)
                {
                    var method = methods[i];
                    if (method.GetCustomAttribute<InjectAttribute>(true) != null)
                    {
                        matches.Add(method);
                    }
                }
            }

            if (matches.Count == 0)
            {
                return null;
            }

            if (matches.Count > 1)
            {
                error =
                    "[Containable.Mono] Skipped '" + componentType.FullName +
                    "' because more than one [Inject] method was found in its inheritance chain.";
                return null;
            }

            var injectMethod = matches[0];
            if (injectMethod.IsStatic)
            {
                error = "[Containable.Mono] Skipped '" + componentType.FullName + "' because [Inject] method must be instance.";
                return null;
            }

            if (injectMethod.IsGenericMethodDefinition || injectMethod.ContainsGenericParameters)
            {
                error =
                    "[Containable.Mono] Skipped '" + componentType.FullName + "' because generic [Inject] methods are not supported.";
                return null;
            }

            if (injectMethod.ReturnType != typeof(void))
            {
                error = "[Containable.Mono] Skipped '" + componentType.FullName + "' because [Inject] method must return void.";
                return null;
            }

            if (!(injectMethod.IsPublic || injectMethod.IsAssembly || injectMethod.IsFamilyOrAssembly))
            {
                error =
                    "[Containable.Mono] Skipped '" + componentType.FullName +
                    "' because [Inject] method must be public, internal, or protected internal.";
                return null;
            }

            var parameters = injectMethod.GetParameters();
            for (var i = 0; i < parameters.Length; ++i)
            {
                if (!IsSupportedParameterType(parameters[i].ParameterType))
                {
                    error =
                        "[Containable.Mono] Skipped '" + componentType.FullName +
                        "' because [Inject] parameter '" + parameters[i].ParameterType.FullName +
                        "' is not supported. Use reference services, Func<T>, or Lazy<T>.";
                    return null;
                }
            }

            return injectMethod;
        }

        private static bool IsSupportedParameterType(Type parameterType)
        {
            if (parameterType == null)
            {
                return false;
            }

            if (TryUnwrapSingleGeneric(parameterType, typeof(Func<>), out var funcType))
            {
                return IsSupportedServiceType(funcType);
            }

            if (TryUnwrapSingleGeneric(parameterType, typeof(Lazy<>), out var lazyType))
            {
                return IsSupportedServiceType(lazyType);
            }

            return IsSupportedServiceType(parameterType);
        }

        private static bool IsSupportedServiceType(Type serviceType)
        {
            return serviceType != null && !serviceType.IsValueType && serviceType != typeof(string);
        }

        private static bool TryUnwrapSingleGeneric(Type candidate, Type genericDefinition, out Type innerType)
        {
            if (candidate.IsGenericType && candidate.GetGenericTypeDefinition() == genericDefinition)
            {
                innerType = candidate.GetGenericArguments()[0];
                return true;
            }

            innerType = null;
            return false;
        }

        private static Dictionary<Type, string> BuildScriptPathLookup()
        {
            var byType = new Dictionary<Type, string>();
            var scriptGuids = AssetDatabase.FindAssets("t:MonoScript");
            for (var i = 0; i < scriptGuids.Length; ++i)
            {
                var path = AssetDatabase.GUIDToAssetPath(scriptGuids[i]);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                var scriptType = monoScript != null ? monoScript.GetClass() : null;
                if (scriptType == null)
                {
                    continue;
                }

                byType[scriptType] = NormalizeAssetPath(path);
            }

            return byType;
        }

        private static string ResolveOutputDir(string assemblyName)
        {
            var asmdefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assemblyName);
            if (!string.IsNullOrEmpty(asmdefPath))
            {
                asmdefPath = NormalizeAssetPath(asmdefPath);
                if (asmdefPath.StartsWith("Assets/", StringComparison.Ordinal))
                {
                    return NormalizeAssetPath(Path.Combine(Path.GetDirectoryName(asmdefPath) ?? "Assets", "Generated", GeneratedFolderName));
                }
            }

            return NormalizeAssetPath(Path.Combine("Assets", "Generated", GeneratedFolderName, SanitizeIdentifier(assemblyName ?? "Default")));
        }

        internal static string SanitizeIdentifier(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "_";
            }

            var chars = value.ToCharArray();
            for (var i = 0; i < chars.Length; ++i)
            {
                var isValid = i == 0
                    ? chars[i] == '_' || char.IsLetter(chars[i])
                    : chars[i] == '_' || char.IsLetterOrDigit(chars[i]);
                if (!isValid)
                {
                    chars[i] = '_';
                }
            }

            return new string(chars);
        }

        internal static string NormalizeAssetPath(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}
