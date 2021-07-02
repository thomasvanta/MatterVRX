using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    private bool selected = false;

    private Outline outlineScript;

    public GameObject infoPrefab;
    private GameObject info = null;

    // Start is called before the first frame update
    void Start()
    {
        outlineScript = GetComponent<Outline>();
        outlineScript.enabled = false;
    }

    public void UpdateInfo(Camera camera)
    {
        if (info != null && !outlineScript.enabled)
        {
            Destroy(info);
            info = null;
        }
        else if (info == null && outlineScript.enabled)
        {
            info = Instantiate(infoPrefab);
            info.GetComponent<UIFillInfo>().Init(this.transform.position, new UIFillInfo.Voxel() { color = Color.red, scale = 0.2f });
            info.GetComponent<UIFaceCamera>().cam = camera;
            info.transform.position = this.transform.position + new Vector3(0, 0, 1);
        }
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
