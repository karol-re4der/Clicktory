using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HierarchySort : MonoBehaviour
{
    public void SortByName()
    {
        for(int i = 1; i<transform.childCount; i++)
        {
            if (transform.GetChild(i - 1).name.CompareTo(transform.GetChild(i).name)>0)
            {
                transform.GetChild(i).SetSiblingIndex(i - 1);
                i = 1;
                continue;
            }
        }
    }
    public void SortByButtonBeingInteractable()
    {
        List<Transform> interactable = new List<Transform>();
        List<Transform> notInteractable = new List<Transform>();


        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.GetComponent<Button>().interactable)
            {
                interactable.Add(transform.GetChild(i));
            }
            else
            {
                notInteractable.Add(transform.GetChild(i));
            }
        }

        int totalIndex = 0;
        for (int i = 0; i < interactable.Count(); i++)
        {
            interactable[i].SetSiblingIndex(totalIndex);
            totalIndex++;
        }
        for (int i = 0; i < notInteractable.Count(); i++)
        {
            notInteractable[i].SetSiblingIndex(totalIndex);
            totalIndex++;
        }
    }
}
