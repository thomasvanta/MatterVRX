using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LaserPointer : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean grabGrip;

    public GameObject laserPrefab;

    public Camera cam;

    private GameObject laser;
    private Transform laserTransform;
    private Vector3 hitPoint;

    private GameObject prevHit = null;

    // Start is called before the first frame update
    void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, 100))
        {
            if (prevHit != null && prevHit != hit.collider.gameObject)
            {
                OutlineController prevCtrl = prevHit.GetComponent<OutlineController>();
                if (prevCtrl != null)
                {
                    prevCtrl.EnableOutline(false);
                    prevCtrl.UpdateInfo(cam);
                }
            }

            hitPoint = hit.point;
            OutlineController ctrl = hit.collider.gameObject.GetComponent<OutlineController>();
            if (ctrl != null)
            {
                ctrl.EnableOutline(true);
                ctrl.UpdateInfo(cam);
                if (GetGrabGrip())
                {
                    ctrl.ToggleSelected();
                }
            }
            prevHit = hit.collider.gameObject;
            ShowLaser(hit);
        }
        else
        {
            laser.SetActive(false);
            if (prevHit != null)
            {
                OutlineController prevCtrl = prevHit.GetComponent<OutlineController>();
                if (prevCtrl != null)
                {
                    prevCtrl.EnableOutline(false);
                    prevCtrl.UpdateInfo(cam);
                }
                prevHit = null;
            }
        }
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(controllerPose.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    private bool GetGrabGrip()
    {
        return grabGrip.GetStateDown(handType);
    }
}
