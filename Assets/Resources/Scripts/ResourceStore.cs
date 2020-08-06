using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class ResourceStore
{
    [System.Serializable]
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
    [SerializeField]
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
    public void Reset()
    {
        foreach(Transform trans in GameObject.Find("Canvas/Top Bar/Resources/").transform)
        {
            GameObject.Destroy(trans.gameObject);
        }
        res.Clear();
    }
    public int GetRes(string type)
    {
        NewRes(type);
        return res.Find((x) => x.type.Equals(type)).amount;
    }
}
