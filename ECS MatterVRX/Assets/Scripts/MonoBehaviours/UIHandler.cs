using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private Text sliderText;
    public void PressReset()
    {
        InputManager.Reset();
    }

    public void SetFilteringMode(int mode)
    {
        InputManager.doFilter = true;
        InputManager.filterMode = (Filters)mode;
    }

    public void SetColorMap(int map)
    {
        InputManager.changedColormap = true;
        InputManager.colormap = (DataReader.ColorMap)map;
    }

    public void ChangeValueFilter(float val)
    {
        InputManager.valueFilter = val;
        sliderText.text = val.ToString();
        if (InputManager.filterMode == Filters.OnValue) InputManager.doFilter = true;
    }
}

public enum Filters
{
    None,
    Selected,
    Unselected,
    OnValue
}