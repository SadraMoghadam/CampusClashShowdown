using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectDelivery : NetworkBehaviour
{
    public float speed = 5f; 
    [SerializeField] private NetworkObject networkObject;
    // [SerializeField] private float minTimeToDestroyImmovableObject = 2f; 
    private int _currentPointIndex = 0; 
    private Rigidbody _rb;
    private bool _isInObjectDestroyingArea = false;
    private MultiplayerController _multiplayerController;
    private List<Transform> _pathPoints;
    private float _timer = 0;
    // private Vector3 _currentPosition; 
    // private Vector3 _previousPosition; 

    void Awake()
    {
        _multiplayerController = MultiplayerController.Instance;
        _pathPoints = _multiplayerController.resourceDeliveryPathPoints;
            
        if (_pathPoints.Count == 0)
        {
            Debug.LogError("No path points defined!");
            enabled = false; 
        }

        GetComponent<Collider>().isTrigger = true;
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
        if (_currentPointIndex < _pathPoints.Count && !_isInObjectDestroyingArea)
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
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("ResourceBox"))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider, true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ObjectDestroyingArea"))
        {
            DestroyObjectServerRpc();
            // StartCoroutine(DestroyObjectProcess());
        }
    }

    
    [ServerRpc(RequireOwnership = false)]
    private void DestroyObjectServerRpc()
    {
        // StartCoroutine(DestroyObjectProcess());
        DestroyObjectClientRpc();
    }
    
    [ClientRpc]
    private void DestroyObjectClientRpc() 
    {
        StartCoroutine(DestroyObjectProcess());
    }

    private IEnumerator DestroyObjectProcess()
    {
        _isInObjectDestroyingArea = true;
        GetComponent<Collider>().isTrigger = false;
        _rb.useGravity = true;
        _rb.freezeRotation = false;
        yield return new WaitForSeconds(1);
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1);
        networkObject.Despawn();
        Destroy(gameObject);
    }

    // private IEnumerator DestroyObjectProcess()
    // {
    //     SetObjectRigidBodyServerRpc();
    //     yield return new WaitForSeconds(1);
    //     DisableObjectColliderServerRpc();
    //     yield return new WaitForSeconds(1);
    //     DestroyObjectServerRpc();
    // }
    //
    // [ServerRpc(RequireOwnership = false)]
    // private void SetObjectRigidBodyServerRpc()
    // {
    //     SetObjectRigidBodyClientRpc();
    // }
    //
    // [ClientRpc]
    // private void SetObjectRigidBodyClientRpc()
    // {
    //     _rb.useGravity = true;
    //     _rb.freezeRotation = false;
    // }
    //
    // [ServerRpc(RequireOwnership = false)]
    // private void DisableObjectColliderServerRpc()
    // {
    //     DisableObjectColliderClientRpc();
    // }
    //
    // [ClientRpc]
    // private void DisableObjectColliderClientRpc() 
    // {
    //     GetComponent<Collider>().enabled = false;
    // }
    //
    // [ServerRpc(RequireOwnership = false)]
    // private void DestroyObjectServerRpc()
    // {
    //     networkObject.Despawn();
    //     DestroyObjectClientRpc();
    // }
    //
    // [ClientRpc]
    // private void DestroyObjectClientRpc() 
    // {
    //     Destroy(gameObject);
    // }

    
    public NetworkObject GetNetworkObject()
    {
        return networkObject;
    }
}
