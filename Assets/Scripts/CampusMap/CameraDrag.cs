using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
   private bool isDragging = false;

    private Vector3 origin;

    private Vector3 difference;

    private Vector3 resetCamera;
    private Vector3 targetPosition;

    public float smoothSpeed;


    public BoxCollider cameraBoundary;


    private void Start()

    {

        resetCamera = Camera.main.transform.position;

    }
   private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
        if (Input.GetMouseButton(0))
        {
            OnMouseDrag();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp();
        }
          if (isDragging)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, smoothSpeed * Time.fixedDeltaTime);
        }
    }

  private void OnMouseDown()
    {
          isDragging = true;
            Vector3 mousePoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
            origin = Camera.main.ScreenToWorldPoint(mousePoint);
            origin.y = Camera.main.transform.position.y;
    }   
    private void OnMouseDrag()

    {
if (isDragging)
        {
          Vector3 mousePoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
            difference = Camera.main.ScreenToWorldPoint(mousePoint) - Camera.main.transform.position;
            targetPosition = origin - difference;
            targetPosition = ClampToBounds(targetPosition);
            targetPosition.y = Camera.main.transform.position.y;
        }

    }


    private void OnMouseUp()
    {

        isDragging = false;

    }

    private Vector3 ClampToBounds(Vector3 position)

    {

        float minX = cameraBoundary.bounds.min.x;

        float maxX = cameraBoundary.bounds.max.x;

        float minZ = cameraBoundary.bounds.min.z;

        float maxZ = cameraBoundary.bounds.max.z;


        position.x = Mathf.Clamp(position.x, minX, maxX);

        position.z = Mathf.Clamp(position.z, minZ, maxZ);


        return position;

    }
}
