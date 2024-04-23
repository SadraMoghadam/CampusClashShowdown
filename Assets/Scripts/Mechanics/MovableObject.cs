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
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Vector3 initPosition;
    [SerializeField] private Transform childTransform;
    private Vector3 _finalPosition;
    private PlayerController _playerController;
    private bool _isInPosition; // is the object in the first position


    private void Start()
    {
        childTransform.localPosition = initPosition;
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
        float localPosValue = axisMovementType == AxisMovementType.Z ? childTransform.localPosition.z : childTransform.localPosition.x;
        if ((localPosValue >= 0.9 && direction == 1) ||
            (localPosValue <= -0.9 && direction == -1))
        {
            return;
        }
        StartCoroutine(MoveCR(direction));
    }

    private IEnumerator MoveCR(int direction)
    {
        yield return new WaitForSeconds(1);
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
        Vector3 movement = ConvertAxisToVector3(axisMovementType) * direction * moveSpeed * _playerController.strength;
        transform.localPosition += movement;
    }

    
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
    
    
}
