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

    public override void Activate()
    {
        FindGates();
        if (gate_in.res)
        {
            Gate gate_out;
            if (evenTick)
            {
                gate_out = gates_out[0];
            }
            else
            {
                gate_out = gates_out[1];
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
