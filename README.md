# Core.Ioc.Mono

Optional Unity-facing layer for [Core.Ioc](https://github.com/Fur-Fighters-Frenzy/Core.Ioc).

This module adds:

- generated component injectors for `MonoBehaviour`
- `ContainableInjectRoot` for cached prefab subtree injection
- `ContainableSceneContext` as a lightweight runtime provider
- `MonoInstaller` support for manual runtime bindings
- instantiation and `AddComponent` wrappers that trigger injection

## Current Scope

- component injection is generated at editor-time
- runtime uses no `MethodInfo.Invoke`, `FieldInfo.SetValue`, or assembly scanning
- only components from `Assets/` are supported for injector generation in this first iteration
- injector methods should use `[Inject]` and be `public`, `internal`, or `protected internal`
- manual scene bindings can be registered through `MonoInstaller`

## Example

```csharp
using UnityEngine;
using Validosik.Core.Ioc;
using Validosik.Core.Ioc.Mono;

public sealed class HudInstaller : MonoInstaller
{
    [SerializeField] private PlayerHud _hud;

    public override void InstallBindings(ServiceContainerManager container)
    {
        container.RegisterInstance<IHud>(_hud);
        container.RegisterInstance(_hud);
    }
}
```

Attach the installer to the same GameObject as `ContainableSceneContext` or place it in a child object if `Include Child Installers` is enabled.

---

# Part of the Core Project

This package is part of the **Core** project, which consists of multiple Unity packages.
See the full project here: [Core](https://github.com/Fur-Fighters-Frenzy/Core)

---
