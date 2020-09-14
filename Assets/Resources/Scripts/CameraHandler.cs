using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CameraHandler : MonoBehaviour
{

    private Vector3 dragPivot;
    private Vector3 targetPos;
    private float targetZoom;
    private float transitionRate = 0.01f;

    public float maxCameraSize;
    public float minCameraSize;

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
                            if (Camera.main.orthographicSize > maxCameraSize)
                            {
                                Camera.main.orthographicSize = maxCameraSize;
                            }
                            else if(Camera.main.orthographicSize < minCameraSize)
                            {
                                Camera.main.orthographicSize = minCameraSize;
                            }
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
                FixBounds();
            }
        }
        else
        {
            transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * 10 * Time.deltaTime + Vector3.up * Input.GetAxis("Vertical") * 10 * Time.deltaTime);


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
            FixBounds();
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
    private void FixBounds()
    {
        float xMaxBound = Globals.GetSave().GetGrid()[Globals.GetSave().GetGrid().GetLength(0)-1, 2].transform.position.x;
        float xMinBound = Globals.GetSave().GetGrid()[0, 2].transform.position.x;
        float yMaxBound = Globals.GetSave().GetGrid()[0, Globals.GetSave().GetGrid().GetLength(1) - 1].transform.position.y;
        float yMinBound = Globals.GetSave().GetGrid()[0, 1].transform.position.y;

        if (transform.position.x < xMinBound)
        {
            transform.position = new Vector3(xMinBound, transform.position.y, transform.position.z);
        }
        else if(transform.position.x > xMaxBound)
        {
            transform.position = new Vector3(xMaxBound, transform.position.y, transform.position.z);
        }
        if (transform.position.y < yMinBound)
        {
            transform.position = new Vector3(transform.position.x, yMinBound, transform.position.z);
        }
        else if (transform.position.y > yMaxBound)
        {
            transform.position = new Vector3(transform.position.x, yMaxBound, transform.position.z);
        }
    }

    public void CenterCamera(Vector3 newPos, float targetZoom = 3, bool instant = true)
    {
        CancelInvoke();
        if (instant)
        {
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
            Camera.main.orthographicSize = targetZoom;
        }
        else
        {
            targetPos = newPos;
            this.targetZoom = targetZoom;
            InvokeRepeating("Transition", 0, transitionRate);
        }
    }
    private void Transition()
    {
        if (Vector2.Distance(transform.position, targetPos) >= transitionRate)
        {
            float z = transform.position.z;
            transform.position = Vector2.Lerp(transform.position, targetPos, transitionRate * 10);
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }
        if(Mathf.Abs(GetComponent<Camera>().orthographicSize - targetZoom) >= transitionRate)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, transitionRate*10);
        }

        if (Vector2.Distance(transform.position, targetPos) < transitionRate && Mathf.Abs(GetComponent<Camera>().orthographicSize-targetZoom)< transitionRate)
        {
            CancelInvoke();
        }
    }
}
