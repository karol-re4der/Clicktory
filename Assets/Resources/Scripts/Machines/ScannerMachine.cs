using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerMachine : Machine
{
    private Gate gate_out;
    private Gate gate_in;
    private void FindGates()
    {
        if (!gate_out)
        {
            gate_out = GetGates().Find((x) => x.outputing);
            gate_in = GetGates().Find((x) => !x.outputing);
        }
    }

    public override void Activate()
    {
        FindGates();
        if(gate_in.GetLink() && gate_out.GetLink()){
            if (store)
            {
                gate_out.res = Globals.GetSave().GetResources().CreateFlowing("Science", store.amount, gate_out.GetComponent<SpriteRenderer>().sortingOrder + SpriteOrderDirection((gate_out.DirectionRotated() + 2) % 4, gate_out.DirectionRotated()), gate_out.GetComponent<MachinePart>().transform.position);
                gate_out.res.Teleport(gate_out, gate_out.res.secondsPerTile);
                store.Dispose();
            }
            if (gate_in.res)
            {
                store = gate_in.res;
                gate_in.res = null;
            }
        }
    }
}
