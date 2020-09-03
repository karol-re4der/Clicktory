using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HiddenGodMode : MonoBehaviour, IPointerDownHandler
{
    

    public float decay = 0.4f;
    public float halfwayDecay = 10f;
    public float delayRequired = 5f;
    public float clicksRequired = 10;

    private bool halfway = false;
    private float lastClick;
    private int clicks = 0;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Time.time - lastClick >= decay)
        {
            clicks = 0;
        }
        if (halfway && clicks == 0 && Time.time - lastClick <= delayRequired)
        {
            halfway = false;
            clicks = 0;
        }
        if (Time.time - lastClick >= halfwayDecay)
        {
            halfway = false;
        }
        


        lastClick = Time.time;
        clicks++;
        Debug.Log(clicks);


        
        if (clicks >= clicksRequired)
        {
            if (halfway)
            {
                GameObject.Find("Canvas").transform.Find("Menu/Scroll View/Viewport/Content/Button_Godmode").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("Menu/Scroll View/Viewport/Content/Button_Godmode").GetComponent<ToggleWithIndicator>().isOn = PlayerPrefs.GetInt("Godmode", 0) == 0 ? false : true;
                Debug.Log("WIN");
            }
            else
            {
                Debug.Log("HALF");
                halfway = true;
                clicks = 0;
            }
        }

    }
}
