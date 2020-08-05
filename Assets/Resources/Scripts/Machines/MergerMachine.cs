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

    public override void Activate()
    {
        FindGates();
        if (gate_out.GetLink())
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
            store.Teleport(gate_out, store.secondsPerTile);
            gate_out.res = store;
            store = null;
            evenTick = !evenTick;
        }
    }
}
