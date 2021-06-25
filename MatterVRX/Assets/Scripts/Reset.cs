using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Reset : MonoBehaviour
{
    public SteamVR_Input_Sources leftHandType;
    public SteamVR_Input_Sources rightHandType;
    public SteamVR_Action_Boolean clickPad;

    /*
    // Start is called before the first frame update
    void Start() { }
    */

    // Update is called once per frame
    void Update()
    {
        if (GetLeftPad() || GetRightPad())
        {
            this.transform.position = Vector3.zero;
            this.transform.localScale = Vector3.one;
        }
    }

    public bool GetLeftPad()
    {
        return clickPad.GetStateDown(leftHandType);
    }

    public bool GetRightPad()
    {
        return clickPad.GetStateDown(rightHandType);
    }
}
