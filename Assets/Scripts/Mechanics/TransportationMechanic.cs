using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TransportationMechanic : MonoBehaviour
{
    public float speed = 1f;
    [SerializeField] private List<Transform> pathPoints; 
    private int _currentPointIndex = 0; 

    void Awake()
    {   
        if (pathPoints.Count == 0)
        {
            Debug.LogError("No path points defined!");
            enabled = false; 
        }
    }
    
    void FixedUpdate()
    {
        MoveObject();
    }
    
    private void MoveObject()
    {
        if (pathPoints.Count == 0) return;

        Vector3 targetPosition = pathPoints[_currentPointIndex].position;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        // Move the object towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime);

        // Rotate the object to face the target position
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            // targetRotation =
            //     new Quaternion(targetRotation.x, targetRotation.y - 180, targetRotation.z, targetRotation.w);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * 5 * Time.fixedDeltaTime);
        }

        // Check if the object is close enough to the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            _currentPointIndex = (_currentPointIndex + 1) % pathPoints.Count;
        }
    }
}