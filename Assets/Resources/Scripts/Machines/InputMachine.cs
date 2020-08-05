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
    public override void Activate()
    {
        FindGates();
        if (gate_in.res)
        {
            store = gate_in.res;
            gate_in.res = null;
            store.Store();
        }
    }
}
