using System;
using System.Reflection;

namespace Validosik.Core.Ioc.Mono.Editor.CodeGeneration
{
    internal sealed class InjectableComponentDescriptor
    {
        public InjectableComponentDescriptor(Type componentType, MethodInfo injectMethod, string scriptPath, string outputDir)
        {
            ComponentType = componentType;
            InjectMethod = injectMethod;
            ScriptPath = scriptPath;
            OutputDir = outputDir;
        }

        public Type ComponentType { get; }
        public MethodInfo InjectMethod { get; }
        public string ScriptPath { get; }
        public string OutputDir { get; }
    }
}
