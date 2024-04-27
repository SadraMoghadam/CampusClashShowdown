using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractionFunctionalities : NetworkBehaviour
{
    
    [SerializeField] private float distanceToInteract = 0.2f;
    [SerializeField] private float timeToMoveObjects = 1f;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private float pushCooldownTime = 2f;
    private bool _isInPickableArea = false;
    private bool _isInPushablePositiveArea = false;
    private bool _isInPushableNegativeArea = false;
    private bool _isInPressableArea = false;
    private int _holdingLayer = 1;
    private int _pressingLayer = 2;
    private int _pickingLayer = 3;
    private int _holdingPushLayerWeight = 0;
    private int _holdingPickLayerWeight = 0;
    private int _pressingLayerWeight = 0;
    private PlayerController _playerController;
    private FollowTransform _followTransform;
    private KeyCode _interactKey;
    private KeyCode _pushKey;
    private KeyCode _pullKey;
    private Transform _pickedObjectTransform;
    private bool _isObjectPickedUp = false;
    private bool _isObjectPushedorPulled = false;
    private Animator _animator;
    private Transform _colliderTransform;
    private Transform _objectMovingPointTransform;
    private int _moveObjectDirection = 1; // 1 is in positive direction and -1 is in negative direction
    private float _timePushed = 0f;


    public override void OnNetworkSpawn()
    {
        _playerController = PlayerController.Instance;
        
        if (_playerController == null)
        {
            _interactKey = KeyCode.F;   
        }
        else
        {
            _interactKey = _playerController.interactKey;
            _pushKey = _playerController.pushKey;
            _pullKey = _playerController.pullKey;
        }
        _animator = GetComponent<Animator>();
        _followTransform = GetComponent<FollowTransform>();
    }

    private void Update()
    {
        if (IsLocalPlayer)
        {
            Interact();
            if (_isObjectPushedorPulled)
            {
                PushAndPull(_colliderTransform);
                _isObjectPushedorPulled = false;
                _timePushed = Time.time + pushCooldownTime;
            }
        }
    }

    // private void FixedUpdate()
    // {
    //
    //     if (IsLocalPlayer)
    //     {
    //         if (_isObjectPushed || _isObjectPulled)
    //         {
    //             PushAndPull(_colliderTransform);
    //             _isObjectPulled = false;
    //             _isObjectPushed = false;
    //         }
    //     }
    // }


    private void InteractWithRaycast()
    {
        RaycastHit hit;
        Debug.DrawLine(transform.position + Vector3.up, transform.position + transform.forward * distanceToInteract + Vector3.up);
        // if (_isObjectPickedUp)
        // {
        //     // Drop the object
        //     // _interactionFunctionalities.DropDown(_animator, hit.transform);
        //     
        //     _holdingPushLayerWeight = 1;
        //     if (Input.GetKeyDown(_interactKey))
        //     {
        //         // DropDown();
        //     }
        // }
        if (Physics.Raycast(transform.position, transform.forward + Vector3.up, out hit, distanceToInteract,
                     layerMaskInteract))
        {
            // Debug.Log(hit.collider.tag);

            if (hit.collider.CompareTag(ClashArenaController.ObjectType.Pickable.ToString()))
            {
                _colliderTransform = hit.collider.transform;
                // Pick up the object
                if (Input.GetKeyDown(_interactKey))
                {
                    PickUp(_colliderTransform);
                }
            }
            else if (hit.collider.CompareTag(ClashArenaController.ObjectType.Pressable.ToString()))
            {
                if (Input.GetKeyDown(_interactKey))
                {
                    Press();
                }
            }
            else if (hit.collider.CompareTag(ClashArenaController.ObjectType.Pushable.ToString() + "PositiveEdge"))
            {
                _colliderTransform = hit.collider.transform.parent.parent;
                _objectMovingPointTransform = hit.collider.transform;
                if (Input.GetKeyDown(_pushKey) || Input.GetKeyDown(_pullKey))
                {
                    _isObjectPushedorPulled = true;
                    SetMovableObjectAsParent(_colliderTransform.transform);
                    _timePushed = Time.time;
                    _holdingPushLayerWeight = 1;
                    SetLayerWeightServerRpc(_holdingLayer, _holdingPushLayerWeight);
                }
                // if (Input.GetKeyUp(_pushKey) || Input.GetKeyUp(_pullKey))
                // {
                //     _isObjectPushedorPulled = false;
                //     _timePushed = Time.time;
                //     _holdingPushLayerWeight = 0;
                //     SetLayerWeightServerRpc(_holdingLayer, _holdingPushLayerWeight);
                //     SetMovableObjectAsParent(null);
                // }

                float deltaTime = Time.time - _timePushed;
                if (Input.GetKey(_pushKey) || Input.GetKey(_pullKey))
                {
                    if (deltaTime >= timeToMoveObjects / _playerController.strength)
                    {
                        _timePushed = Time.time;
                        PushAndPullBehavior("Positive");
                    }
                }
            }
            else if (hit.collider.CompareTag(ClashArenaController.ObjectType.Pushable.ToString() + "NegativeEdge"))
            {
                _colliderTransform = hit.collider.transform.parent.parent;
                _objectMovingPointTransform = hit.collider.transform.GetChild(0);
                if (Input.GetKeyDown(_pushKey) || Input.GetKeyDown(_pullKey))
                {
                    SetMovableObjectAsParent(_colliderTransform.transform);
                    _timePushed = Time.time;
                    _holdingPushLayerWeight = 1;
                    SetLayerWeightServerRpc(_holdingLayer, _holdingPushLayerWeight);
                }
                // if (Input.GetKeyUp(_pushKey) || Input.GetKeyUp(_pullKey))
                // {
                //     _isObjectPushedorPulled = false;
                //     _timePushed = Time.time;
                //     _holdingPushLayerWeight = 0;
                //     SetLayerWeightServerRpc(_holdingLayer, _holdingPushLayerWeight);
                //     _followTransform.SetIsFollowing(false);
                // }

                float deltaTime = Time.time - _timePushed;
                if (Input.GetKey(_pushKey) || Input.GetKey(_pullKey))
                {
                    if (deltaTime >= timeToMoveObjects / _playerController.strength)
                    {
                        _timePushed = Time.time;
                        PushAndPullBehavior("Negative");
                    }
                }
            }
        }
        else
        {
            if (_isObjectPushedorPulled)
            {
                _isObjectPushedorPulled = false;
                _followTransform.SetIsFollowing(false);
                _holdingPushLayerWeight = 0;
                SetLayerWeightServerRpc(_holdingLayer, _holdingPushLayerWeight);   
                _timePushed = 0;
            }
        }
    }

    private void Interact()
    {
        if (_isInPickableArea)
        {
            // Pick up the object
            if (Input.GetKeyDown(_interactKey))
            {
                PickUp(_colliderTransform);
            }
        }
        
        else if (_isInPressableArea)
        {
            if (Input.GetKeyDown(_interactKey))
            {
                Press();
            }
        }
        
        else if (_isInPushablePositiveArea)
        {
            if (Input.GetKeyDown(_pushKey) || Input.GetKeyDown(_pullKey))
            {
                SetMovableObjectAsParent(_colliderTransform.transform);
                _timePushed = Time.time;
                _holdingPushLayerWeight = 1;
                SetLayerWeightServerRpc(_holdingLayer, _holdingPushLayerWeight);
            }
            if (Input.GetKeyUp(_pushKey) || Input.GetKeyUp(_pullKey))
            {
                RemovePlayerParentServerRpc();
                _isObjectPushedorPulled = false;
                _timePushed = Time.time;
                _holdingPushLayerWeight = 0;
                SetLayerWeightServerRpc(_holdingLayer, _holdingPushLayerWeight);
            }

            float deltaTime = Time.time - _timePushed;
            if (Input.GetKey(_pushKey) || Input.GetKey(_pullKey))
            {
                if (deltaTime >= timeToMoveObjects / _playerController.strength)
                {
                    // _timePushed = Time.time + pushCooldownTime;
                    PushAndPullBehavior("Positive");
                }
            }
        }
        
        else if (_isInPushableNegativeArea)
        {
            if (Input.GetKeyDown(_pushKey) || Input.GetKeyDown(_pullKey))
            {
                SetMovableObjectAsParent(_colliderTransform.transform);
                _timePushed = Time.time;
                _holdingPushLayerWeight = 1;
                SetLayerWeightServerRpc(_holdingLayer, _holdingPushLayerWeight);
            }
            if (Input.GetKeyUp(_pushKey) || Input.GetKeyUp(_pullKey))
            {
                RemovePlayerParentServerRpc();
                _isObjectPushedorPulled = false;
                _timePushed = Time.time;
                _holdingPushLayerWeight = 0;
                SetLayerWeightServerRpc(_holdingLayer, _holdingPushLayerWeight);
            }

            float deltaTime = Time.time - _timePushed;
            if (Input.GetKey(_pushKey) || Input.GetKey(_pullKey))
            {
                if (deltaTime >= timeToMoveObjects / _playerController.strength)
                {
                    // _timePushed = Time.time + pushCooldownTime;
                    PushAndPullBehavior("Negative");
                }
            }
        }
        else
        {
            if (_isObjectPushedorPulled)
            {
                _isObjectPushedorPulled = false;
                RemovePlayerParentServerRpc();
                _holdingPushLayerWeight = 0;
                SetLayerWeightServerRpc(_holdingLayer, _holdingPushLayerWeight);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(ClashArenaController.ObjectType.Pickable.ToString()))
        {
            _colliderTransform = other.transform;
            SetFunctionalityAreaAvailability(true, false, false, false);
        }
        else if (other.CompareTag(ClashArenaController.ObjectType.Pressable.ToString()))
        {
            SetFunctionalityAreaAvailability(false, true, false, false);
        }
        else if (other.CompareTag(ClashArenaController.ObjectType.Pushable.ToString() + "PositiveEdge"))
        {
            _colliderTransform = other.transform.parent.parent;
            _objectMovingPointTransform = other.transform.GetChild(0);
            SetFunctionalityAreaAvailability(false, false, true, false);
        }
        else if (other.CompareTag(ClashArenaController.ObjectType.Pushable.ToString() + "NegativeEdge"))
        {
            _colliderTransform = other.transform.parent.parent;
            _objectMovingPointTransform = other.transform.GetChild(0);
            SetFunctionalityAreaAvailability(false, false, false, true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        SetFunctionalityAreaAvailability(false, false, false, false);
    }

    private void SetFunctionalityAreaAvailability(bool isInPickableArea, bool isInPressableArea, bool isInPushablePositiveArea, bool isInPushableNegativeArea)
    {
        _isInPickableArea = isInPickableArea;
        _isInPressableArea = isInPressableArea;
        _isInPushablePositiveArea = isInPushablePositiveArea;
        _isInPushableNegativeArea = isInPushableNegativeArea;
    }

    private void PushAndPullBehavior(String edgeSide)
    {
        if (Input.GetKey(_pushKey))
        {
            _isObjectPushedorPulled = true;
            _moveObjectDirection = edgeSide == "Positive" ? -1 : 1;
        }
        if (Input.GetKey(_pullKey))
        {
            _moveObjectDirection = edgeSide == "Positive" ? 1 : -1;
            _isObjectPushedorPulled = true;
        }
        // _holdingPushLayerWeight = 0;
        // SetLayerWeightServerRpc(_holdingLayer, _holdingPushLayerWeight);
        // StopAllCoroutines();
    }
    
    
    
    private void PushAndPull(Transform objectTransform)
    {
        MovableObject movableObject = objectTransform.GetComponent<MovableObject>();
        movableObject.Move(_moveObjectDirection);
    }

    public void SetMovableObjectAsParent(Transform movableObjectParent) {
        SetMovableObjectAsParentServerRpc(movableObjectParent.GetComponent<NetworkObject>());
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetMovableObjectAsParentServerRpc(NetworkObjectReference movableObjectParentNetworkObjectReference) {
        SetMovableObjectAsParentClientRpc(movableObjectParentNetworkObjectReference);
    }
    
    [ClientRpc]
    private void SetMovableObjectAsParentClientRpc(NetworkObjectReference movableObjectParentNetworkObjectReference) {
        movableObjectParentNetworkObjectReference.TryGet(out NetworkObject movableObjectParentNetworkObject);
    
        _followTransform.SetTargetTransform(_objectMovingPointTransform);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RemovePlayerParentServerRpc() {
        RemovePlayerParentClientRpc();
    }
    
    [ClientRpc]
    private void RemovePlayerParentClientRpc() {
        _followTransform.SetIsFollowing(false);
    }
    
    
    private void PickUp(Transform objectGeneratorTransform)
    {
        if(_isObjectPickedUp)
            return;
        _isObjectPickedUp = true;
        PickableObjectGenerator pickableObject = objectGeneratorTransform.GetComponent<PickableObjectGenerator>();
        pickableObject.Pick();
        _holdingPickLayerWeight = 1;
        SetLayerWeightServerRpc(_pickingLayer, _holdingPickLayerWeight);
        
    }
    
    // public void Pull(Transform objectTransform)
    // {
    //     MovableObject movableObject = objectTransform.GetComponent<MovableObject>();
    //     movableObject.Move(_moveObjectDirection);
    // }

    public void Press()
    {
        PressServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PressServerRpc()
    {
        StartCoroutine(PressCR());
    }

    private IEnumerator PressCR()
    {
        // Set the layer weight on the server
        _pressingLayerWeight = 1;
        SetLayerWeightServerRpc(_pressingLayer, _pressingLayerWeight);
        PlayAnimationServerRpc(_animator.GetCurrentAnimatorClipInfo(_pressingLayer)[0].clip.name, _pressingLayer, 0f);
        yield return new WaitForSeconds(.8f);
        // Set the layer weight on the server
        _pressingLayerWeight = 0;
        SetLayerWeightServerRpc(_pressingLayer, _pressingLayerWeight);
    }

    // public void PickUp(Transform objectTransform)
    // {
    //     _holdingPickLayerWeight = 1;
    //     SetLayerWeightServerRpc(_holdingLayer, _holdingPickLayerWeight);
    //     // _animator.SetLayerWeight(_holdingLayer, 1); // Activate the layer
    //     _pickedObjectTransform = objectTransform;
    //     objectTransform.parent = transform;
    //     objectTransform.GetComponent<Rigidbody>().isKinematic = true;
    //     objectTransform.localPosition = new Vector3(0, 0.28f, 0.55f);
    //     objectTransform.localRotation = Quaternion.identity;
    //     _isObjectPickedUp = true;
    // }
    //
    // public void DropDown()
    // {
    //     _pickedObjectTransform.parent = mainMapTransform;
    //     _holdingPickLayerWeight = 0;
    //     SetLayerWeightServerRpc(_holdingLayer, _holdingPickLayerWeight);
    //     // _animator.SetLayerWeight(_holdingLayer, 0); // Deactivate the layer
    //     _pickedObjectTransform.GetComponent<Rigidbody>().isKinematic = false;
    //     _isObjectPickedUp = false;   
    // }

    
    [ServerRpc(RequireOwnership = false)]
    private void PlayAnimationServerRpc(string animationName, int layerIndex, float normalizedTime)
    {
        PlayAnimationClientRpc(animationName, layerIndex, normalizedTime);
    }

    [ClientRpc]
    private void PlayAnimationClientRpc(string animationName, int layerIndex, float normalizedTime)
    {
        // Play the animation on all clients
        _animator.Play(animationName, layerIndex, normalizedTime);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetLayerWeightServerRpc(int layer, float value)
    {
        SetLayerWeightClientRpc(layer, value);
    }

    [ClientRpc]
    private void SetLayerWeightClientRpc(int layer, float value)
    {
        _animator.SetLayerWeight(layer, value);
    }
}
