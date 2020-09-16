using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMachine : Machine
{
    private Gate gate_in;
    private void FindGates()
    {
        if (!gate_in)
        {
            gate_in = parts.Find((x) => x.GetComponent<Gate>()).GetComponent<Gate>();
        }
    }

    public override bool CanActivate()
    {
        if (gate_in == null)
        {
            FindGates();
        }

        return base.CanActivate() && gate_in.res;
    }

    public override void Activate()
    {
        EnableAnimations();
        if (CanActivate())
        {
            Globals.LogStat("Resources stored", 1);
            store = gate_in.res;
            gate_in.res = null;
            store.Store();
            store.Fade(true);
        }
    }

    public override void EndActivation()
    {
        activationTimer--;
    }
}
