using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MoveRightHand : MonoBehaviour // the right hand is used for vertical movement
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Vector2 trackPad;
    public GameObject voxels;

    // parameter to export in config file
    public float verticalSpeed = 1;


    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        Vector2 v = GetTrackPad();
        if (v.y != 0)
        {
            voxels.transform.position += new Vector3(0, v.y, 0) * Time.deltaTime * verticalSpeed;
        }
    }

    public Vector2 GetTrackPad()
    {
        return trackPad.GetAxis(handType);
    }
}
