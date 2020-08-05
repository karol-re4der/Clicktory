﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public GameObject machine;
    private GameObject trinket;
    public string type = "Grass";
    public int x = 0;
    public int y = 0;

    public Tile[] links;

    void Update()
    {
        
    }
    public void RefreshSprite()
    {
        switch (type)
        {
            case "Concrete":
                GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Tiles/Tile_Spritesheet")[0];
                break;
            case "Grass":
                GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Tiles/Tile_Spritesheet")[1];
                break;
            case "Stone":
                GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Tiles/Tile_Spritesheet")[2];
                break;
            default:
                GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Textures/Tiles/Tile_Spritesheet")[0];
                break;
        }

        if (!machine)
        {
            ClearTrinket();
            if (Random.Range(0, 1f) > 0.33f)
            {
                trinket = Instantiate(Resources.Load("Prefabs/Trinket") as GameObject, transform);
                Sprite[] sprites = Resources.LoadAll<Sprite>("Textures/Tiles/Trinkets_"+type+"_Spritesheet");
                trinket.GetComponent<SpriteRenderer>().sprite = sprites[(int)(Random.Range(0, 1f) * sprites.Length)];

                trinket.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
                trinket.transform.Translate(new Vector3(0, GetComponent<SpriteRenderer>().size.y / 2, 0));
            }
        }
    }
    public void ClearTrinket()
    {
        if (trinket)
        {
            Destroy(trinket);
        }
    }

    public Tile(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool IsSpaceAvailable()
    {
        return machine == null;
    }
    public bool IsSpaceAvailable(GameObject template, int rotation)
    {
        for (int x = 0; x < template.GetComponent<Machine>().sizeX; x++)
        {
            for (int y = 0; y < template.GetComponent<Machine>().sizeY; y++)
            {
                Tile newTile = this;
                for (int xx = 0; xx < x; xx++)
                {
                    newTile = newTile.links[rotation];
                }
                for (int yy = 0; yy < y; yy++)
                {
                    newTile = newTile.links[(rotation + 1) % 4];
                }
                if (!newTile || !newTile.IsSpaceAvailable())
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void NewMachine(string type, int rotation)
    {
        GameObject template = Resources.Load("Prefabs/Machines/" + type) as GameObject;

        if (IsSpaceAvailable(template, rotation))
        {
            machine = GameObject.Instantiate(template, GameObject.Find("Map/Machines").transform);
            machine.transform.position = transform.position;

            List<Tile> newOwners = new List<Tile>();
            for(int x = 0; x<machine.GetComponent<Machine>().sizeX; x++)
            {
                for(int y = 0; y<machine.GetComponent<Machine>().sizeY; y++)
                {
                    Tile newTile = this;
                    for (int xx = 0; xx < x; xx++)
                    {
                        newTile = newTile.links[rotation];
                    }
                    for (int yy = 0; yy < y; yy++)
                    {
                        newTile = newTile.links[(rotation+1)%4];
                    }
                    newOwners.Add(newTile);
                }
            }

            machine.GetComponent<Machine>().Initiate(newOwners, rotation);
        }
    }
    public void RemoveMachine()
    {
        if (machine && machine.GetComponent<Machine>().removable)
        {
            foreach (Gate gate in machine.GetComponents<Gate>())
            {
                if (gate.res)
                {
                    gate.res.Store();
                    gate.res = null;
                }
                else if (gate.GetLink() && gate.GetLink().res)
                {
                    gate.GetLink().res.Store();
                    gate.GetLink().res = null;
                }
            }
            if (machine.GetComponent<Machine>().store)
            {
                machine.GetComponent<Machine>().store.Dispose();
                machine.GetComponent<Machine>().store = null;
            }
            foreach(GameObject part in machine.GetComponent<Machine>().parts)
            {
                Destroy(part);
            }
            Destroy(machine.gameObject);
            machine = null;
        }
    }

    public void OnMouseUp()
    {
        bool foo = false;

        if (!Application.isEditor)
        {
            foo = !EventSystem.current.IsPointerOverGameObject(0);
        }
        else
        {
            foo = !EventSystem.current.IsPointerOverGameObject();
        }

        if (foo)
        {
            if (Globals.GetInterface().IsDemolishing())
            {
                if (Input.touches.Length > 0 && Input.touches[0].deltaPosition == Vector2.zero)
                {
                    GameObject.Find("Map").GetComponent<Builder>().DemolishOnTile(this);
                }
                else if (Application.isEditor)
                {
                    GameObject.Find("Map").GetComponent<Builder>().DemolishOnTile(this);
                }
            }
            else
            {
                if (Input.touches.Length > 0 && Input.touches[0].deltaPosition == Vector2.zero)
                {
                    GameObject.Find("Map").GetComponent<Builder>().BuildFromSelection(this);
                }
                else if (Application.isEditor)
                {
                    GameObject.Find("Map").GetComponent<Builder>().BuildFromSelection(this);
                }
            }
        }
    }

    public void OnMouseDown()
    {
        ClearTrinket();
    }

    public void LinkUp()
    {
        links = new Tile[4];
        for (int i = 0; i < 4; i++)
        {
            Vector2 d = Vector2.zero;
            if (y % 2 != 0)
            {
                switch (i)
                {
                    case 0:
                        d.x = 1;
                        d.y = 1;
                        break;
                    case 1:
                        d.x = 1;
                        d.y = -1;
                        break;
                    case 2:
                        d.y = -1;
                        break;
                    case 3:
                        d.y = 1;
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0:
                        d.y = 1;
                        break;
                    case 1:
                        d.y = -1;
                        break;
                    case 2:
                        d.x = -1;
                        d.y = -1;
                        break;
                    case 3:
                        d.x = -1;
                        d.y = 1;
                        break;
                }
            }

            if (x + d.x >= 0 && y + d.y >= 0)
            {
                if (x + (int)d.x < Globals.GetSave().GetGrid().GetLength(0) && y + (int)d.y < Globals.GetSave().GetGrid().GetLength(1))
                {
                    if (Globals.GetSave().GetGrid()[x + (int)d.x, y + (int)d.y] != null)
                    {
                        links[i] = Globals.GetSave().GetGrid()[x + (int)d.x, y + (int)d.y].GetComponent<Tile>();
                    }
                }
            }
        }
    }
}