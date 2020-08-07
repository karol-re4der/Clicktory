using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;

public class InterfaceHandler : MonoBehaviour
{
    public float progressPerClick = 0.1f;
    public int rotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Update res display
        Transform frame = GameObject.Find("Canvas/Top Bar/Resources/").transform;
        foreach (ResourceStore.Res res in Globals.GetResources().res)
        {
            Transform obj = frame.Find(res.type);
            if (obj != null)
            {
                obj.Find("Amount").GetComponent<TextMeshProUGUI>().text = ""+res.amount;
            }
            else
            {
                GameObject newRes = Instantiate(Resources.Load("UI/ResourceFrame") as GameObject, frame);
                newRes.name = res.type;
                int spriteNumber = 0;
                switch (res.type)
                {
                    case "Coal":
                        spriteNumber = 1;
                        break;
                    case "Iron":
                        spriteNumber = 2;
                        break;
                }
                newRes.transform.GetChild(0).GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Textures/Resources/Resource_Spritesheet/")[spriteNumber];
            }
        }
    }

    public bool IsDemolishing()
    {
        ToggleGroup group = GameObject.Find("Canvas/Bottom Bar/Scroll View/Viewport/Content").GetComponent<ToggleGroup>();
        if (group.AnyTogglesOn() && group.ActiveToggles().ElementAt(0).name.Equals("Demolish"))
        {
            return true;
        }
        return false;
    }

    public void Button_Expand(GameObject frame)
    {
        float anchor_collapsed = 255f / Camera.main.pixelRect.height;
        float anchor_expanded = 470f / Camera.main.pixelRect.height;

        if (frame.GetComponent<RectTransform>().anchorMax.y == anchor_expanded)
        {
            frame.GetComponent<RectTransform>().anchorMax = new Vector2(1, anchor_collapsed);
            frame.transform.Find("Scroll View/Viewport/Content").GetComponent<ToggleGroup>().SetAllTogglesOff();
            frame.transform.Find("Tapbar").gameObject.SetActive(true);
            frame.transform.Find("Scroll View").gameObject.SetActive(false);

            frame.transform.Find("Expand").gameObject.SetActive(false);
            frame.transform.Find("Collapse").gameObject.SetActive(true);
        }
        else
        {
            frame.GetComponent<RectTransform>().anchorMax = new Vector2(1, anchor_expanded);
            frame.transform.Find("Tapbar").gameObject.SetActive(false);
            frame.transform.Find("Scroll View").gameObject.SetActive(true);

            frame.transform.Find("Expand").gameObject.SetActive(true);
            frame.transform.Find("Collapse").gameObject.SetActive(false);

            //Button_Rotate(frame.transform.Find("Scroll View/Viewport/Content").gameObject);
        }
    }

    public void Button_Tapbar(GameObject bar)
    {
        if(PlayerPrefs.GetInt("Godmode", 0) == 1)
        {
            Globals.GetLogic().Tick();
        }
        else if (bar.GetComponent<RectTransform>().anchorMax.x >= 1)
        {
            bar.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1f);
            bar.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1f);
            Globals.GetLogic().Tick();
        }
        else
        {
            bar.GetComponent<RectTransform>().anchorMax = new Vector2(bar.GetComponent<RectTransform>().anchorMax.x + progressPerClick, 1f);
            bar.GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, bar.GetComponent<RectTransform>().anchorMax.x);
        }
    }

    public void Button_Rotate(GameObject frame)
    {
        Rotate(frame, 1);
    }
    public void Button_Rotate_Clockwise(GameObject frame)
    {
        Rotate(frame, -1);
    }
    private void Rotate(GameObject frame, int direction)
    {
        if (direction > 0)
        {
            rotation = (rotation + direction) % 4;
        }
        else
        {
            if (rotation > 0)
            {
                rotation = rotation + direction;
            }
            else
            {
                rotation = 4 + direction;
            }
        }
        foreach (Transform trans in frame.transform)
        {
            if (!trans.name.Equals("Rotate") && !trans.name.Equals("Demolish"))
            {
                string path = "Textures/Interface/Building Icons/" + trans.name + "_Icon_Spritesheet";
                Sprite[] sheet = Resources.LoadAll<Sprite>(path);
                Sprite txt = sheet.Length == 4 ? sheet[rotation] : null;
                if (!txt)
                {
                    txt = Resources.Load<Sprite>("Textures/Machines/Machine_" + trans.name);
                }
                if (!txt)
                {
                    txt = Resources.Load<Sprite>("Textures/Templates/template_tile");
                }
                trans.Find("Image").GetComponent<Image>().sprite = txt;
            }
        }
    }

    public void Button_Menu()
    {
        if (GameObject.Find("Canvas/Menu"))
        {
            transform.Find("Fade").GetComponent<Fade>().FadeOut(0);
            GameObject.Find("Canvas/Menu").SetActive(false);
        }
        else
        {
            transform.Find("Fade").GetComponent<Fade>().FadeIn(0.5f);
            transform.Find("Menu").gameObject.SetActive(true);

            if (!PlayerPrefs.HasKey("Audio"))
            {
                PlayerPrefs.SetInt("Audio", 1);
            }
            else if (!PlayerPrefs.HasKey("Autosave"))
            {
                PlayerPrefs.SetInt("Autosave", 1);
            }

            transform.Find("Menu/Scroll View/Viewport/Content/Button_Audio").GetComponent<ToggleWithIndicator>().isOn = PlayerPrefs.GetInt("Audio") == 0 ? false : true;
            transform.Find("Menu/Scroll View/Viewport/Content/Button_Autosave").GetComponent<ToggleWithIndicator>().isOn = PlayerPrefs.GetInt("Autosave") == 0 ? false : true;
            transform.Find("Menu/Scroll View/Viewport/Content/Button_Godmode").GetComponent<ToggleWithIndicator>().isOn = PlayerPrefs.GetInt("Godmode", 0) == 0 ? false : true;

            transform.Find("Menu/Scroll View/Viewport/Content/Button_Reset/Final Text").gameObject.SetActive(false);
            transform.Find("Menu/Scroll View/Viewport/Content/Button_Reset/Confirmation Text").gameObject.SetActive(false);
            transform.Find("Menu/Scroll View/Viewport/Content/Button_Reset/Text").gameObject.SetActive(true);
        }
    }
    public void Button_Reset(GameObject frame)
    {
        if(frame.transform.Find("Final Text").gameObject.activeSelf)
        {
            Globals.GetLogic().GetComponent<Builder>().Reset();
            Globals.GetLogic().GetComponent<Builder>().GenerateDeposits();
            Button_Menu();
        }
        else if (frame.transform.Find("Confirmation Text").gameObject.activeSelf)
        {
            frame.transform.Find("Confirmation Text").gameObject.SetActive(false);
            frame.transform.Find("Final Text").gameObject.SetActive(true);
        }
        else if (frame.transform.Find("Text").gameObject.activeSelf)
        {
            frame.transform.Find("Text").gameObject.SetActive(false);
            frame.transform.Find("Confirmation Text").gameObject.SetActive(true);
        }
    }
    public void Button_Audio(Toggle toggle)
    {
        PlayerPrefs.SetInt("Audio", toggle.isOn?1:0);
    }
    public void Button_Autosave(Toggle toggle)
    {
        PlayerPrefs.SetInt("Autosave", toggle.isOn ? 1 : 0);
    }
    public void Button_Godmode(Toggle toggle)
    {
        PlayerPrefs.SetInt("Godmode", toggle.isOn ? 1 : 0);
    }
}
