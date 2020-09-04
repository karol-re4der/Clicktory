using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachinePart : MonoBehaviour
{
    public Sprite[] rotations;
    public Tile tile;

    public void RefreshSprite(int rotation)
    {
        GetComponent<SpriteRenderer>().sprite = rotations[rotation];
    }

    public void OnMouseUp()
    {
        tile.machine.GetComponent<Machine>().OnMouseUp();
    }

    public void LaunchSmoke()
    {
        InvokeRepeating("Invoke_Smoking", 0, 0.1f);
    }
    public void StopSmoke()
    {
        CancelInvoke();
    }

    private void Invoke_Smoking()
    {
        Instantiate(Resources.Load("Prefabs/Smoke") as GameObject, GameObject.Find("Map/Smoke").transform).GetComponent<Smoke>().Launch(GetComponent<SpriteRenderer>().sortingOrder, transform.position);
    }
}
