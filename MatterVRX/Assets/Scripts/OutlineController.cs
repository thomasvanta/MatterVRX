using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    private Outline outlineScript;

    // Start is called before the first frame update
    void Start()
    {
        outlineScript = GetComponent<Outline>();
        outlineScript.enabled = false;
    }

    // Update is called once per frame
    void Update() {}

    public void EnableOutline(bool enabled)
    {
        outlineScript.enabled = enabled;
    }
}
