using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flow : MonoBehaviour
{
    public bool flowModified = true;

    public class FlowGroup{
        public List<Machine> machineOrder = new List<Machine>();

        public void RefreshGroup()
        {
            while(machineOrder.Last().GetGates().Where((x)=>!x.outputing).Count()==1 && machineOrder.Last().GetGates().Find((x) => !x.outputing).GetLink())
            {
                Machine nextInLine = machineOrder.Last().GetGates().Find((x) => !x.outputing).GetLink().owner;
                if (nextInLine.GetGates().Where((x) => !x.outputing).Count() > 1)
                {
                    machineOrder.Add(nextInLine);
                    break;
                }
                else if (nextInLine.GetGates().Where((x) => x.outputing).Count() > 1)
                {
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
        Color col = Color.green;
        foreach(FlowGroup group in groupOrder)
        {
            col = Color.Lerp(col, Color.red, (float)i / groupOrder.Count());
            i++;

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
        if (flowModified)
        {
            Debug.Log("Reflowed");
            groupOrder = new List<FlowGroup>();

            foreach (GameObject machine in GameObject.FindGameObjectsWithTag("Machine"))
            {
                if (!machine.GetComponent<Machine>().EndsFlow())
                {
                    continue;
                }

                groupOrder.Add(new FlowGroup());
                groupOrder.Last().machineOrder.Add(machine.GetComponent<Machine>());
            }

            foreach (FlowGroup group in groupOrder)
            {
                group.RefreshGroup();
            }
            flowModified = false;
        }
    }
}
