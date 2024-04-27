using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectDelivery : NetworkBehaviour
{
    public float speed = 5f; 
    private int _currentPointIndex = 0; 
    private Rigidbody _rb;
    private bool _isInObjectDestroyingArea = false;
    private ClashArenaController _clashArenaController;
    public Transform[] _pathPoints; 

    void Start()
    {
        _clashArenaController = ClashArenaController.Instance;
        _pathPoints = _clashArenaController.resourcePathPoints;
            
        if (_pathPoints.Length == 0)
        {
            Debug.LogError("No path points defined!");
            enabled = false; 
        }

        _rb = GetComponent<Rigidbody>(); 
    }

    void FixedUpdate()
    {
        MoveObject();
    }

    private void MoveObject()
    {
        if (_currentPointIndex < _pathPoints.Length && !_isInObjectDestroyingArea)
        {
            Vector3 targetPosition = _pathPoints[_currentPointIndex].position;
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            _rb.velocity = moveDirection * speed;

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
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
