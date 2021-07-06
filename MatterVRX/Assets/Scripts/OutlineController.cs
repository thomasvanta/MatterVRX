using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    private bool selected = false;

    private Outline outlineScript;

    // Start is called before the first frame update
    void Start()
    {
        outlineScript = GetComponent<Outline>();
        outlineScript.enabled = false;
    }

    public void EnableOutline(bool enabled)
    {
        outlineScript.enabled = enabled || selected;
    }

    public void ToggleSelected()
    {
        selected = !selected;
        if (selected)
        {
            EnableOutline(true);
            outlineScript.OutlineColor = Color.red; //new Color(255, 140, 0); // orange
            outlineScript.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        }
        else
        {
            outlineScript.OutlineColor = Color.white;
            outlineScript.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }

    public bool GetSelected()
    {
        return selected;
    }
}
