using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum AxisMovementType
{
    X,
    Z,
}

public class MovableObject : NetworkBehaviour
{
    [SerializeField] private AxisMovementType axisMovementType;
    // [SerializeField] private float moveSpeed = 1f;
    // [SerializeField] private Vector3 initPosition;
    [SerializeField] private int initPosition; // Either 0, 1, or -1
    [SerializeField] private Transform childTransform;
    private int _objectCurrentPosition;
    private PlayerController _playerController;
    private bool _isInPosition; // is the object in the first position
    private Animator _animator;
    private static readonly int InitializePosition = Animator.StringToHash("InitializePosition");
    private static readonly int MovePositive = Animator.StringToHash("MovePositive");
    private static readonly int MoveNegative = Animator.StringToHash("MoveNegative");


    private void Start()
    {
        // childTransform = transform;
        // childTransform.localPosition = initPosition;
        _animator = GetComponent<Animator>();
        _objectCurrentPosition = initPosition;
        InitializeAnimation();
        StartCoroutine(WaitForPlayerControllerInitialization());
    }
    
    private IEnumerator WaitForPlayerControllerInitialization()
    {
        // Wait until the PlayerController instance is available
        while (PlayerController.Instance == null)
        {
            yield return null;
        }
        _playerController = PlayerController.Instance;
    }

    /// <summary>
    /// Move function that is attached to each movable object and controls the movement of the object depending on the
    /// player (enemies in the opposite direction of allies)  
    /// </summary>
    /// <param name="direction">Can be either 1 or -1 depending on the object and the player team</param>
    public void Move(int direction)
    {
        // if (_playerController == null)
        // {
        //     _playerController = PlayerController.Instance;
        // }
        // // Calculate the movement vector based on the direction and other parameters
        // Vector3 movement = ConvertAxisToVector3(axisMovementType) * direction * moveSpeed * _playerController.strength;
        //
        // // Apply the movement to the object's position
        // transform.Translate(movement);
        // MoveServerRpc(direction);
        StartCoroutine(MoveCR(direction));
    }

    private IEnumerator MoveCR(int direction)
    {
        yield return new WaitForSeconds(0.2f);
        MoveServerRpc(direction);   
        StopAllCoroutines();
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveServerRpc(int direction)
    {
        MoveObject(direction);
        // Synchronize the movement with all clients
        SetTransformClientRpc(childTransform.localPosition);
    }

    [ClientRpc]
    private void SetTransformClientRpc(Vector3 newPosition)
    {
        // Update the object's position on all clients
        childTransform.localPosition = newPosition;
    }

    private void MoveObject(int direction)
    {
        if ((_objectCurrentPosition == -1 && direction == -1) || 
            (_objectCurrentPosition == 1 && direction == 1))
        {
            return;
        }

        _objectCurrentPosition += direction;
        TriggerAnimation(direction);

    }

    // private void MoveObject(int direction)
    // {
    //     Vector3 movement = ConvertAxisToVector3(axisMovementType) * direction * moveSpeed * _playerController.strength;
    //     float localPosValue = (axisMovementType == AxisMovementType.Z ? childTransform.localPosition.z : childTransform.localPosition.x);
    //     if ((localPosValue >= 0.9 && direction == 1) ||
    //         (localPosValue <= -0.9 && direction == -1))
    //     {
    //         return;
    //     }
    //     childTransform.localPosition += movement;
    // }

    
    private Vector3 ConvertAxisToVector3(AxisMovementType type)
    {
        
        switch (type)
        {
            case AxisMovementType.X:
                return Vector3.right;
            case AxisMovementType.Z:
                return Vector3.forward;
        }

        return Vector3.right;
    }

    private void InitializeAnimation()
    {
        _animator.SetInteger(InitializePosition, (int)initPosition);
    }

    private void TriggerAnimation(int direction)
    {
        _animator.SetTrigger(direction == 1 ? MovePositive : MoveNegative);
    }
    
}
