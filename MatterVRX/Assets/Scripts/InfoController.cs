using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoController : MonoBehaviour
{
    private Outline outlineScript;

    public GameObject infoPrefab;
    private GameObject info = null;

    private bool showInfo = true;

    private ToggleInfoEvent infoEvent;

    // Start is called before the first frame update
    void Start()
    {
        outlineScript = GetComponent<Outline>();
        outlineScript.enabled = false;

        infoEvent = this.transform.parent.GetComponent<ToggleInfoEvent>();
    }

    public void UpdateInfo(Camera camera)
    {
        if (info != null && !outlineScript.enabled)
        {
            Destroy(info);
            info = null;

            infoEvent.onToggleInfo -= ToggleInfo;
        }
        else if (info == null && outlineScript.enabled)
        {
            info = Instantiate(infoPrefab);
            info.transform.parent = this.transform;

            info.GetComponent<UIFillInfo>().Init(this.transform.position, new UIFillInfo.Voxel() { color = Color.red, scale = 0.2f });

            UIFaceCamera faceCam = info.GetComponent<UIFaceCamera>();
            faceCam.cam = camera;

            showInfo = infoEvent.infoOn;
            info.SetActive(showInfo);

            infoEvent.onToggleInfo += ToggleInfo;
        }
    }

    public void ToggleInfo()
    {
        bool on = infoEvent.infoOn;
        showInfo = on;
        if (info != null) info.SetActive(on);
    }
}
