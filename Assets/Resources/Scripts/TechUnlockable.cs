using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechUnlockable : MonoBehaviour
{
    public string tech;
    public int required;
    public void Update()
    {
        if (GetLevel() < required)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    private int GetLevel()
    {
        switch (tech)
        {
            case "Industry":
                return Globals.GetSave().industrialTech;
            case "Science":
                return Globals.GetSave().scientificTech;
            case "Logistics":
                return Globals.GetSave().logisticTech;
        }
        return 0;
    }
}
