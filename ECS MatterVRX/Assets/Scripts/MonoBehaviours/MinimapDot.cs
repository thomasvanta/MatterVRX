using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class MinimapDot : MonoBehaviour
{
    private float3 minimapScale;

    public void SetMapScale(float3 scale)
    {
        minimapScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 dv = InputManager.direction * minimapScale;
        //transform.localPosition += dv;
    }
}