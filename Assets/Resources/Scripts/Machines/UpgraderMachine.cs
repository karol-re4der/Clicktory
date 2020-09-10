using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UpgraderMachine : Machine
{
    public FlowingResource oreStore;
    public FlowingResource coalStore;

    private Gate gate_out;
    private List<Gate> gates_in;
    private void FindGates()
    {
        if (!gate_out)
        {
            gate_out = GetGates().Find((x) => x.outputing);
            gates_in = GetGates().Where((x) => !x.outputing).ToList();
        }
    }

    public override bool CanActivate()
    {
        return base.CanActivate() && gate_out && oreStore && coalStore;
    }

    public override void Activate()
    {
        FindGates();

        if (CanActivate()) 
        { 
            if(oreStore && coalStore)
            {
                base.Activate();
                oreStore.Dispose();
                coalStore.Dispose();
            }
            
        }

        foreach (Gate gate in gates_in)
        {
            if (gate.res)
            {
                if (!oreStore && gate.res.type.Equals("Ore"))
                {
                    oreStore = gate.res;
                    oreStore.Fade(false);
                    gate.res = null;
                }
                else if (!coalStore && gate.res.type.Equals("Coal"))
                {
                    coalStore = gate.res;
                    coalStore.Fade(false);
                    gate.res = null;
                }
            }
        }
    }

    public override void EndActivation()
    {
        if (CanEndActivation())
        {
            base.EndActivation();
            gate_out.res = Globals.GetSave().GetResources().CreateFlowing("Iron", oreStore.amount, gate_out.GetComponent<SpriteRenderer>().sortingOrder + SpriteOrderDirection((gate_out.DirectionRotated() + 2) % 4, gate_out.DirectionRotated()), gate_out.GetComponent<MachinePart>().transform.position);
            gate_out.res.Teleport(gate_out, 0);
            gate_out.res.Appear();
        }
        activationTimer--;
    }
}
