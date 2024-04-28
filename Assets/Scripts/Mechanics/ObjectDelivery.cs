using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectDelivery : NetworkBehaviour
{
    public float speed = 5f; 
    // [SerializeField] private float minTimeToDestroyImmovableObject = 2f; 
    private int _currentPointIndex = 0; 
    private Rigidbody _rb;
    private bool _isInObjectDestroyingArea = false;
    private ClashArenaController _clashArenaController;
    private Transform[] _pathPoints;
    private NetworkObject _networkObject;
    private float _timer = 0;
    // private Vector3 _currentPosition; 
    // private Vector3 _previousPosition; 

    void Awake()
    {
        SetNetworkObject();
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

        // if (Vector3.Distance(_previousPosition, _currentPosition) == 0)
        // {
        //     _timer += Time.fixedDeltaTime;
        //     if (_timer > minTimeToDestroyImmovableObject)
        //     {
        //         StartCoroutine(DestroyObjectProcess());
        //         _timer = 0;
        //     }
        // }
        // else
        // {
        //     _timer = 0;
        // }
    }

    private void MoveObject()
    {
        if (_currentPointIndex < _pathPoints.Length && !_isInObjectDestroyingArea)
        {
            Vector3 targetPosition = _pathPoints[_currentPointIndex].position;
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            // _previousPosition = transform.position;
            _rb.velocity = moveDirection * speed;
            // _currentPosition = transform.position;

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
            DestroyObjectServerRpc();
        }
    }

    private IEnumerator DestroyObjectProcess()
    {
        _rb.useGravity = true;
        _rb.freezeRotation = false;
        yield return new WaitForSeconds(1);
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1);
        _networkObject.Despawn();
        Destroy(gameObject);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void DestroyObjectServerRpc()
    {
        DestroyObjectClientRpc();
    }
    
    [ClientRpc]
    private void DestroyObjectClientRpc() 
    {
        StartCoroutine(DestroyObjectProcess());
    }


    public void SetNetworkObject()
    {
        _networkObject = GetComponent<NetworkObject>();
    }
    
    public NetworkObject GetNetworkObject()
    {
        return _networkObject;
    }
}
