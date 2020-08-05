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
}
