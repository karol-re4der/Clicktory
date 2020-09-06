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
    public bool animationSynchronized = false;
    public bool animationBoomerang = false;
    public bool forceAnimation = false;
    public int animationSpeed = 1;

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

    public void Animate(int timer)
    {
        if (animating || forceAnimation)
        {
            if (animationSynchronized)
            {
                animationStage = timer;
            }
            else if(animationSpeed==1)
            {
                animationStage++;
                animationStage %= animationBoomerang?4:3;
            }
            else
            {
                animationStage++;
                animationStage %= animationBoomerang?4 * animationSpeed:3*animationSpeed;
            }

            if (animationStage < animationSpeed)
            {
                GetComponent<SpriteRenderer>().sprite = rotations_one[rotation];
            }
            else if (animationStage < animationSpeed * 2)
            {
                GetComponent<SpriteRenderer>().sprite = rotations_two[rotation];
            }
            else if (animationStage < animationSpeed * 3)
            {
                GetComponent<SpriteRenderer>().sprite = rotations_three[rotation];
            }
            else if(animationStage < animationSpeed * 4)
            {
                GetComponent<SpriteRenderer>().sprite = rotations_two[rotation];
            }
        }
    }

    private void Invoke_Smoking()
    {
        Instantiate(Resources.Load("Prefabs/Smoke") as GameObject, GameObject.Find("Map/Smoke").transform).GetComponent<Smoke>().Launch(GetComponent<SpriteRenderer>().sortingOrder, transform.position);
    }
}
