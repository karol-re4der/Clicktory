using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergerMachine : Machine
{
    public bool evenTick = false;

    private Gate gate_out;
    private Gate[] gates_in;

    private void FindGates()
    {
        if (!gate_out)
        {
            gate_out = GetGates().Find((x) => x.outputing);
            gates_in = GetGates().Where((x) => !x.outputing).ToArray();
        }
    }

    public override bool CanActivate()
    {
        if (gate_out == null)
        {
            FindGates();
        }

        return base.CanActivate() && gate_out.GetLink() && gates_in.Length > 0;
    }


    public override void Activate()
    {
        EnableAnimations();
        if (CanActivate())
        {
            Gate gate_in;

            if ((gates_in[0].res && evenTick) || (!gates_in[1].res && gates_in[0].res))
            {
                gate_in = gates_in[0];
            }
            else if (gates_in[1].res)
            {
                gate_in = gates_in[1];
            }
            else
            {
                return;
            }
            store = gate_in.res;
            gate_in.res = null;
            store.ThreePointMove(gate_out, parts[0].transform.position);
            gate_out.res = store;
            store = null;
            evenTick = !evenTick;
        }
    }

    public override void EndActivation()
    {
        activationTimer--;
    }
}
