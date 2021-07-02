using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    public Camera cam = null;

    void LateUpdate()
    {
        if (cam != null) this.transform.LookAt(cam.transform);
    }
}
