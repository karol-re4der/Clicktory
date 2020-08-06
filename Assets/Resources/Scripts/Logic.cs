using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    public SaveFile save;

    // Start is called before the first frame update
    void Start()
    {

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

    
}
