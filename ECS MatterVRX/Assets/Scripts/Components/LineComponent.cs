using Unity.Entities;
using Unity.Mathematics;

public struct LineComponent : IComponentData
{
    public float3 baseFrom;
    public float3 baseTo;
    public bool filtered;
}
