using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AxisMovementType
{
    X,
    Z,
}

public class MovableObject : MonoBehaviour
{
    [SerializeField] private AxisMovementType axisMovementType;
    [SerializeField] private float moveSpeed = 1f;
    private Vector3 _initPosition;
    private Vector3 _finalPosition;
    private bool _isInPosition; // is the object in the first position


    private void Awake()
    {
        _initPosition = transform.localPosition;
    }

    /// <summary>
    /// Move function that is attached to each movable object and controls the movement of the object depending on the
    /// player (enemies in the opposite direction of allies)  
    /// </summary>
    /// <param name="direction">Can be either 1 or -1 depending on the object and the player team</param>
    public void Move(int direction)
    {
        transform.Translate(ConvertAxisToVector3(axisMovementType) * direction * moveSpeed);
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
