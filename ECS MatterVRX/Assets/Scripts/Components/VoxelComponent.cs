using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public struct VoxelComponent : IComponentData
{
    public float3 basePosition;
    public float baseScale;
    public bool filtered;
    //public DynamicBuffer<BufferInt> annotationsIds;
    public int annotationId;
}

//[InternalBufferCapacity(3)]
//public struct BufferInt : IBufferElementData
//{
//    public int value;
//}
 
