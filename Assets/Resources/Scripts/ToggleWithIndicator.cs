using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ToggleWithIndicator : Toggle
{
    public Color activeColor = Color.green;

    void Start()
    {
        this.onValueChanged.AddListener(OnToggleValueChanged);
    }

    //Dear diary
    //Toggles Suck
    //Sincerely
    //Me
    private void OnToggleValueChanged(bool isOn)
    {
        ColorBlock cb = this.colors;
        if (isOn)
        {
            cb.normalColor = Color.green;
            cb.highlightedColor = Color.green;
            cb.selectedColor = Color.green;
        }
        else
        {
            cb = ColorBlock.defaultColorBlock;
        }
        this.colors = cb;
    }
    public void SetToggled(bool state)
    {
        if(isOn != state)
        {
            isOn = !isOn;
            OnToggleValueChanged(isOn);
        }
    }
}
