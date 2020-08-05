using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UpgraderMachine : Machine
{
    public override void Activate()
    {
        //Gate gate_in = Array.Find(GetComponents<Gate>(), (x) => !x.outputing);
        //Gate gate_out = Array.Find(GetComponents<Gate>(), (x) => x.outputing);

        //if (gate_in.GetLink() && gate_out.GetLink())
        //{
        //    //move in
        //    store = gate_in.res;
        //    gate_in.res = null;

        //    //upgrade
        //    if (store)
        //    {
        //        store.type = "betterium";
        //        store.Refresh();
        //        store.Teleport(gate_out.GetLink().owner.owner, gate_out.GetPosition(), store.secondsPerTile);

        //    }

        //    //move out
        //    gate_out.res = store;
        //    store = null;
        //}
    }
}
