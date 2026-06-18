using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using AtomData;
using UnityEngine;
[BurstCompile]
public struct AtomPlotterJob : IJobParallelFor
{
    public Entity atomEntity;
    public EntityCommandBuffer.ParallelWriter ECB;
    [DeallocateOnJobCompletion]public NativeArray<float3> location;
    [DeallocateOnJobCompletion]public NativeArray<Element> element;
    float4 GammaToLinear(float4 gammaColor)
    {
        // Burst-compatible Gamma to Linear approximation
        return new float4(math.pow(gammaColor.xyz, 2.2f), gammaColor.w);
    }
    public void Execute(int index)
    {
        var e = ECB.Instantiate(index, atomEntity);
        ECB.SetComponent(index, e, new LocalTransform{ Position = location[index], Rotation = quaternion.identity, Scale = (0.055f * AtomicRadiiAndColors.Radius(element[index]))});
        ECB.SetComponent(index, e, new URPMaterialPropertyBaseColor{ Value = GammaToLinear(AtomicRadiiAndColors.ElementColor(element[index]))});
    
    }
    
}
[BurstCompile]
public struct BondPlotterJob : IJobParallelFor
{
    public Entity bondEntity;
    public EntityCommandBuffer.ParallelWriter ECB;
    [DeallocateOnJobCompletion]public NativeArray<float4x4> transformdata;
   [DeallocateOnJobCompletion] public NativeArray<float4> element;
    
    public void Execute(int index)
    {
        var e = ECB.Instantiate(index, bondEntity);
        ECB.RemoveComponent<BondPrototypeTag>(index, e);
        ECB.SetComponent(index, e, LocalTransform.FromMatrix(transformdata[index]));
        ECB.SetComponent(index, e, new URPMaterialPropertyBaseColor{ Value = element[index]});
    
    }
    
}
public struct BondTransformElement : IBufferElementData
{
    public float4x4 Value;
}