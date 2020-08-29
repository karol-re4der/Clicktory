using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowingResource : MonoBehaviour
{
    //base variables
    public int amount;
    public string type;

    //transition variables
    private Vector3 targetPos;
    private Vector3 startingPos;
    private float progress;
    private int targetLayer;
    public int ticksPerTile = 10;
    public float secondsPerTile = 0.2f;
    private bool isFinal = false;

    public FlowingResource(int amount, string type)
    {
        this.amount = amount;
        this.type = type;
    }

    public void Refresh()
    {
        GetComponent<SpriteRenderer>().sprite = Globals.GetSave().GetResources().FindResSprite(type);
    }
 
    public void Move(Gate targetGate, bool final)
    {
        CancelInvoke();
        isFinal = final;
        progress = 0;
        targetPos = targetGate.GetPosition();
        targetLayer = targetGate.GetOrder();
        startingPos = transform.position;
        InvokeRepeating("Transition", 0f, secondsPerTile / (float)ticksPerTile);
    }
    public void Teleport(Gate targetGate, float delay)
    {

        CancelInvoke();
        gameObject.SetActive(false);
        progress = 1;
        targetPos = targetGate.GetPosition();
        targetLayer = targetGate.GetOrder();
        isFinal = false;
        InvokeRepeating("Transition", delay, secondsPerTile / (float)ticksPerTile);

    }

    public void Store()
    {
        Globals.GetResources().AddRes(type, amount);
        Destroy(gameObject);
    }
    public void Dispose()
    {
        Destroy(gameObject);
    }

    private void Transition()
    {
        transform.position = Vector3.Lerp(startingPos, targetPos, progress);
        progress += 1f / (float)ticksPerTile;
        if (progress > 0.5f)
        {
            GetComponent<SpriteRenderer>().sortingOrder = targetLayer;
        }
        if (progress >= 1f)
        {
            gameObject.SetActive(true);
            CancelInvoke();
            if (isFinal)
            {
                Dispose();
            }
        }
    }
}
