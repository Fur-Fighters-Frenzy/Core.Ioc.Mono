using System;
using UnityEngine;
using Validosik.Core.Ioc;

namespace Validosik.Core.Ioc.Mono.Generated
{
    public interface IGeneratedComponentInjector
    {
        Type TargetType { get; }
        void Inject(ServiceContainerManager manager, Component component);
    }
}
