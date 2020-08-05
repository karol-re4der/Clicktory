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
        public string content;
    }

    private GameObject[,] grid;
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
    public ResourceStore GetResources()
    {
        return res;
    }

    public SaveFile()
    {
        locationPath = Application.persistentDataPath + "/Saves/";
        res = new ResourceStore();
    }

    public void PrepareForSaving()
    {
        saveData = new List<TileData>();
        for(int x = 0; x<grid.GetLength(0); x++)
        {
            for(int y = 0; y<grid.GetLength(1); y++)
            {
                if (grid[x, y].GetComponent<Tile>() && grid[x, y].GetComponent<Tile>().machine)
                {
                    TileData newTile = new TileData();
                    newTile.posX = x;
                    newTile.posY = y;
                    newTile.rotation = grid[x, y].GetComponent<Tile>().machine.GetComponent<Machine>().rotation;
                    newTile.content = grid[x, y].GetComponent<Tile>().machine.GetComponent<Machine>().type;
                    saveData.Add(newTile);
                }
            }
        }
    }

    public void Restore()
    {
        if (saveData.Count()>0)
        {
            foreach (TileData td in saveData)
            {
                grid[td.posX, td.posY].GetComponent<Tile>().NewMachine(td.content, td.rotation);
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
            //MapTools.GetSave().Prepare();
            string saveAsJson = JsonUtility.ToJson(this);
            if (saveAsJson.Length == 0)
            {
                return;
            }

            //Debug.Log(saveAsJson);
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

    public void Load()
    {
        if (File.Exists(locationPath+saveName+saveExtension))
        {
            StreamReader reader = new StreamReader(locationPath + saveName + saveExtension);
            try
            {
                SaveFile save = JsonUtility.FromJson<SaveFile>(reader.ReadToEnd());
                reader.Close();
                saveData = save.saveData;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            finally
            {
                reader.Close();
            }
        }
        else return;
    }
}
