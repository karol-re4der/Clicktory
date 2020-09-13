using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flow : MonoBehaviour
{
    public class FlowGroup{
        public List<Machine> machineOrder = new List<Machine>();

        public void RefreshGroup()
        {
            while(machineOrder.Last().GetGates().Where((x)=>!x.outputing).Count()==1 && machineOrder.Last().GetGates().Find((x) => !x.outputing).GetLink())
            {
                machineOrder.Add(machineOrder.Last().GetGates().Find((x) => !x.outputing).GetLink().owner);
            }
        }
    }

    public List<FlowGroup> groupOrder = new List<FlowGroup>();

    public void RefreshFlow()
    {
        groupOrder = new List<FlowGroup>();

        foreach(GameObject machine in GameObject.FindGameObjectsWithTag("Machine"))
        {
            if (!machine.GetComponent<Machine>().EndsFlow())
            {
                continue;
            }

            groupOrder.Add(new FlowGroup());
            groupOrder.Last().machineOrder.Add(machine.GetComponent<Machine>());
        }

        foreach(FlowGroup group in groupOrder)
        {
            group.RefreshGroup();
        }
    }
}
