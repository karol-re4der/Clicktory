using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hidden : MonoBehaviour
{
    public bool onlyEditor = true;

    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isEditor || !onlyEditor)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
