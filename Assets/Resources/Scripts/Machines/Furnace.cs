using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : Manufactory
{
    public override void SetRecipes()
    {
        availableRecipes.Add(new Recipe());
        availableRecipes.First().mat_out = new Recipe.Pair("Metal", 1);
        availableRecipes.First().mat_in.Add(new Recipe.Pair("Ore", 1));
        availableRecipes.First().mat_in.Add(new Recipe.Pair("Coal", 1));
    }
}
