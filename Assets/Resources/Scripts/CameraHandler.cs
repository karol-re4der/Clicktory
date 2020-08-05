using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CameraHandler : MonoBehaviour
{

    private Vector3 dragPivot;

    void Start()
    {
        
    }

    void Update()
    {
        if (!Application.isEditor)
        {
            if (!EventSystem.current.IsPointerOverGameObject(0))
            {
                if (Input.touches.Length > 0)
                {
                    if (Input.touches.Length == 2)
                    {
                        if (Input.touches[0].phase == TouchPhase.Began || Input.touches[1].phase == TouchPhase.Began)
                        {
                            SavePivot(Vector2.Lerp(Input.touches[0].position, Input.touches[1].position, 0.5f));
                        }
                        else if (Input.touches[0].phase == TouchPhase.Moved || Input.touches[1].phase == TouchPhase.Moved)
                        {
                            DragTo(Vector2.Lerp(Input.touches[0].position, Input.touches[1].position, 0.5f));
                        }

                        if (Input.touches[0].deltaPosition != Vector2.zero || Input.touches[1].deltaPosition != Vector2.zero)
                        {
                            float a = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
                            float b = Vector2.Distance(Input.touches[0].position + Input.touches[0].deltaPosition, Input.touches[1].position + Input.touches[1].deltaPosition);
                            float delta = a / b;
                            Camera.main.orthographicSize *= delta;
                        }
                    }
                    else if (Input.touches.Length == 1)
                    {
                        if (Input.touches[0].phase == TouchPhase.Began)
                        {
                            SavePivot(Input.touches[0].position);
                        }
                        else if (Input.touches[0].phase == TouchPhase.Moved)
                        {
                            DragTo(Input.touches[0].position);
                        }
                    }
                }
            }
        }
        else
        {
            transform.Translate(Vector3.right*Input.GetAxis("Horizontal")*10*Time.deltaTime + Vector3.up*Input.GetAxis("Vertical")*10*Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                Camera.main.orthographicSize *= 1.05f;
            }
            else if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                Camera.main.orthographicSize *= 0.95f;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                if (Input.mouseScrollDelta.y < Camera.main.orthographicSize)
                {
                    Camera.main.orthographicSize -= Input.mouseScrollDelta.y;
                }
            }
        }
    }

    private void SavePivot(Vector2 pivot)
    {
        dragPivot = Camera.main.ScreenToWorldPoint(pivot);
    }

    private void DragTo(Vector2 touch)
    {
        Vector3 currentPos = new Vector3(touch.x, touch.y, 0);
        currentPos = Camera.main.ScreenToWorldPoint(currentPos);
        Vector3 offset = dragPivot - currentPos;
        transform.position = transform.position + offset;
    }
}
