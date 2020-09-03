using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Floater : MonoBehaviour
{
    private Vector2 targetPos;
    private float speed = 1650;
    private float sideSpeed;
    private float decay = 0.99f;
    private Vector2 dir;

    private string type;
    private int amount;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed*Time.deltaTime);

        float newScale = 0.5f + ((transform.localScale.x - 0.5f) * decay);
        transform.localScale = new Vector2(newScale, newScale);

        if (transform.position.Equals(targetPos))
        {
            Globals.GetSave().GetResources().AddRes(type, amount);
            Destroy(gameObject);
        }

        transform.Translate(dir * Time.deltaTime * sideSpeed);
        sideSpeed *= 0.95f;

    }

    public void Launch(Vector2 startPos,string type, int amount)
    {
        transform.position = Camera.main.WorldToScreenPoint(startPos);
        this.type = type;
        this.amount = amount;
        GetComponent<Image>().sprite = Globals.GetSave().GetResources().FindResSprite(type);

        dir = new Vector2(-1 + Random.Range(0, 2f), -1 + Random.Range(0, 2f));
        sideSpeed = speed;

        Globals.GetSave().GetResources().NewRes(type);
        Globals.GetInterface().Update();
        foreach (Transform trans in GameObject.Find("Canvas/Top Bar/Resources").transform)
        {
            if (trans.gameObject.name.Equals(type))
            {
                targetPos = trans.position;
                return;
            }
        }
    }
}
