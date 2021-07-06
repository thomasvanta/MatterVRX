using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ToggleInfoEvent : MonoBehaviour
{
    public event Action onToggleInfo;

    public bool infoOn = true;
    
    public void ToggleInfo(bool on)
    {
        infoOn = on;
        if (onToggleInfo != null) onToggleInfo();
    }
}
