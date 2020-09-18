using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manufactory : Machine
{
    private Gate gate_out;
    private List<Gate> gates_in;

    private FlowingResource store_first;
    private FlowingResource store_second;

    public string material_first;
    public string material_second;
    public string material_result;

    private void FindGates()
    {
        if (!gate_out)
        {
            gates_in = new List<Gate>();
            foreach(Gate gate in GetGates())
            {
                if (!gate.outputing)
                {
                    gates_in.Add(gate);
                }
                else
                {
                    gate_out = gate;
                }
            }
        }
    }

    public override bool CanActivate()
    {
        FindGates();
        return base.CanActivate() && !gate_out.IsOccupied() && store_first && store_second;
    }

    public override void Activate()
    {
        if (CanActivate())
        {
            if (store_second && store_first)
            {
                base.Activate();
                store_first.Dispose();
                store_second.Dispose();
            }

        }

        foreach (Gate gate in gates_in)
        {
            if (gate.res)
            {
                if (!store_first && gate.res.type.Equals(material_first))
                {
                    store_first = gate.res;
                    store_first.Fade(false);
                    gate.res = null;
                }
                else if (!store_second && gate.res.type.Equals(material_second))
                {
                    store_second = gate.res;
                    store_second.Fade(false);
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
            gate_out.res = Globals.GetSave().GetResources().CreateFlowing(material_result, store_first.amount, gate_out.GetComponent<SpriteRenderer>().sortingOrder + SpriteOrderDirection((gate_out.DirectionRotated() + 2) % 4, gate_out.DirectionRotated()), gate_out.GetComponent<MachinePart>().transform.position);
            gate_out.res.Teleport(gate_out, 0);
            gate_out.res.Appear();
            Globals.LogStat("Resources processed", 1);

        }
        activationTimer--;
    }
}
