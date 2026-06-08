using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Unity.Burst;

using System.Runtime.CompilerServices;
using UnityRandom = Unity.Mathematics.Random;

public class AtomRenderBaker : Baker<AtomRenderAuthoring>
{
    public override void Bake(AtomRenderAuthoring authoring)
    {
        var prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Renderable);

        var prototype = GetEntity(TransformUsageFlags.Dynamic);

        // The baked prefabEntity already has all rendering components from conversion
        AddComponent(prototype, new PointPrototypeTag());
        AddComponent(prototype, new EntityPrefabComponent { Prefab = prefabEntity });


        //AddComponent<LocalTransform>(prototype);
    }
}
public class BondRenderBaker : Baker<BondRenderAuthoring>
{
    public override void Bake(BondRenderAuthoring authoring)
    {
        var prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Renderable);

        var prototype = GetEntity(TransformUsageFlags.Dynamic);

        // The baked prefabEntity already has all rendering components from conversion
        AddComponent(prototype, new BondPrototypeTag());
        AddComponent(prototype, new EntityPrefabComponent { Prefab = prefabEntity });


        //AddComponent<LocalTransform>(prototype);
    }
}
// In use
public struct PointPrototypeTag : IComponentData { }
public struct BondPrototypeTag : IComponentData { }
public struct EntityPrefabComponent : IComponentData
{
    public Entity Prefab;
}
