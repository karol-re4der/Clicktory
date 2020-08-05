using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tap_Feel : MonoBehaviour
{
    public bool feelEnabled = true;
    public float strenght = 1.5f;
    public float decay = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale != Vector3.one)
        {
            float newScale = transform.localScale.x - decay;
            if (newScale < 1)
            {
                transform.localScale = Vector3.one;
            }
            else
            {
                transform.localScale = new Vector3(newScale, newScale, 1);
            }
        }
    }

    void OnMouseDown()
    {
        if (feelEnabled)
        {
            transform.localScale = new Vector3(strenght, strenght, 1);
        }
    }
}
