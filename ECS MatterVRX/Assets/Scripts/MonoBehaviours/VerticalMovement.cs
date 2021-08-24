using UnityEngine;
using Valve.VR;

public class VerticalMovement : MonoBehaviour // the right hand is used for vertical movement
{
    [SerializeField] private SteamVR_Input_Sources handType;
    [SerializeField] private SteamVR_Action_Vector2 trackPad;
    [SerializeField] private Transform dummyTumor;


    // Update is called once per frame
    void Update()
    {
        Vector2 v = GetDirection();

        InputManager.direction.y = v.y * InputManager.verticalSpeed;
        InputManager.globalPosition.y += InputManager.direction.y * Time.deltaTime;

        dummyTumor.position += new Vector3(0, InputManager.direction.y, 0) * Time.deltaTime;
    }

    public Vector2 GetDirection()
    {
        return trackPad.GetAxis(handType);
    }
}
