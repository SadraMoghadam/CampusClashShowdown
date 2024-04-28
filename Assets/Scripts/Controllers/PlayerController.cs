using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IParent<PickableObject>
{
    [SerializeField] private float rotateSpeed = 360;
    public ulong playerId;
    public float speed = 2;
    public float runSpeed = 3;
    public float strength = 1;
    public KeyCode interactKey = KeyCode.F;
    public KeyCode pushKey = KeyCode.J;
    public KeyCode pullKey = KeyCode.K;
    [SerializeField] private Transform holdingPoint;
    private Vector3 _input;
    private Rigidbody _rb;
    private Animator _animator;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private ClashArenaController _clashArenaController;
    private PickableObject _pickableObject;



    private static PlayerController _instance;
    public static PlayerController Instance => _instance;

    // private void Awake()
    // {
    //     if (_instance == null)
    //     {
    //         _instance = this;
    //     }
    //     _rb = GetComponent<Rigidbody>();
    //     _animator = GetComponent<Animator>();
    // }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        
        base.OnNetworkSpawn();
        if (_instance == null)
        {
            _instance = this;
        }
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        AssignPlayerId(GetComponent<NetworkObject>().OwnerClientId);
        _clashArenaController = ClashArenaController.Instance;
        transform.position = _clashArenaController.spawnLocations[0].position;

    }

    private void AssignPlayerId(ulong clientId)
    {
        playerId = clientId; 
        Debug.Log("Player with Client ID " + clientId + " assigned ID: " + playerId);
    }

    private void Update() {
        if (!IsOwner) return;
        
        GatherInput();
        Look();
    }

    private void FixedUpdate() {
        Move();
    }

    public Transform GetHoldingPointTransform()
    {
        return holdingPoint;
    }

    private void GatherInput() {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        SetAnimations();
    }

    private void SetAnimations()
    {
        // ANIMATIONS
        // idle
        if (_input == Vector3.zero)
        {
            _animator.SetFloat(Speed, 0);
        }
        // Walk
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            _animator.SetFloat(Speed, 0.5f);
        }
        // run
        else
        {
            _animator.SetFloat(Speed, 1);
        }
    }

    private void Look() {
        if (_input == Vector3.zero) return;

        var rot = Quaternion.LookRotation(Utilities.ToIso(_input), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotateSpeed * Time.deltaTime);
    }

    private void Move()
    {
        if (_input == Vector3.zero) return;
        float actualSpeed = speed;
        // run
        if (Input.GetKey(KeyCode.LeftShift))
        {
            actualSpeed = runSpeed;  
        }
        _rb.MovePosition(transform.position + transform.forward * (_input.normalized.magnitude * actualSpeed * Time.deltaTime));
    }


    public Transform GetChildFollowTransform()
    {
        return holdingPoint;
    }

    public void SetChild(PickableObject child)
    {
        _pickableObject = child;
    }

    public PickableObject GetChild()
    {
        return _pickableObject;
    }

    public void ClearChild()
    {
        _pickableObject = null;
    }

    public bool HasChild()
    {
        return _pickableObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return GetComponent<NetworkObject>();
    }
}