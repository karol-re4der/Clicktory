using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnPointerHeldDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent onPointerHeldDown = new UnityEvent();
    private float startAt = 0;
    public float delay = 1f;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (startAt == 0)
        {
            startAt = Time.time;
            Invoke("OnPointerHeld", delay);
        }
    }

    public void OnPointerHeld()
    {
        startAt = 0;
        onPointerHeldDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CancelInvoke();
        startAt = 0;
    }
}
