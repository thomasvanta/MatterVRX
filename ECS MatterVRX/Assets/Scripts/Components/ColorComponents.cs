using Unity.Entities;
using Unity.Mathematics;
using System;
using Unity.Rendering;


[Serializable]
[MaterialProperty("_Color", MaterialPropertyFormat.Float4)]
public struct MainColorComponent : IComponentData
{
    public float4 value;
}

[Serializable]
[MaterialProperty("_OutlineColor", MaterialPropertyFormat.Float4)]
public struct OutlineColorComponent : IComponentData
{
    public float4 value;
}