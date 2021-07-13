using Unity.Entities;
using Unity.Mathematics;
public struct OutlineComponent : IComponentData
{
    public bool isSelected;
    public float4 color;
}
