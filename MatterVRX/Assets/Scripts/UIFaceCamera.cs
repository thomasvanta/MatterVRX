using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    public Camera cam = null;
    public Vector3 parentPosition;

    void Update()
    {
        if (cam != null)
        {
            this.transform.position = parentPosition + cam.transform.right * 0.5f;
            this.transform.LookAt(cam.transform);
            this.transform.forward *= -1;
        }
    }
}
