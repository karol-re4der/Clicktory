using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    public static Logic GetLogic()
    {
        return GameObject.Find("Map").GetComponent<Logic>();
    }
    public static InterfaceHandler GetInterface()
    {
        return GameObject.Find("Canvas").GetComponent<InterfaceHandler>();
    }
    public static CameraHandler GetCamera()
    {
        return Camera.main.GetComponent<CameraHandler>();
    }
    public static ResourceStore GetResources()
    {
        return GetLogic().save.GetResources();
    }
    public static SaveFile GetSave()
    {
        return GetLogic().save;
    }
    public static string ParseNumber(int number)
    {
        string parsed = "";
        if (number >= 1000000000)
        {
            parsed = (int)(number / 1000000000) + "M";
        }
        else if (number >= 1000000)
        {
            parsed = (int)(number / 1000000) + "G";
        }
        else if (number >= 1000)
        {
            parsed = (int)(number / 1000)+"k";
        }
        else
        {
            parsed = number+"";
        }

        return parsed;
    }
}
