using UnityEngine;
using Valve.VR;
using Unity.Mathematics;

public class Zoom : MonoBehaviour
{
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private Camera userHead;
    [SerializeField] private SteamVR_Input_Sources leftHandType;
    [SerializeField] private SteamVR_Input_Sources rightHandType;
    [SerializeField] private SteamVR_Action_Single squeeze;
    [SerializeField] private Transform dummyTumor;

    private Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        bool zooming = GetLeftTrigger() && GetRightTrigger();

        if (zooming)
        {
            Vector3 newOffset = leftHand.transform.position - rightHand.transform.position;

            // if we start zooming, offset will be zero and if we don't have this test,
            // there will be a violent first zoom
            if (offset == Vector3.zero) offset = newOffset;

            if (offset.sqrMagnitude == 0) return;

            float scale = ((newOffset.sqrMagnitude / offset.sqrMagnitude) - 1) * InputManager.zoomSpeed + 1;

            InputManager.zoomPivot = userHead.transform.position + InputManager.zoomCenterOffset * userHead.transform.forward;
            InputManager.zoomFactor = scale;
            InputManager.zoomGlobal *= scale;

            float3 delta = InputManager.globalPosition - InputManager.zoomPivot;
            InputManager.globalPosition = InputManager.zoomPivot + scale * delta;

            delta = (float3)dummyTumor.position - InputManager.zoomPivot;
            dummyTumor.position = InputManager.zoomPivot + scale * delta;
            dummyTumor.localScale *= scale;

            offset = newOffset;
        }
        else
        {
            offset = Vector3.zero;
            InputManager.zoomPivot = float3.zero;
            InputManager.zoomFactor = 1;
        }
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
