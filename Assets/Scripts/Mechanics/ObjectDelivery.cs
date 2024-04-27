using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDelivery : MonoBehaviour
{
    public Transform[] pathPoints; 
    public float speed = 5f; 
    private int _currentPointIndex = 0; 
    private Rigidbody _rb;
    private bool _isInObjectDestroyingArea = false;

    void Start()
    {
        // Ensure there is at least one path point defined
        if (pathPoints.Length == 0)
        {
            Debug.LogError("No path points defined!");
            enabled = false; // Disable the script if no path points are defined
        }

        _rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
    }

    void FixedUpdate()
    {
        // Move towards the current point using Rigidbody physics
        if (_currentPointIndex < pathPoints.Length && !_isInObjectDestroyingArea)
        {
            Vector3 targetPosition = pathPoints[_currentPointIndex].position;
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            _rb.velocity = moveDirection * speed;

            // Check if reached the current point
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                // Move to the next point in the path
                _currentPointIndex = (_currentPointIndex + 1);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ObjectDestroyingArea"))
        {
            _isInObjectDestroyingArea = true;
            StartCoroutine(DestroyObjectProcess());
        }
    }

    private IEnumerator DestroyObjectProcess()
    {
        _rb.useGravity = true;
        _rb.freezeRotation = false;
        yield return new WaitForSeconds(1);
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
