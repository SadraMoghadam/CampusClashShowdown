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
    private bool _isObjectDelivered;
    private MultiplayerController _multiplayerController;
    private List<Transform> _pathPoints;
    private float _timer = 0;
    private NetworkVariable<bool> _isStopped;

    private TeamCharacteristicsScriptableObject _teamCharacteristics;

    private NetworkVariable<int> _teamId = new NetworkVariable<int>(0); 
    // private Vector3 _currentPosition; 
    // private Vector3 _previousPosition; 

    void Awake()
    {
        _multiplayerController = MultiplayerController.Instance;
        _pathPoints = ClashArenaController.Instance.resourceDeliveryPathPoints;
        _isObjectDelivered = false;
        _isStopped = new NetworkVariable<bool>(_multiplayerController.GetIsConveyorBeltStopped());
        // if (!networkObject.IsSpawned)
        // {
        //     networkObject.Spawn(true);
        // }
            
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
        if (_isStopped.Value)
        {
            _rb.velocity = Vector3.zero;
        }
        else if (_currentPointIndex < _pathPoints.Count && !_isInObjectDestroyingArea)
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
            GameManager.Instance.AudioManager.Instantplay(SoundName.DestroyedBox, transform.position);
            DestroyObjectServerRpc();
            // StartCoroutine(DestroyObjectProcess());
        }
        
        if (other.CompareTag("ObjectDeliveryArea"))
        {
            GameManager.Instance.AudioManager.Instantplay(SoundName.BoxDelivered, transform.position);
            ResourceObjectDelivered();
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
        Team team = _teamId.Value == 1 ? Team.Team2 : Team.Team1;
        ClashRewardCalculator.Instance.AddRewardByRewardType(team, RewardType.BoxDestruction);
    }

    private IEnumerator DestroyObjectProcess()
    {
        _isInObjectDestroyingArea = true;
        GetComponent<Collider>().isTrigger = false;
        _rb.useGravity = true;
        _rb.freezeRotation = false;
        yield return new WaitForSeconds(1);
        ClashVFXContainer.InstantiateVFX(ClashVFXType.DestroyBoxOnConveyor, transform.position, 4.0f);
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(1);
        networkObject.Despawn();
        Destroy(gameObject);
    }
    
    
    public static void SpawnResourceBoxOnDeliveryPath(IParent<PickableObject> objectParent)
    {
        MultiplayerController.Instance.SpawnResourceBoxOnDeliveryPath(objectParent);
    }

    public void SetResourceObjectAttributes(TeamCharacteristicsScriptableObject teamCharacteristics)
    {
        _teamCharacteristics = teamCharacteristics;
        _teamId.Value = teamCharacteristics.team == Team.Team1 ? 1 : 2;
        Color teamColor = teamCharacteristics.color;
        float r = teamColor.r;
        float g = teamColor.g;
        float b = teamColor.b;
        SetResourceObjectAttributesServerRpc(r, g, b, teamCharacteristics.team == Team.Team1 ? 1 : 2);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetResourceObjectAttributesServerRpc(float r, float g, float b, int teamId)
    {
        SetResourceObjectAttributesClientRpc(r, g, b, teamId);
    }

    [ClientRpc]
    private void SetResourceObjectAttributesClientRpc(float r, float g, float b, int teamId)
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b); // Set the color of boxes with respect to their team
        Team team = teamId == 1 ? Team.Team1 : Team.Team2;
        ClashRewardCalculator.Instance.AddRewardByRewardType(team, RewardType.BoxConveyorPlacement);

    }

    private void ResourceObjectDelivered()
    {
        DeliverObjectServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void DeliverObjectServerRpc()
    {
        int teamId = _teamCharacteristics.id;
        DeliverObjectClientRpc(teamId);
    }
    
    [ClientRpc]
    private void DeliverObjectClientRpc(int teamId) 
    {
        StartCoroutine(DeliverObjectProcess(teamId));
    }

    private IEnumerator DeliverObjectProcess(int teamId)
    {
        if (!_isObjectDelivered)
        {
            _multiplayerController.IncreaseTeamScore(teamId);
            _isObjectDelivered = true;
            yield return new WaitForSeconds(1);
            networkObject.Despawn();
            Destroy(gameObject);
            
        }
    }

    public NetworkObject GetNetworkObject()
    {
        return networkObject;
    }

    public void SetMovement()
    {
        SetMovementServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetMovementServerRpc()
    {
        _isStopped.Value = !_isStopped.Value;
        _multiplayerController.SetIsConveyorBeltStopped(_isStopped.Value);
    }
    
}
