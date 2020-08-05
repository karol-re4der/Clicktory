using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStore
{
    public class Res
    {
        public string type;
        public int amount;

        public Res(string type)
        {
            this.type = type;
            amount = 0;
        }
    }

    public List<Res> res;

    public ResourceStore()
    {
        res = new List<Res>();
    }

    public void NewRes(string type)
    {
        if (res.Where((x) => x.type.Equals(type)).Count()==0)
        {
            res.Add(new Res(type));
        }
    }
    public void AddRes(string type, int amount)
    {
        NewRes(type);
        res.Find((x) => x.type.Equals(type)).amount += amount;
    }
    public void RemoveRes(string type, int amount)
    {
        NewRes(type);
        res.Find((x) => x.type.Equals(type)).amount -= amount;

    }
    public int GetRes(string type)
    {
        NewRes(type);
        return res.Find((x) => x.type.Equals(type)).amount;
    }
}
