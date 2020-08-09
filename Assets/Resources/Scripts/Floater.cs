using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Floater : MonoBehaviour
{
    private Vector2 targetPos;
    private float speed = 2000;

    private string type;
    private int amount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed*Time.deltaTime);

        if (transform.position.Equals(targetPos))
        {
            Globals.GetSave().GetResources().AddRes(type, amount);
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 startPos,string type, int amount)
    {
        transform.position = Camera.main.WorldToScreenPoint(startPos);
        this.type = type;
        this.amount = amount;
        GetComponent<Image>().sprite = Globals.GetInterface().FindResSprite(type);

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
