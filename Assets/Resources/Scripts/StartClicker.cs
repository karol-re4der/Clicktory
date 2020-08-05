using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartClicker : MonoBehaviour
{
    public int times;
    private bool executed = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (!executed)
        {
            for (int i = 0; i < times; i++)
            {
                GetComponent<Button>().onClick.Invoke();
            }
            executed = true;
        }
    }
}
