using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;
using Unity.Rendering;


[Serializable]
[MaterialProperty("_Color", MaterialPropertyFormat.Float4)]

public struct CustomColor : IComponentData
{
    public Color value;
}
