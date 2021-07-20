using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public void PressReset()
    {
        InputManager.Reset();
    }

    public void SetFilteringMode(int mode)
    {
        InputManager.doFilter = true;
        InputManager.filterMode = (Filters)mode;
    }
}

public enum Filters
{
    None,
    Selected,
    Unselected
}