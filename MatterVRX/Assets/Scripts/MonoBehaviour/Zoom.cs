using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Zoom : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;
    public Camera userHead;
    public SteamVR_Input_Sources leftHandType;
    public SteamVR_Input_Sources rightHandType;
    public SteamVR_Action_Single squeeze;

    // parameter to export in config file
    public float zoomFactor = 1.0f;
    public float zoomCenterOffset = 1.5f;

    private Vector3 offset;
    private bool zooming = false;

    /*
    // Start is called before the first frame update
    void Start() { }
    */

    // Update is called once per frame
    void Update()
    {
        zooming = GetLeftTrigger() && GetRightTrigger();

        if (zooming)
        {
            Vector3 newOffset = leftHand.transform.position - rightHand.transform.position;

            // if we start zooming, offset will be zero and if we don't have this test,
            // there will be a violent first zoom
            if (offset == Vector3.zero) offset = newOffset;

            if (offset.sqrMagnitude == 0) return;
            
            float scale = ((newOffset.sqrMagnitude / offset.sqrMagnitude) - 1) * zoomFactor + 1;

            ScaleAround(this.gameObject, userHead.transform.position + zoomCenterOffset * userHead.transform.forward, scale);

            offset = newOffset;
        }
        else offset = Vector3.zero;
    }


    public void ScaleAround(GameObject target, Vector3 pivot, float scale)
    {
        Vector3 A = target.transform.localPosition;
        Vector3 B = pivot;

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        // calc final position post-scale
        Vector3 FP = B + C * scale;

        // finally, actually perform the scale/translation
        target.transform.localScale *= scale;
        target.transform.localPosition = FP;
    }

    public bool GetLeftTrigger()
    {
        return squeeze.GetAxis(leftHandType) > 0.7f;
    }

    public bool GetRightTrigger()
    {
        return squeeze.GetAxis(rightHandType) > 0.7f;
    }
}
