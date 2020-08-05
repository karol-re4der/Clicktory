using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Machine : MonoBehaviour
{
    public List<Tile> owners;
    public FlowingResource store;
    public int rotation;

    public string type;
    public bool removable;
    public bool obstructing;
    public int sizeX;
    public int sizeY;
    public List<GameObject> parts;
    public bool turnedOff = false;

    private List<Gate> gates;

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

    public void Initiate(List<Tile> owners, int rotation)
    {
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

            part.GetComponent<MachinePart>().tile.ClearTrinket();
            i++;
        }
    }
    public virtual void Activate()
    {

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
        if (!EventSystem.current.IsPointerOverGameObject(0))
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
        }
    }

}
