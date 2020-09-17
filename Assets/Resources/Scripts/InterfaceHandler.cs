using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.EventSystems;
using TMPro;

public class InterfaceHandler : MonoBehaviour
{
    public List<GameObject> unlockable = new List<GameObject>();

    public int rotation = 0;
    public int currFilter = 0;

    public Machine activeMachine;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        //Update stats window
        if(transform.Find("Stats Window/Content").gameObject.activeSelf)
        {
            TextMeshProUGUI text = transform.Find("Stats Window/Content/Scroll View/Viewport/Content/Text/").gameObject.GetComponent<TextMeshProUGUI>();
            bool isSession = transform.Find("Stats Window/Content/Name").GetComponent<TextMeshProUGUI>().text.Equals("Session Stats");
            text.text = "";
            foreach (Stats.Stat stat in isSession?Globals.GetSessionStats().stats:Globals.GetGameStats().stats)
            {
                text.text += stat.name + ":\n" + Globals.ParseNumber((int)stat.value) + "\n\n";
            }
        }

        //Update machine view
        if (activeMachine)
        {
            if(!transform.Find("Machine Bar").gameObject.activeSelf)
            {
                transform.Find("Machine Bar").gameObject.SetActive(true);

                transform.Find("Machine Bar/Scroll View/Viewport/Content/Turnoff").GetComponent<ToggleWithIndicator>().isOn = !activeMachine.turnedOff;

                transform.Find("Machine Bar/Scroll View/Viewport/Content/Name/").gameObject.GetComponent<TextMeshProUGUI>().text = activeMachine.type+(activeMachine.tier>1?(" "+activeMachine.tier):"");

            }

        }
        else
        {
            transform.Find("Machine Bar").gameObject.SetActive(false);
        }

        //Update res display
        Transform frame = GameObject.Find("Canvas/Top Bar/Resources/").transform;
        foreach (ResourceStore.Res res in Globals.GetResources().res)
        {
            Transform obj = frame.Find(res.type);
            if (obj != null)
            {
                obj.Find("Amount").GetComponent<TextMeshProUGUI>().text = Globals.ParseNumber(res.amount);
            }
            else
            {
                GameObject newRes = Instantiate(Resources.Load("UI/ResourceFrame") as GameObject, frame);
                newRes.name = res.type;
                
                newRes.transform.GetChild(0).GetComponent<Image>().sprite = Globals.GetSave().GetResources().FindResSprite(res.type);
            }
        }
    }

    public void CheckUnlocks()
    {
        foreach (GameObject obj in unlockable)
        {
            obj.GetComponent<TechUnlockable>().Check();
        }
    }
    public void LoadWorkshop()
    {
        GameObject frame = transform.Find("Workshop Window/Content/Scroll View/Viewport/Content/").gameObject;

        //clearing
        foreach(Transform trans in frame.transform)
        {
            Destroy(trans.gameObject);
        }
        frame.transform.DetachChildren();

        //filling
        foreach(Logic.UpgradeTree.Upgrade upgrade in Globals.GetLogic().workshop.upgradeTree)
        {
            GameObject newFrame = Instantiate(Resources.Load("UI/UnlockFrame") as GameObject, frame.transform);
            newFrame.name = upgrade.name;
            newFrame.GetComponent<TechUnlockable>().tech = upgrade.techTypeRequired;
            newFrame.GetComponent<TechUnlockable>().required = upgrade.techLevelRequired;

            newFrame.transform.Find("Top/Name").GetComponent<TextMeshProUGUI>().text = upgrade.name;
            newFrame.transform.Find("Scroll View/Viewport/Content/Desc/").GetComponent<TextMeshProUGUI>().text = upgrade.desc.Replace("%d", ""+upgrade.increase);

            foreach (Logic.UpgradeTree.Upgrade.Pair cost in upgrade.cost)
            {
                GameObject newRes = Instantiate(Resources.Load("UI/ResourceFrame") as GameObject, newFrame.transform.Find("Cost/Scroll View/Viewport/Content/"));
                newRes.name = cost.res;
                newRes.transform.Find("Icon").GetComponent<Image>().sprite = Globals.GetSave().GetResources().FindResSprite(cost.res);
                newRes.transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = Globals.ParseNumber(cost.amount);
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
            bar.GetComponent<RectTransform>().anchorMax = new Vector2(bar.GetComponent<RectTransform>().anchorMax.x + Globals.GetTapPower(), 1f);
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
            if (!trans.name.Equals("Rotate") && !trans.name.Equals("Demolish") && !trans.name.Equals("Filter"))
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

    public void Button_Filter(GameObject frame)
    {
        string tierText = frame.transform.Find("Filter/Text").GetComponent<TextMeshProUGUI>().text;
        int tier = 0;
        if(tierText.Equals("Tier 1"))
        {
            tier = 2;
            tierText = "Tier 2";
        }
        else if(tierText.Equals("Tier 2"))
        {
            tier = 0;
            tierText = "All Tiers";
        }
        else
        {
            tier = 1;
            tierText = "Tier 1";
        }

        currFilter = tier;
        frame.transform.Find("Filter/Text").GetComponent<TextMeshProUGUI>().text = tierText;
        frame.transform.Find("Filter/Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/Interface/Icons/Filter_" + tier);


        foreach (Transform button in frame.transform)
        {
            if(!button.gameObject.name.Equals("Rotate") && !button.gameObject.name.Equals("Demolish") && !button.gameObject.name.Equals("Filter"))
            {
                GameObject template = Resources.Load("Prefabs/Machines/" + button.gameObject.name) as GameObject;
                if (tier == 0 || tier == template.GetComponent<Machine>().tier)
                {
                    button.gameObject.SetActive(true);
                    if (button.gameObject.GetComponent<TechUnlockable>())
                    {
                        button.gameObject.GetComponent<TechUnlockable>().blocked = false;
                    }
                }
                else
                {
                    button.gameObject.SetActive(false);
                    if (button.gameObject.GetComponent<TechUnlockable>())
                    {
                        button.gameObject.GetComponent<TechUnlockable>().blocked = true;
                    }
                }
            }
        }
    }
    public void Button_Menu()
    {
        if (Globals.IsBuilding())
        {
            Button_Expand(GameObject.Find("Canvas").transform.Find("Bottom Bar").gameObject);

        }

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
    public void Button_Research()
    {
        if (Globals.IsBuilding())
        {
            Button_Expand(GameObject.Find("Canvas").transform.Find("Bottom Bar").gameObject);

        }

        transform.Find("Research Window").GetComponent<Submenu>().Enter();

        Transform frame = transform.Find("Research Window/Content/Paths/Industry");
        int i = Globals.GetSave().industrialTech;
        if (i < Globals.GetLogic().industrialTech.techTree.Count())
        {
            frame.gameObject.SetActive(true);
            Logic.TechTree.Tech tech = Globals.GetLogic().industrialTech.techTree[i];
            frame.Find("Top/Name").GetComponent<TextMeshProUGUI>().text = tech.name;
            frame.Find("Scroll View/Viewport/Content/Desc").GetComponent<TextMeshProUGUI>().text = tech.desc;

            foreach (Transform trans in frame.Find("Cost/Scroll View/Viewport/Content/"))
            {
                Destroy(trans.gameObject);
            }
            foreach (Logic.TechTree.Tech.Pair cost in tech.cost)
            {
                GameObject newRes = Instantiate(Resources.Load("UI/ResourceFrame") as GameObject, frame.Find("Cost/Scroll View/Viewport/Content/"));
                newRes.name = cost.res;
                newRes.transform.Find("Icon").GetComponent<Image>().sprite = Globals.GetSave().GetResources().FindResSprite(cost.res);
                newRes.transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = Globals.ParseNumber(cost.amount);
            }
        }
        else
        {
            frame.gameObject.SetActive(false);
        }

        frame = transform.Find("Research Window/Content/Paths/Science");
        i = Globals.GetSave().scientificTech;
        if (i < Globals.GetLogic().scientificTech.techTree.Count())
        {
            frame.gameObject.SetActive(true);
            Logic.TechTree.Tech tech = Globals.GetLogic().scientificTech.techTree[i];
            frame.Find("Top/Name").GetComponent<TextMeshProUGUI>().text = tech.name;
            frame.Find("Scroll View/Viewport/Content/Desc").GetComponent<TextMeshProUGUI>().text = tech.desc;

            foreach (Transform trans in frame.Find("Cost/Scroll View/Viewport/Content/"))
            {
                Destroy(trans.gameObject);
            }
            foreach (Logic.TechTree.Tech.Pair cost in tech.cost)
            {
                GameObject newRes = Instantiate(Resources.Load("UI/ResourceFrame") as GameObject, frame.Find("Cost/Scroll View/Viewport/Content/"));
                newRes.name = cost.res;
                newRes.transform.Find("Icon").GetComponent<Image>().sprite = Globals.GetSave().GetResources().FindResSprite(cost.res);
                newRes.transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = Globals.ParseNumber(cost.amount);
            }
        }
        else
        {
            frame.gameObject.SetActive(false);
        }

        frame = transform.Find("Research Window/Content/Paths/Logistics");
        i = Globals.GetSave().logisticTech;
        if (i < Globals.GetLogic().logisticTech.techTree.Count())
        {
            frame.gameObject.SetActive(true);
            Logic.TechTree.Tech tech = Globals.GetLogic().logisticTech.techTree[i];
            frame.Find("Top/Name").GetComponent<TextMeshProUGUI>().text = tech.name;
            frame.Find("Scroll View/Viewport/Content/Desc").GetComponent<TextMeshProUGUI>().text = tech.desc;

            foreach (Transform trans in frame.Find("Cost/Scroll View/Viewport/Content/"))
            {
                Destroy(trans.gameObject);
            }
            foreach (Logic.TechTree.Tech.Pair cost in tech.cost)
            {
                GameObject newRes = Instantiate(Resources.Load("UI/ResourceFrame") as GameObject, frame.Find("Cost/Scroll View/Viewport/Content/"));
                newRes.name = cost.res;
                newRes.transform.Find("Icon").GetComponent<Image>().sprite = Globals.GetSave().GetResources().FindResSprite(cost.res);
                newRes.transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = Globals.ParseNumber(cost.amount);
            }
        }
        else
        {
            frame.gameObject.SetActive(false);
        }
    }
    public void Button_Workshop()
    {
        if (Globals.IsBuilding())
        {
            Button_Expand(GameObject.Find("Canvas").transform.Find("Bottom Bar").gameObject);
        }

        transform.Find("Workshop Window").GetComponent<Submenu>().Enter();


        Transform frame = transform.Find("Workshop Window/Content/Scroll View/Viewport/Content/");

        foreach (Transform trans in frame)
        {
            foreach (Transform nextRes in trans.Find("Cost/Scroll View/Viewport/Content/"))
            {
                Destroy(nextRes.gameObject);
            }
            trans.transform.Find("Cost/Scroll View/Viewport/Content/").DetachChildren();

            foreach (Logic.UpgradeTree.Upgrade.Pair cost in Globals.GetLogic().workshop.upgradeTree.Find((x)=>x.name.Equals(trans.gameObject.name)).cost)
            {
                GameObject newRes = Instantiate(Resources.Load("UI/ResourceFrame") as GameObject, trans.Find("Cost/Scroll View/Viewport/Content/"));
                newRes.name = cost.res;
                newRes.transform.Find("Icon").GetComponent<Image>().sprite = Globals.GetSave().GetResources().FindResSprite(cost.res);
                newRes.transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = Globals.ParseNumber(cost.amount);
            }
        }
    }
    public void Button_Stats()
    {
        if (Globals.IsBuilding())
        {
            Button_Expand(GameObject.Find("Canvas").transform.Find("Bottom Bar").gameObject);
        }

        transform.Find("Stats Window").GetComponent<Submenu>().Enter();
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
    public void Button_Tech(GameObject button)
    {
        Globals.GetLogic().TechUp(button.name);
        CheckUnlocks();
    }
    public void Button_Switch_Stats(GameObject frame)
    {
        if(frame.transform.Find("Name").GetComponent<TextMeshProUGUI>().text.Equals("Session Stats"))
        {
            frame.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = "Overall stats";
            frame.transform.Find("Button/Text").GetComponent<TextMeshProUGUI>().text = "Show Session";
        }
        else
        {
            frame.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = "Session Stats";
            frame.transform.Find("Button/Text").GetComponent<TextMeshProUGUI>().text = "Show Overall";

        }
    }


    public void Button_Machine_Turnoff(ToggleWithIndicator toggle)
    {
        if (toggle.isOn == activeMachine.turnedOff)
        {
            activeMachine.turnedOff = !activeMachine.turnedOff;
            toggle.isOn = !activeMachine.turnedOff;
        }
    }
    public void Button_Machine_Demolish()
    {
        GameObject.Find("Map").GetComponent<Builder>().DemolishOnTile(activeMachine.parts[0].GetComponent<MachinePart>().tile);
    }
    public void Button_Machine_Upgrade()
    {

    }

    public void Toggle_Hide_Details(GameObject toggle)
    {
        if (!toggle.GetComponent<ToggleWithIndicator>().group.AnyTogglesOn())
        {
            Button_Hide_Details();
        }
        else if(transform.Find("Details Window/Content").gameObject.activeSelf)
        {
            Button_Show_Details(toggle);
        }
    }
    public void Button_Hide_Details()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Submenu"))
        {
            obj.GetComponent<Submenu>().Exit();
        }
    }
    public void Button_Show_Details(GameObject building)
    {
        transform.Find("Details Window").GetComponent<Submenu>().Enter();

        Machine template = (Resources.Load("Prefabs/Machines/" + building.name) as GameObject).GetComponent<Machine>();

        transform.Find("Details Window/Content/Name").GetComponent<TextMeshProUGUI>().text = template.type;
        transform.Find("Details Window/Content/Description/Viewport/Content/Text").GetComponent<TextMeshProUGUI>().text = template.description;
        transform.Find("Details Window/Content/Building Cost/Availability/Current").GetComponent<TextMeshProUGUI>().text = "" + template.GetCurrentlyBuilt();
        if (template.GetBuildingLimit() < 0)
        {
            transform.Find("Details Window/Content/Building Cost/Availability/Max").gameObject.SetActive(false);
            transform.Find("Details Window/Content/Building Cost/Availability/Out of").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("Details Window/Content/Building Cost/Availability/Max").gameObject.SetActive(true);
            transform.Find("Details Window/Content/Building Cost/Availability/Out of").gameObject.SetActive(true);
            transform.Find("Details Window/Content/Building Cost/Availability/Max").GetComponent<TextMeshProUGUI>().text = "" + template.GetBuildingLimit();
        }

        int i = 0;
        foreach (Transform trans in transform.Find("Details Window/Content/Rotations/Viewport/Content/"))
        {
            trans.Find("Image").GetComponent<Image>().sprite = template.GetIcon(i);
            i++;
        }

        foreach (Transform trans in transform.Find("Details Window/Content/Building Cost/Scroll View/Viewport/Content/"))
        {
            Destroy(trans.gameObject);
        }
        foreach (KeyValuePair<string, int> cost in template.GetBuildingCost())
        {
            GameObject newRes = Instantiate(Resources.Load("UI/ResourceFrame") as GameObject, transform.Find("Details Window/Content/Building Cost/Scroll View/Viewport/Content/"));
            newRes.name = cost.Key;
            newRes.transform.Find("Icon").GetComponent<Image>().sprite = Globals.GetSave().GetResources().FindResSprite(cost.Key);
            newRes.transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = Globals.ParseNumber(cost.Value);
        }

        float newAnchor = transform.Find("Bottom Bar").GetComponent<RectTransform>().anchorMax.y;
        transform.Find("Details Window").GetComponent<RectTransform>().anchorMin = new Vector2(0, newAnchor);
    }
}
