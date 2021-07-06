using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Selector : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean grabGrip;

    public float growthRate = 1;
    public float maxRadius = 1;

    private Vector3 initialHandPos;

    private new SphereCollider collider;
    private Mesh mesh;

    public void Start()
    {
        collider = this.GetComponent<SphereCollider>();

        collider.enabled = false;
    }

    public void Update()
    {
        if (Grabbing())
        {
            float radius = (controllerPose.transform.position - initialHandPos).sqrMagnitude;
            
            if (radius * growthRate < maxRadius)
            {
                collider.radius = Mathf.Min(radius * growthRate, maxRadius);
            }

            //this.transform.localScale = Vector3.one * radius;
        }
        else collider.enabled = false;
    }

    public bool Grabbing()
    {
        return grabGrip.GetState(handType);
    }

    public void StartGrabbing(Vector3 center)
    {
        initialHandPos = controllerPose.transform.position;
        collider.enabled = true;
        this.transform.position = center;
        collider.radius = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("trigg in");
        other.gameObject.GetComponent<OutlineController>().Select(true);
    }

    private void OnTriggerExit(Collider other)
    {
        print("trigg out");
        other.gameObject.GetComponent<OutlineController>().Select(false);
    }
}
