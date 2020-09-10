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
    private Vector3 midPoint;
    private float progress;
    private int targetLayer;
    public int ticksPerTile = 10;
    public float secondsPerTile = 1f;
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
 
    public void ThreePointMove(Gate targetGate, Vector3 midPoint)
    {
        CancelInvoke();
        isFinal = false;
        progress = 0;
        this.midPoint = midPoint;
        targetPos = targetGate.GetPosition();
        targetLayer = targetGate.GetOrder();
        startingPos = transform.position;
        InvokeRepeating("Transition", 0f, secondsPerTile / (float)ticksPerTile / 2);
    }
    public void Move(Gate targetGate, bool final)
    {
        CancelInvoke("Transition");
        isFinal = final;
        progress = 0;
        targetPos = targetGate.GetPosition();
        targetLayer = targetGate.GetOrder();
        startingPos = transform.position;
        InvokeRepeating("Transition", 0f, secondsPerTile / (float)ticksPerTile);
    }
    public void Teleport(Gate targetGate, float delay)
    {

        CancelInvoke("Transition");
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
    }
    public void Dispose()
    {
        Destroy(gameObject);
    }
    public void Fade(bool isFinal)
    {
        CancelInvoke("Fade");
        this.isFinal = isFinal;
        progress = 0;
        InvokeRepeating("Invoke_Fade", 0, secondsPerTile / ticksPerTile /2);
    }
    public void Invoke_Fade()
    {
        progress += secondsPerTile / ticksPerTile;

        Color newColor = GetComponent<SpriteRenderer>().color;
        newColor.a = 1f - progress;
        GetComponent<SpriteRenderer>().color = newColor;

        if (progress >= 1)
        {
            CancelInvoke();
            if (isFinal)
            {
                Destroy(gameObject);
            }
        }
    }
    public void Appear()
    {
        Color newColor = GetComponent<SpriteRenderer>().color;
        newColor.a = 0f;
        GetComponent<SpriteRenderer>().color = newColor;
        InvokeRepeating("Invoke_Appear", 0, secondsPerTile/ ticksPerTile);
    }
    public void Invoke_Appear()
    {
        Color newColor = GetComponent<SpriteRenderer>().color;
        newColor.a+=secondsPerTile/ticksPerTile;
        GetComponent<SpriteRenderer>().color = newColor;

        if (newColor.a >= 1f)
        {
            CancelInvoke("Invoke_Appear");
        }
    }

    private void Transition()
    {
        transform.position = Vector3.Lerp(startingPos, midPoint==Vector3.zero?targetPos:midPoint, progress);

        progress += 1f / (float)ticksPerTile;
        if (progress > 0.5f)
        {
            GetComponent<SpriteRenderer>().sortingOrder = targetLayer;

        }
        if (progress >= 1f)
        {
            gameObject.SetActive(true);
            CancelInvoke("Transition");

            if (midPoint!=Vector3.zero)
            {
                startingPos = midPoint;
                midPoint = Vector3.zero;
                progress = 0f;
                InvokeRepeating("Transition", 0f, secondsPerTile / (float)ticksPerTile / 2);
            }

            if (isFinal)
            {
                Dispose();
            }
        }
    }
}
