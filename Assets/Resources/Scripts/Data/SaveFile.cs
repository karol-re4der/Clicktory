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
    }

    private GameObject[,] grid;

    [SerializeField]
    private ResourceStore res;
    [SerializeField]
    public List<TileData> saveData;

    private string locationPath;
    private string saveExtension = ".save";
    private string saveName = "save";

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
            saveData.Add(newTile);
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
            foreach (TileData td in saveData)
            {
                if (td.deposit.Length>0)
                {
                    grid[td.posX, td.posY].GetComponent<Tile>().NewDeposit(td.deposit);

                }
                if (td.content.Length > 0)
                {
                    grid[td.posX, td.posY].GetComponent<Tile>().NewMachine(td.content, td.rotation);
                }
            }
        }
        saveData = null;
    }

    public void Save()
    {
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
