using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manufactory : Machine
{
    private Gate gate_out;
    private List<Gate> gates_in;

    private List<FlowingResource> store;

    public Recipe currentRecipe;
    public List<Recipe> availableRecipes;

    public class Recipe
    {
        public class Pair
        {
            public string material;
            public int amount;
            public Pair(string material, int amount)
            {
                this.material = material;
                this.amount = amount;
            }
        }
        public List<Pair> mat_in = new List<Pair>();
        public Pair mat_out;
    }

    void Start()
    {
        store = new List<FlowingResource>();
        availableRecipes = new List<Recipe>();

        SetRecipes();

        currentRecipe = availableRecipes.First();
    }

    protected virtual void SetRecipes()
    {
        //Gear
        Recipe newRecipe = new Recipe();
        newRecipe.mat_out = new Recipe.Pair("Gear", 1);
        newRecipe.mat_in.Add(new Recipe.Pair("Metal", 1));
        availableRecipes.Add(newRecipe);

        //Wire
        newRecipe = new Recipe();
        newRecipe.mat_out = new Recipe.Pair("Wire", 1);
        newRecipe.mat_in.Add(new Recipe.Pair("Metal", 1));
        availableRecipes.Add(newRecipe);

        //Mechanism
        newRecipe = new Recipe();
        newRecipe.mat_out = new Recipe.Pair("Mechanism", 1);
        newRecipe.mat_in.Add(new Recipe.Pair("Gear", 1));
        newRecipe.mat_in.Add(new Recipe.Pair("Metal", 1));
        availableRecipes.Add(newRecipe);

        //PCB
        newRecipe = new Recipe();
        newRecipe.mat_out = new Recipe.Pair("PCB", 1);
        newRecipe.mat_in.Add(new Recipe.Pair("Wire", 1));
        newRecipe.mat_in.Add(new Recipe.Pair("Metal", 1));
        availableRecipes.Add(newRecipe);
    }

    private void FindGates()
    {
        if (!gate_out)
        {
            gates_in = new List<Gate>();
            foreach(Gate gate in GetGates())
            {
                if (!gate.outputing)
                {
                    gates_in.Add(gate);
                }
                else
                {
                    gate_out = gate;
                }
            }
        }
    }

    public override bool CanActivate()
    {
        FindGates();
        return base.CanActivate() && !gate_out.IsOccupied() && currentRecipe!=null && store.Count()==currentRecipe.mat_in.Count();
    }

    public override void Activate()
    {
        if (CanActivate())
        {
            base.Activate();
            store.ForEach((x) => x.Dispose());
            store.Clear();
        }

        foreach (Gate gate in gates_in)
        {
            if (gate.res)
            {
                if(currentRecipe.mat_in.Find((x)=>x.material.Equals(gate.res.type))!=null && store.Where((x) => x.type.Equals(gate.res.type)).Count() == 0){
                    store.Add(gate.res);
                    gate.res.Fade(false);
                    gate.res = null;
                }
            }
        }
    }

    public override void EndActivation()
    {
        if (CanEndActivation())
        {
            base.EndActivation();
            gate_out.res = Globals.GetSave().GetResources().CreateFlowing(currentRecipe.mat_out.material, currentRecipe.mat_out.amount, gate_out.GetComponent<SpriteRenderer>().sortingOrder + SpriteOrderDirection((gate_out.DirectionRotated() + 2) % 4, gate_out.DirectionRotated()), gate_out.GetComponent<MachinePart>().transform.position);
            gate_out.res.Teleport(gate_out, 0);
            gate_out.res.Appear();
            Globals.LogStat("Resources processed", 1);

        }
        activationTimer--;
    }
}
