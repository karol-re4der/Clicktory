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

    public bool CanAfford(List<KeyValuePair<string, int>> cost)
    {
        foreach (KeyValuePair<string, int> pair in cost)
        {
            if (!CanAfford(pair.Key, pair.Value))
            {
                return false;
            }
        }
        return true;
    }
    public bool CanAfford(string costType, int costValue)
    {
        if (PlayerPrefs.GetInt("Godmode", 0) == 1)
        {
            return true;
        }
        return costValue < GetRes(costType);
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

        if (PlayerPrefs.GetInt("Godmode", 0) == 0)
        {
            res.Find((x) => x.type.Equals(type)).amount -= amount;
        }
    }
    public void RemoveRes(List<KeyValuePair<string, int>> toRemove)
    {
        foreach (KeyValuePair<string, int> pair in toRemove)
        {
            RemoveRes(pair.Key, pair.Value);
        }
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

    public Sprite FindResSprite(string type)
    {
        int spriteNumber = 3;
        switch (type)
        {
            case "Coal":
                spriteNumber = 1;
                break;
            case "Iron":
                spriteNumber = 2;
                break;
            case "Ore":
                spriteNumber = 0;
                break;
            case "Science":
                spriteNumber = 4;
                break;
        }
        return Resources.LoadAll<Sprite>("Textures/Resources/Resource_Spritesheet/")[spriteNumber];
    }
    public FlowingResource CreateFlowing(string type, int amount, int order, Vector3 position)
    {
        FlowingResource newRes = GameObject.Instantiate(Resources.Load("Prefabs/Items/Resource") as GameObject, GameObject.Find("Map/Resources").transform).GetComponent<FlowingResource>();
        newRes.transform.position = position;
        newRes.amount = amount;
        newRes.type = type;
        newRes.GetComponent<SpriteRenderer>().sortingOrder = order;
        newRes.Refresh();
        return newRes;
    }
}
