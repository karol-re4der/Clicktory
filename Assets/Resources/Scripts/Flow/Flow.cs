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
                Machine nextInLine = machineOrder.Last().GetGates().Find((x) => !x.outputing).GetLink().owner;
                if (machineOrder.Last().BreaksFlow())
                {
                    if (nextInLine.GetGates().Where((x) => x.outputing).Count() > 1)
                    {
                        machineOrder.Add(nextInLine);
                    }
                    break;
                }
                else
                {
                    machineOrder.Add(nextInLine);
                }
            }
        }
    }

    public List<FlowGroup> groupOrder = new List<FlowGroup>();

    public void HighlightFlow()
    {
        int i = 0;
        Color col = Color.white;
        foreach(FlowGroup group in groupOrder)
        {
            i++;
            col = Color.Lerp(col, Color.black, (float)i / groupOrder.Count());

            foreach (Machine machine in group.machineOrder)
            {
                foreach (GameObject part in machine.parts)
                {
                    part.GetComponent<MachinePart>().tile.Highlight(col);
                }
            }
        }
    }

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
