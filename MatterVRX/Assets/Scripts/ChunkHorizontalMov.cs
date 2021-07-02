using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ChunkHorizontalMov : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Vector2 trackPad;
    public Camera playerCamera;

    // parameter to export in config file
    public float horizontalSpeed = 1;

    /*
    // Start is called before the first frame update
    void Start() { }
    */

    // Update is called once per frame
    void Update()
    {
        Vector2 v = GetDirection();

        Vector3 move = new Vector3(playerCamera.transform.forward.x * v.y + playerCamera.transform.right.x * v.x,
            0, playerCamera.transform.forward.z * v.y + playerCamera.transform.right.z * v.x);

        this.transform.position += move * Time.deltaTime * horizontalSpeed * (float)System.Math.Log(1 + transform.localScale.x);
    }

    public Vector2 GetDirection()
    {
        return trackPad.GetAxis(handType);
    }
}
