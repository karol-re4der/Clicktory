using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class Stats
{
    [System.Serializable]
    public class Stat
    {
        public string name;
        public float value;

        public Stat(string name, float value)
        {
            this.name = name;
            this.value = value;
        }
    }

    [SerializeField]
    public List<Stat> stats = new List<Stat>();

    public void LogStat(string name, float value)
    {
        Stat stat = null;
        if (stats.Find((x) => x.name.Equals(name))!=null)
        {
            stat = stats.Find((x) => x.name.Equals(name));
        }
        else
        {
            stat = new Stat(name, value);
            stats.Add(stat);
        }

        stat.value += value;
    }
}
