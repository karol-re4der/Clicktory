using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    public float speed = 0.6f;
    public float duration = 10f;
    public float speedDeviation = 0.2f;
    public float durationDeviation = 5f;
    public float chance = 0.33f;
    public float growth = 2;

    private float launchTime;


    void Update()
    {
        float progress = (float)(Time.time - launchTime) / duration;

        transform.Translate(Vector2.up * speed * Time.deltaTime);
        transform.localScale = new Vector2(progress * growth, progress * growth);

        transform.Translate(Globals.GetLogic().windDir * Globals.GetLogic().windPower * Time.deltaTime*progress);

        Color newColor = GetComponent<SpriteRenderer>().color;
        newColor.a = 0.8f-progress*0.8f;
        GetComponent<SpriteRenderer>().color = newColor;
    }

    public void Launch(int layer, Vector2 position)
    {
        if (Random.Range(0, 1f) < chance)
        {
            launchTime = Time.time;
            GetComponent<SpriteRenderer>().sortingOrder = layer + 1;
            transform.position = position;

            duration -= -durationDeviation + Random.Range(0, durationDeviation * 2);
            Invoke("Invoke_End", duration);
        }
        else
        {
            Invoke_End();
        }
    }
    private void Invoke_End()
    {
        Destroy(gameObject);
    }
}
