# Manual Mono Injection Sample

This sample shows three integration paths for `Validosik.Core.Ioc.Mono`:

- injecting an existing `MonoBehaviour` in the scene with `UnityObjectInjector`
- injecting a runtime-added component with `ContainableInstantiator.AddComponent`
- injecting a prefab hierarchy through `ContainableInjectRoot`

## Setup

1. Import the sample from Package Manager.
2. Create an empty GameObject in a scene, for example `Mono Demo Root`.
3. Add `SampleManualContainerContext`, `SampleMonoInjectionDemo`, and optionally `SamplePrefabSpawnDemo` to that object.
4. Create a child GameObject and add `SampleExistingComponent` to it.
5. Assign the child reference into `SampleMonoInjectionDemo`.
6. Enter Play Mode and inspect the Console.

## What to Expect

- `SampleManualContainerContext` builds a container in code, so the sample does not depend on the IoC editor window.
- `SampleMonoInjectionDemo` injects the existing scene component and also demonstrates `AddComponent` with and without the wrapper.
- `SamplePrefabSpawnDemo` lets you assign a prefab and verify early injection via `ContainableInjectRoot`.

## Prefab Flow Check

1. Create a prefab under `Assets/`.
2. Put `SamplePrefabInjectedComponent` on the prefab root or on a child object.
3. Save the prefab.
4. The editor postprocessor should add `ContainableInjectRoot` to the prefab root and cache injectable targets.
5. Assign that prefab into `SamplePrefabSpawnDemo` and enter Play Mode.

`SamplePrefabInjectedComponent` logs from `Start`, so if injection happened early enough you will see the success message there.
