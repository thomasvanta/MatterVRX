using Unity.Entities;
using Unity.Mathematics;

public struct VoxelComponent : IComponentData
{
    public float3 basePosition;
    public float baseScale;
    public bool filtered;
}
 
