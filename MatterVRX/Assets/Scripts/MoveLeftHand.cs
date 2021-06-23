using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MoveLeftHand : MonoBehaviour // left hand for horizontal movement
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Vector2 trackPad;
    public GameObject voxels;

    // parameter to export in config file
    public float horizontalSpeed = 1;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (voxels != null)
        {
            Vector2 v = GetDirection();
            voxels.transform.position += new Vector3(v.x, 0, v.y) * Time.deltaTime * horizontalSpeed;

        }
    }

    public Vector2 GetDirection()
    {
        return trackPad.GetAxis(handType);
    }
}
