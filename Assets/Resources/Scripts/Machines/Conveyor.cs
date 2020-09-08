using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : Machine
{
    private Gate gate_in;
    private Gate gate_out;

    private void FindGates()
    {
        gate_in = Array.Find(parts[0].GetComponents<Gate>(), (x) => !x.outputing);
        gate_out = Array.Find(parts[0].GetComponents<Gate>(), (x) => x.outputing);
    }

    public override bool CanActivate()
    {
        if (gate_in == null)
        {
            FindGates();
        }

        return base.CanActivate() && (!gate_out.res && gate_in.res && !gate_out.IsOccupied());
    }

    public override void Activate()
    {
        EnableAnimations();
        if (CanActivate())
        {

            gate_in.res.ThreePointMove(gate_out.GetLink(), parts[0].transform.position);

            gate_out.res = gate_in.res;
            gate_in.res = null;
        }
    }

    public override void EndActivation()
    {
        activationTimer--;
    }
}
