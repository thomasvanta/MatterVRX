using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public struct VoxelComponent : IComponentData
{
    public float3 basePosition;
    public float baseScale;
    public float value;
    public bool filtered;
    public int4 annotationsIds;
}
 
