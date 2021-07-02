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
        data.pointerPressRaycast = data.pointerCurrentRaycast;

        GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(curObject, d, ExecuteEvents.pointerDownHandler);

        if (newPointerPress == null) newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(curObject);

        d.pressPosition = d.position;
        d.pointerPress = newPointerPress;
        d.rawPointerPress = curObject;
    }

    private void ProcessRelease(PointerEventData d)
    {
        ExecuteEvents.Execute(d.pointerPress, d, ExecuteEvents.pointerUpHandler);

        GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(curObject);

        if (d.pointerPress == pointerUpHandler)
        {
            ExecuteEvents.Execute(d.pointerPress, d, ExecuteEvents.pointerClickHandler);
        }

        eventSystem.SetSelectedGameObject(null);

        d.pressPosition = Vector2.zero;
        d.pointerPress = null;
        d.rawPointerPress = null;
    }
}
