# Core.Ioc.Mono

Optional Unity-facing layer for [Core.Ioc](https://github.com/Fur-Fighters-Frenzy/Core.Ioc).

This module adds:

- generated component injectors for `MonoBehaviour`
- `ContainableInjectRoot` for cached prefab subtree injection
- `ContainableSceneContext` as a lightweight runtime provider
- instantiation and `AddComponent` wrappers that trigger injection

## Current Scope

- component injection is generated at editor-time
- runtime uses no `MethodInfo.Invoke`, `FieldInfo.SetValue`, or assembly scanning
- only components from `Assets/` are supported for injector generation in this first iteration
- injector methods should use `[Inject]` and be `public`, `internal`, or `protected internal`

## Example

```csharp
using UnityEngine;
using Validosik.Core.Ioc.Attributes;

public sealed class PlayerHud : MonoBehaviour
{
    private IHealthService _healthService;
    private IInputService _inputService;

    [Inject]
    internal void Construct(IHealthService healthService, IInputService inputService)
    {
        _healthService = healthService;
        _inputService = inputService;
    }
}
```

The editor generates an injector class into the same assembly as the component and updates prefab roots with `ContainableInjectRoot`.

---

# Part of the Core Project

This package is part of the **Core** project, which consists of multiple Unity packages.
See the full project here: [Core](https://github.com/Fur-Fighters-Frenzy/Core)

---
