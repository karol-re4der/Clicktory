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

    public override void Activate()
    {
        //if (!IsInvoking("Invoke_Activate"))
        //{
            foreach (GameObject part in parts)
            {
                part.GetComponent<MachinePart>().animationStage = 0;
                part.GetComponent<MachinePart>().animating = true;
            }
            Invoke("Invoke_Activate", activationTime);
        //}

        if (gate_in == null)
        {
            FindGates();
        }

        if (!gate_out.res && gate_in.res && !gate_out.IsOccupied())
        {
            if (gate_out.GetLink())
            {
                gate_in.res.Move(gate_out.GetLink(), false);
            }
            else
            {
                gate_in.res.Move(gate_out, true);
            }
            gate_out.res = gate_in.res;
            gate_in.res = null;
        }
    }
    public void Invoke_Activate()
    {
        foreach (GameObject part in parts)
        {
            part.GetComponent<MachinePart>().animating = false;
        }
    }
}
