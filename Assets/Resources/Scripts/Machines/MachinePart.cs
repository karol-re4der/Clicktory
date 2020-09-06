using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachinePart : MonoBehaviour
{
    public Sprite[] rotations_one;
    public Sprite[] rotations_two;
    public Sprite[] rotations_three;
    public Tile tile;
    public bool smoking = false;
    public int rotation = 0;
    public int animationStage = 0;
    public bool animating = false;

    public void RefreshSprite(int rotation)
    {
        this.rotation = rotation;
        GetComponent<SpriteRenderer>().sprite = rotations_one[rotation];
    }

    public void OnMouseUp()
    {
        tile.machine.GetComponent<Machine>().OnMouseUp();
    }

    public void LaunchSmoke()
    {
        if (smoking)
        {
            InvokeRepeating("Invoke_Smoking", 0, 0.1f);
        }
    }
    public void StopSmoke()
    {
        CancelInvoke();
    }

    public void Animate()
    {
        if (animating)
        {
            animationStage++;
            animationStage %= 3;


            switch (animationStage)
            {
                case 0:
                    GetComponent<SpriteRenderer>().sprite = rotations_one[rotation];
                    break;
                case 1:
                    GetComponent<SpriteRenderer>().sprite = rotations_two[rotation];
                    break;
                case 2:
                    GetComponent<SpriteRenderer>().sprite = rotations_three[rotation];
                    break;
            }
        }
    }

    private void Invoke_Smoking()
    {
        Instantiate(Resources.Load("Prefabs/Smoke") as GameObject, GameObject.Find("Map/Smoke").transform).GetComponent<Smoke>().Launch(GetComponent<SpriteRenderer>().sortingOrder, transform.position);
    }
}
