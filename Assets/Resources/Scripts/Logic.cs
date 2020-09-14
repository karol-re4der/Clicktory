using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Logic : MonoBehaviour
{
    public SaveFile save;
    public Flow flow;

    public TechTree industrialTech;
    public TechTree scientificTech;
    public TechTree logisticTech;

    public Vector2 windDir;
    public float windPower;

    private float lastAnimationFrame = 0;
    private float lastTick = 0;
    private int animationTimer = 0;
    public int animationLength = 3;
    public float autoTick = 1f/3;

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
        flow = new Flow();


        //Load techtree
        industrialTech = JsonUtility.FromJson<TechTree>(Resources.Load<TextAsset>("Techs/industrialTech").ToString());
        logisticTech = JsonUtility.FromJson<TechTree>(Resources.Load<TextAsset>("Techs/logisticTech").ToString());
        scientificTech = JsonUtility.FromJson<TechTree>(Resources.Load<TextAsset>("Techs/scientificTech").ToString());

        //Load starting world state
        save = new SaveFile();
        GetComponent<Builder>().SetStartingState();
        if (!save.Load())
        {
            GetComponent<Builder>().GenerateDeposits();
        }
        if (save.saveData != null && save.saveData.Count() > 0)
        {
            save.Restore();
        }

        if (GameObject.FindGameObjectsWithTag("Machine").ToList().Find((x) => x.GetComponent<Machine>().type.Equals("Storage")))
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
        Globals.GetInterface().CheckUnlocks();

        if(Time.time - lastTick > autoTick)
        {
            lastTick = Time.time;

            foreach (GameObject mach in GameObject.FindGameObjectsWithTag("Machine"))
            {
                mach.GetComponent<Machine>().Animate(animationTimer);
            }
            animationTimer++;
            animationTimer %= animationLength;

            if (animationTimer == 0)
            {
                Tick();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            flow.HighlightFlow();
        }

    }
    public void Tick()
    {
        if (GameObject.FindWithTag("Machine"))
        {

            flow.RefreshFlow();
            foreach (Flow.FlowGroup group in flow.groupOrder)
            {
                foreach (Machine machine in group.machineOrder)
                {
                    machine.GetComponent<Machine>().InOut();
                }
            }
            foreach (Flow.FlowGroup group in flow.groupOrder)
            {
                foreach (Machine machine in group.machineOrder)
                {
                    machine.GetComponent<Machine>().EndActivation();
                }
            }
            foreach (Flow.FlowGroup group in flow.groupOrder)
            {
                foreach (Machine machine in group.machineOrder)
                {
                    machine.GetComponent<Machine>().Activate();
                }
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
