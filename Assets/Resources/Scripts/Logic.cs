using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Logic : MonoBehaviour
{
    public SaveFile save;

    public TechTree industrialTech;
    public TechTree scientificTech;
    public TechTree logisticTech;

    [System.Serializable]
    public class TechTree
    {
        [System.Serializable]
        public class Tech
        {
            [System.Serializable]
            public class Pair
            {
                public string res;
                public int amount;
            }
            public string name;
            public string desc;
            public List<Pair> cost;

            public List<KeyValuePair<string, int>> GetCost()
            {
                List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
                foreach (Pair pair in cost)
                {
                    list.Add(new KeyValuePair<string, int>(pair.res, pair.amount));
                }
                return list;
            }
        }
        public List<Tech> techTree;
    }
    void Start()
    {
        //Load techtree
        StreamReader reader = new StreamReader("Assets/Resources/Techs/industrialTech.json");
        industrialTech = JsonUtility.FromJson<TechTree>(reader.ReadToEnd());
        reader = new StreamReader("Assets/Resources/Techs/logisticTech.json");
        logisticTech = JsonUtility.FromJson<TechTree>(reader.ReadToEnd());
        reader = new StreamReader("Assets/Resources/Techs/scientificTech.json");
        scientificTech = JsonUtility.FromJson<TechTree>(reader.ReadToEnd());
        reader.Close();

        //Load starting world state
        save = new SaveFile();
        GetComponent<Builder>().SetStartingState();
        if (!save.Load())
        {
            GetComponent<Builder>().GenerateDeposits();
        }
        if (save.saveData!=null && save.saveData.Count()>0)
        {
            save.Restore();
        }

        if (GameObject.FindGameObjectsWithTag("Machine").ToList().Find((x)=>x.GetComponent<Machine>().type.Equals("Storage")))
        {
            Camera.main.GetComponent<CameraHandler>().CenterCamera(GameObject.FindGameObjectsWithTag("Machine").ToList().Find((x) => x.GetComponent<Machine>().type.Equals("Storage")).GetComponent<Machine>().GetCenterPoint());
        }
        else
        {
            Camera.main.GetComponent<CameraHandler>().CenterCamera(save.GetGrid()[save.GetGrid().GetLength(0) / 2, save.GetGrid().GetLength(1) / 2].transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Tick()
    {
        if (GameObject.FindWithTag("Machine"))
        {
            foreach (GameObject machine in GameObject.FindGameObjectsWithTag("Machine"))
            {
                machine.GetComponent<Machine>().InOut();
            }
            foreach (GameObject machine in GameObject.FindGameObjectsWithTag("Machine"))
            {
                machine.GetComponent<Machine>().Activate();
            }
        }

        if (PlayerPrefs.GetInt("Autosave") == 1)
        {
            save.Save();
        }
    }

    public void TechUp(string techName)
    {
        TechTree.Tech tech = null;

        switch (techName)
        {
            case "Industry":
                tech = industrialTech.techTree[save.industrialTech];
                break;
            case "Science":
                tech = scientificTech.techTree[save.scientificTech];
                break;
            case "Logistics":
                tech = logisticTech.techTree[save.logisticTech];
                break;
        }

        if (Globals.GetSave().GetResources().CanAfford(tech.GetCost()))
        {
            Globals.GetSave().GetResources().RemoveRes(tech.GetCost());
            switch (techName)
            {
                case "Industry":
                    save.industrialTech++;
                    break;
                case "Science":
                    save.scientificTech++;
                    break;
                case "Logistics":
                    save.logisticTech++;
                    break;
            }
            Globals.GetInterface().Button_Research();
        }
    }
}
