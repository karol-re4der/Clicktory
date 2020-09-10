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

    public override bool CanActivate()
    {
        FindGates();
        return base.CanActivate() && gate_in.res && !store;
    }

    public override void Activate()
    {
        if(CanActivate()){
            base.Activate();

            store = gate_in.res;
            store.Fade(false);
            gate_in.res = null;
        }
    }

    public override void EndActivation()
    {
        if (CanEndActivation())
        {
            base.EndActivation();

            gate_out.res = Globals.GetSave().GetResources().CreateFlowing("Science", store.amount, gate_out.GetComponent<SpriteRenderer>().sortingOrder + SpriteOrderDirection((gate_out.DirectionRotated() + 2) % 4, gate_out.DirectionRotated()), gate_out.GetComponent<MachinePart>().transform.position);
            gate_out.res.Teleport(gate_out, 0);
            gate_out.res.Appear();
            store.Dispose();
        }
        activationTimer--;
    }
}
