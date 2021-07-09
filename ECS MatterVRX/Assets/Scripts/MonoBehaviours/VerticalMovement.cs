using UnityEngine;
using Valve.VR;

public class VerticalMovement : MonoBehaviour // the right hand is used for vertical movement
{
    [SerializeField] private SteamVR_Input_Sources handType;
    [SerializeField] private SteamVR_Action_Vector2 trackPad;

    // parameter to export in config file
    [SerializeField] private float verticalSpeed = 1;


    // Update is called once per frame
    void Update()
    {
        Vector2 v = GetDirection();

        InputManager.yAxis = v.y * verticalSpeed;
    }

    public Vector2 GetDirection()
    {
        return trackPad.GetAxis(handType);
    }
}
