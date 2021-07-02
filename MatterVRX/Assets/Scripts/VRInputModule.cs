using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class VRInputModule : BaseInputModule
{
    public Camera cam;
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean click;

    private GameObject curObject = null;
    private PointerEventData data = null;

    protected override void Awake()
    {
        base.Awake();

        data = new PointerEventData(eventSystem);
    }

    public override void Process()
    {
        data.Reset();
        data.position = new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 2);

        eventSystem.RaycastAll(data, m_RaycastResultCache);
        data.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        curObject = data.pointerCurrentRaycast.gameObject;

        m_RaycastResultCache.Clear();

        HandlePointerExitAndEnter(data, curObject);

        if (click.GetStateDown(handType)) ProcessPress(data);

        if (click.GetStateUp(handType)) ProcessRelease(data);
    }

    private void ProcessPress(PointerEventData d)
    {

    }

    private void ProcessRelease(PointerEventData d)
    {

    }
}
