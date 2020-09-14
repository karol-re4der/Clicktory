using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpliterMachine : Machine
{
    private bool evenTick = false;

    private Gate gate_in;
    private Gate[] gates_out;

    private void FindGates()
    {
        if (!gate_in)
        {
            gate_in = GetGates().Find((x) => !x.outputing);
            gates_out = GetGates().Where((x) => x.outputing).ToArray();
        }
    }

    public override bool CanActivate()
    {
        if (gate_in == null)
        {
            FindGates();
        }

        return base.CanActivate() && gate_in.res && (!gates_out.First().IsOccupied() || !gates_out.Last().IsOccupied());
    }

    public override void Activate()
    {
        EnableAnimations();
        if (CanActivate())
        {
            Gate gate_out;
            if (evenTick && !gates_out[0].IsOccupied())
            {
                gate_out = gates_out[0];
            }
            else
            {
                gate_out = gates_out[1];
            }

            store = gate_in.res;
            gate_in.res = null;
            store.ThreePointMove(gate_out, parts[0].transform.position);
            gate_out.res = store;
            store = null;
            evenTick = !evenTick;
        }
    }

    public override bool EndsFlow()
    {
        return true;
    }

    public override void EndActivation()
    {
        activationTimer--;
    }
}
