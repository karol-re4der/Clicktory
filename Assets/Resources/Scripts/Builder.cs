using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Builder : MonoBehaviour
{
    public GameObject selection;
    public int worldSize = 100;
    public float gridShiftX = 78f / 79f;
    public float gridShiftY = 19f / 79f;
    public int deposits = 10;
    public int depositSize = 4;

    public void BuildFromSelection(Tile tile)
    {
        if (tile.machine == null)
        {
            if (selection.GetComponent<ToggleGroup>().ActiveToggles().Count()>0)
            {
                tile.NewMachine(selection.GetComponent<ToggleGroup>().ActiveToggles().ElementAt(0).gameObject.name, Globals.GetInterface().rotation);
            }
        }
    }

    public void DemolishOnTile(Tile tile)
    {
        tile.RemoveMachine();
    }

    public void DeleteTile(GameObject tileObject)
    {
        if (tileObject.GetComponent<Tile>())
        {
            if (tileObject.GetComponent<Tile>().machine)
            {
                tileObject.GetComponent<Tile>().RemoveMachine();
            }
        }
        Destroy(tileObject);
    }

    public void Reset()
    {
        foreach(GameObject o in GameObject.FindGameObjectsWithTag("Resource"))
        {
            o.GetComponent<FlowingResource>().Store();
        }
        for(int x = 0; x< Globals.GetSave().GetGrid().GetLength(0); x++)
        {
            for(int y = 0; y< Globals.GetSave().GetGrid().GetLength(1); y++)
            {
                DeleteTile(Globals.GetSave().GetGrid()[x,y]);
            }
        }
        Globals.GetSave().GetGrid() = null;
        SetStartingState();
    }

    public void SetStartingState()
    {
        Globals.GetSave().GetGrid() = new GameObject[worldSize, worldSize * 2];
        for (int x = 0; x < Globals.GetSave().GetGrid().GetLength(0); x++)
        {
            for (int y = 0; y < Globals.GetSave().GetGrid().GetLength(1); y++)
            {
                if (y == 0)
                {
                    Globals.GetSave().GetGrid()[x,y] = NewTile(x, y, "Filler");
                }
                else if (x == Globals.GetSave().GetGrid().GetLength(0) - 1 && y % 2 == 1)
                {
                    Globals.GetSave().GetGrid()[x, y] = NewTile(x, y, "Filler");
                }
                else
                {
                    Globals.GetSave().GetGrid()[x, y] = NewTile(x, y, "Tile");
                    Globals.GetSave().GetGrid()[x, y].GetComponent<Tile>().RefreshSprite();
                }
            }
        }

        //Globals.GetLogic().grid[5, 10].GetComponent<Tile>().NewMachine("Output", Globals.GetLogic().grid[5, 10], 1);
        //grid[5, 9].GetComponent<Tile>().NewMachine("Conveyor", grid[5, 9], 1);
        //grid[6, 8].GetComponent<Tile>().NewMachine("Spliter", grid[6, 8], 0);

        //grid[5, 7].GetComponent<Tile>().NewMachine("Clockwise Corner", grid[5, 7], 1);
        //grid[6, 9].GetComponent<Tile>().NewMachine("Counterclockwise Corner", grid[6, 9], 1);

        //grid[7, 8].GetComponent<Tile>().NewMachine("Conveyor", grid[7, 8], 1);
        //grid[6, 6].GetComponent<Tile>().NewMachine("Upgrader", grid[6, 6], 1);

        //grid[6, 5].GetComponent<Tile>().NewMachine("Clockwise Corner", grid[6, 5], 0);
        //grid[7, 7].GetComponent<Tile>().NewMachine("Counterclockwise Corner", grid[7, 7], 2);

        //grid[7, 6].GetComponent<Tile>().NewMachine("Merger", grid[7, 6], 1);
        //grid[7, 5].GetComponent<Tile>().NewMachine("Conveyor", grid[7, 5], 1);
        //Globals.GetLogic().grid[8, 4].GetComponent<Tile>().NewMachine("Input", Globals.GetLogic().grid[8, 4], 1);

        //link tiles
        for (int x = 0; x < Globals.GetSave().GetGrid().GetLength(0); x++)
        {
            for (int y = 0; y < Globals.GetSave().GetGrid().GetLength(1); y++)
            {
                if (Globals.GetSave().GetGrid()[x, y].GetComponent<Tile>())
                {
                    Globals.GetSave().GetGrid()[x, y].GetComponent<Tile>().LinkUp();
                }
            }
        }

        GenerateDeposits();
    }

    public void GenerateDeposits()
    {
        for (int i = 0; i < deposits; i++) {
            int x = (int)(Random.Range(0, 1f) * (Globals.GetSave().GetGrid().GetLength(0) - 2) + 1);
            int y = (int)(Random.Range(0, 1f) * (Globals.GetSave().GetGrid().GetLength(1) - 2) + 1);

            for(int xx = 0; xx<depositSize; xx++)
            {
                for(int yy = 0; yy<depositSize; yy++)
                {
                    if (x + xx < Globals.GetSave().GetGrid().GetLength(0) && y + yy < Globals.GetSave().GetGrid().GetLength(1) && Globals.GetSave().GetGrid()[x + xx, y + yy].GetComponent<Tile>())
                    {
                        Globals.GetSave().GetGrid()[x + xx, y + yy].GetComponent<Tile>().type = "Stone";
                        Globals.GetSave().GetGrid()[x + xx, y + yy].GetComponent<Tile>().RefreshSprite();
                    }
                }
            }
        }
    }

    private GameObject NewTile(int x, int y, string type)
    {
        GameObject obj = GameObject.Instantiate(Resources.Load("Prefabs/" + type) as GameObject, GameObject.Find("Map/Tiles").transform);
        obj.name = type + " " + x + ":" + y;
        if (type.Equals("Tile"))
        {
            obj.GetComponent<Tile>().x = x;
            obj.GetComponent<Tile>().y = y;
        }
        if (Globals.GetSave().GetGrid()[x, y] != null)
        {
            GameObject.Destroy(Globals.GetSave().GetGrid()[x, y].GetComponent<Tile>().machine);
            GameObject.Destroy(Globals.GetSave().GetGrid()[x, y]);
        }

        float pos_x = obj.GetComponent<SpriteRenderer>().size.x * gridShiftX * (x - Globals.GetSave().GetGrid().GetLength(0) / 2) + (y % 2) * obj.GetComponent<SpriteRenderer>().size.x * gridShiftX / 2;


        float pos_y = obj.GetComponent<SpriteRenderer>().size.y * gridShiftY * (y - Globals.GetSave().GetGrid().GetLength(1));

        obj.transform.position = new Vector3(pos_x, pos_y, -1);
        obj.GetComponent<SpriteRenderer>().sortingOrder = (2 * worldSize - y)*2;
        return obj;
    }
}
