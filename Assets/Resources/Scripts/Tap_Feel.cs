using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tap_Feel : MonoBehaviour
{
    public bool feelEnabled = true;
    public float strengh = 1.5f;
    public float decay = 0.05f;

    private Vector3 orginalPosition;
    private bool positionSet = false;
    private float progress = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Invoke_Return()
    {
        if (positionSet && transform.position != orginalPosition)
        {
            progress += decay;
            if (progress < 1)
            {
                transform.position = new Vector3(transform.position.x, orginalPosition.y + strengh*(1-progress), transform.position.z);
            }
            else
            {
                transform.position = orginalPosition;
                CancelInvoke();
            }
        }
    }

    void OnMouseDown()
    {
        if (!Globals.IsBuilding())
        {
            if (Globals.IsPointerInGame())
            {
                if (!positionSet)
                {
                    orginalPosition = transform.position;
                    positionSet = true;
                }
                if (feelEnabled)
                {
                    CancelInvoke();
                    progress = 0;
                    transform.position = new Vector3(transform.position.x, orginalPosition.y + strengh, transform.position.z);
                    InvokeRepeating("Invoke_Return", 0, 0.1f);
                }
            }
        }
    }
}
