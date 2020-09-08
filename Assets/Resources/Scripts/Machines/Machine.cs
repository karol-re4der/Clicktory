using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Machine : MonoBehaviour
{
    private List<KeyValuePair<string, int>> buildingCost;
    public List<string> editor_buildingCostKey;
    public List<int> editor_buildingCostValue;
    public int buildingLimit;

    public List<Tile> owners;
    public FlowingResource store;
    public int rotation;

    public string description = "";

    public string type;
    public bool removable;
    public bool obstructing;
    public int sizeX;
    public int sizeY;
    public List<GameObject> parts;
    public bool turnedOff = false;
    public bool selfActivating = false;
    public int activationTime;
    public int activationTimer = 0;

    private List<Gate> gates;

    private float lastTap;
    private float doubleTapSpeed = 0.25f;

    public int GetRemainingBuildingLimit()
    {
        if (buildingLimit < 0)
        {
            return 1;
        }
        return GetBuildingLimit() - GetCurrentlyBuilt();
    }
    public int GetCurrentlyBuilt()
    {
        return GameObject.FindGameObjectsWithTag("Machine").Where((x) => x.GetComponent<Machine>().type.Equals(type)).ToList().Count;
    }
    public int GetBuildingLimit()
    {
        return buildingLimit;
    }

    public Sprite GetIcon(int rotation = 0)
    {
        return Resources.LoadAll<Sprite>("Textures/Interface/Building Icons/"+type+"_Icon_Spritesheet")[rotation];
    }

    public List<KeyValuePair<string, int>> GetBuildingCost()
    {
        if (buildingCost!=null)
        {
            return buildingCost;
        }
        else
        {
            buildingCost = new List<KeyValuePair<string, int>>();
            for(int i = 0; i<editor_buildingCostKey.Count; i++)
            {
                buildingCost.Add(new KeyValuePair<string, int>(editor_buildingCostKey[i], editor_buildingCostValue[i]));
            }
            editor_buildingCostKey.Clear();
            editor_buildingCostValue.Clear();
            return buildingCost;
        }
    }
    public List<Gate> GetGates()
    {
        if (gates==null)
        {
            gates = new List<Gate>();
            foreach (GameObject part in parts)
            {
                if (part.GetComponent<Gate>())
                {
                    gates.AddRange(part.GetComponents<Gate>());
                }
            }
        }
        return gates;
    }

    public Vector3 GetCenterPoint()
    {
        Vector2 centerPoint = parts[0].transform.position;
        if (sizeX * sizeY < 4)
        {
            for (int i = 1; i < parts.Count; i++)
            {
                centerPoint = Vector2.Lerp(centerPoint, parts[i].transform.position, 0.5f);
            }
        }
        else if (sizeX * sizeY == 4)
        {
            centerPoint = Vector2.Lerp(Vector2.Lerp(parts[0].transform.position, parts[1].transform.position, 0.5f), Vector2.Lerp(parts[2].transform.position, parts[3].transform.position, 0.5f), 0.5f);
        }
        else if(sizeX*sizeY == 6)
        {
            centerPoint = Vector2.Lerp(Vector2.Lerp(parts[0].transform.position, parts[1].transform.position, 0.5f), Vector2.Lerp(parts[4].transform.position, parts[5].transform.position, 0.5f), 0.5f);
        }
        else if (sizeX * sizeY == 9)
        {
            centerPoint = parts[5].transform.position;
        }

        return centerPoint;
    }

    public void Initiate(List<Tile> owners, int rotation)
    {
        lastTap = Time.time;

        this.owners = owners;
        this.rotation = rotation;
        RefreshSprite();

        int i = 0;
        foreach(GameObject part in parts)
        {
            part.GetComponent<MachinePart>().tile = owners.ElementAt(i);
            part.transform.position = owners.ElementAt(i).transform.position;
            part.transform.Translate(new Vector3(0, owners.ElementAt(i).GetComponent<SpriteRenderer>().size.y / 2, 0));
            part.GetComponent<SpriteRenderer>().sortingOrder = owners.ElementAt(i).GetComponent<SpriteRenderer>().sortingOrder + 1;
            if (obstructing)
            {
                part.GetComponent<SpriteRenderer>().sortingOrder++;
            }
            owners.ElementAt(i).machine = this.gameObject;

            part.GetComponent<MachinePart>().tile.type = "Concrete";
            part.GetComponent<MachinePart>().tile.RefreshSprite();
            part.GetComponent<MachinePart>().tile.ClearTrinket();
            i++;
        }
    }
    public virtual void Activate()
    {
        if (CanActivate())
        {
            LaunchSmoke();
            EnableAnimations();
            activationTimer = activationTime;
        }
    }
    public virtual void EndActivation()
    {
        StopSmoke();
        StopAnimations();
    }
    public virtual bool CanActivate()
    {
        return !turnedOff && !IsInvoking("EndActivation") && activationTimer<0;
    }
    public virtual bool CanEndActivation()
    {
        return activationTimer == 0;
    }
    public virtual void InOut()
    {
        foreach (GameObject part in parts)
        {
            part.GetComponents<Gate>().ToList().ForEach((x) => x.Activate());
        }
    }
    public void RefreshSprite()
    {
        foreach(GameObject part in parts)
        {
            part.GetComponent<MachinePart>().RefreshSprite(rotation);
        }
    }

    public void LaunchSmoke()
    {
        foreach(GameObject part in parts)
        {
            part.GetComponent<MachinePart>().LaunchSmoke();
        }
    }
    public void StopSmoke()
    {
        foreach (GameObject part in parts)
        {
            part.GetComponent<MachinePart>().StopSmoke();
        }
    }
    public void Animate(int timer)
    {
        foreach (GameObject part in parts)
        {
            part.GetComponent<MachinePart>().Animate(timer);
        }
    }
    public void EnableAnimations()
    {
        foreach (GameObject part in parts)
        {
            part.GetComponent<MachinePart>().animating = true;
        }
    }
    public void StopAnimations()
    {
        foreach (GameObject part in parts)
        {
            part.GetComponent<MachinePart>().animating = false;
        }
    }


    public int SpriteOrderDirection(int in_dir, int out_dir) //not applying to corners yet
    {
        if (out_dir == (in_dir + 2) % 4)
        {
            if (out_dir == 0 || out_dir == 3)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
        return 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseUp()
    {
        if (Globals.IsPointerInGame())
        {
            if (Globals.GetInterface().IsDemolishing())
            {
                if (Input.touches.Length > 0 && Input.touches[0].deltaPosition == Vector2.zero)
                {
                    GameObject.Find("Map").GetComponent<Builder>().DemolishOnTile(owners.First());
                }
                else if (Application.isEditor)
                {
                    GameObject.Find("Map").GetComponent<Builder>().DemolishOnTile(owners.First());
                }
            }
            else if(Time.time - lastTap <= doubleTapSpeed)
            {
                Camera.main.GetComponent<CameraHandler>().CenterCamera(GetCenterPoint(), instant: false);
                Globals.GetInterface().activeMachine = this;
            }
            lastTap = Time.time;
        }
    }

}
