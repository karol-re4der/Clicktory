using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Globals
{
    public static bool IsBuilding()
    {
        return GameObject.Find("Canvas").transform.Find("Bottom Bar/Scroll View").gameObject.activeSelf;
    }
    public static bool IsPointerInGame()
    {
        if (Application.isEditor)
        {
            if (EventSystem.current.IsPointerOverGameObject()) {
                return false;
            }
        }
        else
        {
            //foreach (Touch touch in Input.touches)
            //{
            //    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            //    {
            //        return false;
            //    }
            //}
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count == 0;
        }
        return true;
    }
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
