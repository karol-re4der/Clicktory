using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveFile
{
    [System.Serializable]
    public class TileData
    {
        public int posX;
        public int posY;
        public int rotation;
        public string content = "";
        public string deposit = "";
        public string res = "";
        public int activeRecipe = 0;
        public bool turnedOff = false;
    }

    private GameObject[,] grid;

    [SerializeField]
    private ResourceStore res;
    [SerializeField]
    public List<TileData> saveData;
    [SerializeField]
    public int logisticTech = 0;
    [SerializeField]
    public int industrialTech = 0;
    [SerializeField]
    public int scientificTech = 0;
    [SerializeField]
    public Stats gameStats = new Stats();
    [SerializeField]
    public List<string> unlocks = new List<string>();

    private string locationPath;
    private string saveExtension = ".save";
    private string saveName = "save";

    public bool IsUnlocked(string name)
    {
        return unlocks.Contains(name);
    }
    public void Unlock(string name)
    {
        if (!IsUnlocked(name))
        {
            unlocks.Add(name);
        }
    }

    public ref GameObject[,] GetGrid()
    {
        return ref grid;
    }
    public ref ResourceStore GetResources()
    {
        return ref res;
    }

    public SaveFile()
    {
        locationPath = Application.persistentDataPath + "/Saves/";
        res = new ResourceStore();
    }

    public void PrepareForSaving()
    {
        saveData = new List<TileData>();
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Machine"))
        {
            TileData newTile = new TileData();
            newTile.posX = obj.GetComponent<Machine>().parts[0].GetComponent<MachinePart>().tile.x;
            newTile.posY = obj.GetComponent<Machine>().parts[0].GetComponent<MachinePart>().tile.y;
            newTile.rotation = obj.GetComponent<Machine>().rotation;
            newTile.content = obj.GetComponent<Machine>().type;

            if (obj.GetComponent<Manufactory>())
            {
                newTile.activeRecipe = obj.GetComponent<Manufactory>().availableRecipes.IndexOf(obj.GetComponent<Manufactory>().currentRecipe);
            }

            if (obj.GetComponent<Machine>().turnedOff)
            {
                newTile.turnedOff = true;
            }

            if (obj.GetComponent<Machine>().tier > 1)
            {
                newTile.content += " " + obj.GetComponent<Machine>().tier;
            }
            saveData.Add(newTile);

            foreach(Gate gate in obj.GetComponent<Machine>().GetGates().Where((x)=>x.res))
            {
                TileData newRes = new TileData();
                newRes.posX = gate.GetComponent<MachinePart>().tile.x;
                newRes.posY = gate.GetComponent<MachinePart>().tile.y;
                newRes.rotation = gate.direction;
                newRes.res = gate.res.type;
                saveData.Add(newRes);
            }
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Deposit"))
        {
            TileData tile = saveData.Find((x) => x.posX == obj.GetComponent<Tile>().x && x.posY == obj.GetComponent<Tile>().y);
            if (tile != null)
            {
                tile.deposit = obj.GetComponent<Tile>().deposit;
            }
            else
            {
                TileData newTile = new TileData();
                newTile.posX = obj.GetComponent<Tile>().x;
                newTile.posY = obj.GetComponent<Tile>().y;
                newTile.deposit = obj.GetComponent<Tile>().deposit;
                saveData.Add(newTile);
            }
        }
    }

    public void Restore()
    {
        if (saveData.Count()>0)
        {
            foreach (TileData td in saveData.Where((x)=>x.deposit.Length>0))
            {
                grid[td.posX, td.posY].GetComponent<Tile>().NewDeposit(td.deposit);
            }
            foreach(TileData td in saveData.Where((x)=>x.content.Length>0))
            {
                grid[td.posX, td.posY].GetComponent<Tile>().NewMachine(td.content, td.rotation, restoring: true);
                grid[td.posX, td.posY].GetComponent<Tile>().machine.GetComponent<Machine>().turnedOff = td.turnedOff;
                if (td.activeRecipe > 0)
                {
                    grid[td.posX, td.posY].GetComponent<Tile>().machine.GetComponent<Manufactory>().SetRecipes();
                    grid[td.posX, td.posY].GetComponent<Tile>().machine.GetComponent<Manufactory>().currentRecipe = grid[td.posX, td.posY].GetComponent<Tile>().machine.GetComponent<Manufactory>().availableRecipes[td.activeRecipe];
                }
            }
            foreach (TileData td in saveData.Where((x) => x.res.Length > 0))
            {
                var foo = grid[td.posX, td.posY].GetComponent<Tile>().machine.GetComponent<Machine>().parts.Find((x) => x.GetComponent<MachinePart>().tile == grid[td.posX, td.posY].GetComponent<Tile>()).GetComponents<Gate>().ToList();

                Gate targetGate = grid[td.posX, td.posY].GetComponent<Tile>().machine.GetComponent<Machine>().parts.Find((x) => x.GetComponent<MachinePart>().tile == grid[td.posX, td.posY].GetComponent<Tile>()).GetComponents<Gate>().ToList().Find((x) => x.direction == td.rotation);
                int targetOrder = targetGate.GetComponent<SpriteRenderer>().sortingOrder + targetGate.owner.SpriteOrderDirection((targetGate.DirectionRotated() + 2) % 4, targetGate.DirectionRotated());
                targetGate.res = Globals.GetSave().GetResources().CreateFlowing(td.res, 1, targetOrder, targetGate.GetPosition());
            }
        }
        saveData = null;
    }

    public void Save()
    {
        float startedAt = Time.realtimeSinceStartup;
        PrepareForSaving();
        try
        {
            Directory.CreateDirectory(locationPath);
            string saveAsJson = JsonUtility.ToJson(this);
            if (saveAsJson.Length == 0)
            {
                return;
            }

            FileStream file;

            file = File.Create(locationPath + saveName + saveExtension);

            StreamWriter writer = new StreamWriter(file);
            writer.Write(saveAsJson);
            writer.Close();
            file.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return;
        }

        Debug.Log("Saved in " + (Time.realtimeSinceStartup - startedAt)+"s");
    }

    public bool Load()
    {
        if (File.Exists(locationPath+saveName+saveExtension))
        {
            StreamReader reader = new StreamReader(locationPath + saveName + saveExtension);
            try
            {
                SaveFile save = JsonUtility.FromJson<SaveFile>(reader.ReadToEnd());
                reader.Close();
                saveData = save.saveData;
                res = save.res;
                gameStats = save.gameStats;
                logisticTech = save.logisticTech;
                industrialTech = save.industrialTech;
                scientificTech = save.scientificTech;
                unlocks = save.unlocks;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return false;
            }
            finally
            {
                reader.Close();
            }
            return true;
        }
        else return false;
    }
}
