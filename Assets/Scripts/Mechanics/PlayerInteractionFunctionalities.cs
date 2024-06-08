using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerInteractionFunctionalities : NetworkBehaviour
{
    [SerializeField] private float distanceToInteract = 0.2f;
    [SerializeField] private float timeToMoveObjects = 1f;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private float pushCooldownTime = 2f;
    [SerializeField] private float conveyorBeltStopButtonCoolDown = 5;
    [SerializeField] private PlayerUI playerUI;
    private bool _isInPickableArea = false;
    private bool _isInPushablePositiveArea = false;
    private bool _isInPushableNegativeArea = false;
    private bool _isInBlockButtonArea = false;
    private bool _isInConveyorButtonArea = false;
    private bool _isInDeliveryArea = false;
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
    private MultiplayerController _multiplayerController;
    private ClashArenaController _clashArenaController;
    private ClashSceneUI _clashSceneUI;
    // private NetworkVariable<float> _conveyorButtonCooldownTimer = new NetworkVariable<float>(0f);


    public override void OnNetworkSpawn()
    {
        _playerController = GetComponent<PlayerController>();
        // _conveyorButtonCooldownTimer = new NetworkVariable<float>(0f);
        
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
        _clashArenaController = ClashArenaController.Instance;
        _clashSceneUI = ClashSceneUI.Instance;
        _isObjectPickedUp = false;
        _multiplayerController = MultiplayerController.Instance;
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
                    PickUp();
                }
            }
            else if (hit.collider.CompareTag(ClashArenaController.ObjectType.BlockButton.ToString()))
            {
                _colliderTransform = hit.collider.transform.parent;
                if (Input.GetKeyDown(_interactKey))
                {
                    Press();
                    _colliderTransform.GetComponent<BlockButton>().BlockButtonBehavior();
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
        HideKeyBindingHelper();
        HideAllKeyBindings();
        
        if (_isObjectPickedUp)
        {
            if (_isInDeliveryArea)
            {
                ShowKeyBindingHelperByType(KeyBindingType.DeliveryArea);
                ShowKeyBindingByType(KeyBindingType.DeliveryArea);
                // Drop the object
                if (Input.GetKeyDown(_interactKey))
                {
                    DropDown();
                }
            }
            else
            {
                ShowKeyBindingHelperByType(KeyBindingType.DropBox);
                // Drop and destroy the object
                if (Input.GetKeyDown(_interactKey))
                {
                    DropDownAndDestroy();
                }
            }
        }
        else
        {
            if (_isInPickableArea)
            {
                ShowKeyBindingHelperByType(KeyBindingType.PickableArea);
                ShowKeyBindingByType(KeyBindingType.PickableArea);
                // Pick up the object
                if (Input.GetKeyDown(_interactKey))
                {
                    PickUp();
                }
            }
            
            else if (_isInBlockButtonArea)
            {
                if (_colliderTransform.GetComponent<BlockButton>().isAbleToPress)
                {
                    ShowKeyBindingHelperByType(KeyBindingType.BlockButtonArea);
                    ShowKeyBindingByType(KeyBindingType.BlockButtonArea);
                    if (Input.GetKeyDown(_interactKey))
                    {
                        Press();
                        _colliderTransform.GetComponent<BlockButton>().BlockButtonBehavior();
                    }
                }
            }
            
            else if (_isInConveyorButtonArea)
            {
                if (_clashSceneUI.isAbleToPress)
                {
                    ShowKeyBindingHelperByType(KeyBindingType.ConveyorButtonArea);
                    ShowKeyBindingByType(KeyBindingType.ConveyorButtonArea);
                    if (Input.GetKeyDown(_interactKey))
                    {
                        Press();
                        _colliderTransform.GetComponent<ConveyorButton>().ConveyorButtonBehavior();
                        _multiplayerController.conveyorButtonCooldownTimer = 0;
                        SetConveyorStopCooldownServerRpc(conveyorBeltStopButtonCoolDown);
                    }   
                }
            }
            
            else if (_isInPushablePositiveArea)
            {
                MovableObject movableObject = _colliderTransform.GetComponent<MovableObject>();
                ShowKeyBindingByType(KeyBindingType.PushAndPullArea, movableObject.id);
                ShowKeyBindingHelperByType(KeyBindingType.PushAndPullArea);
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
                    if(deltaTime <= 0.01f)
                        playerUI.DisablePushOrPullTimerSlider();
                    else
                        playerUI.SetPushOrPullTimer(deltaTime / (timeToMoveObjects / _playerController.strength));
                    if (deltaTime >= timeToMoveObjects / _playerController.strength)
                    {
                        // _timePushed = Time.time + pushCooldownTime;
                        PushAndPullBehavior("Positive");
                    }
                }
            }
            
            else if (_isInPushableNegativeArea)
            {
                MovableObject movableObject = _colliderTransform.GetComponent<MovableObject>();
                ShowKeyBindingByType(KeyBindingType.PushAndPullArea, movableObject.id);
                ShowKeyBindingHelperByType(KeyBindingType.PushAndPullArea);
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
                    playerUI.DisablePushOrPullTimerSlider();
                }
    
                float deltaTime = Time.time - _timePushed;
                if (Input.GetKey(_pushKey) || Input.GetKey(_pullKey))
                {
                    if(deltaTime <= 0.01f)
                        playerUI.DisablePushOrPullTimerSlider();
                    else
                        playerUI.SetPushOrPullTimer(deltaTime / (timeToMoveObjects / _playerController.strength));
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

                if (!_isInPushablePositiveArea && !_isInPushableNegativeArea)
                {
                    playerUI.DisablePushOrPullTimerSlider();
                }
            }   
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(ClashArenaController.ObjectType.Pickable.ToString()))
        {
            _colliderTransform = other.transform;
            SetFunctionalityAreaAvailability(true, false, false, false, false);
        }
        else if (other.CompareTag(ClashArenaController.ObjectType.BlockButton.ToString()))
        {
            _colliderTransform = other.transform.parent;
            SetFunctionalityAreaAvailability(false, true, false, false, false);
        }
        else if (other.CompareTag(ClashArenaController.ObjectType.ConveyorButton.ToString()))
        {
            _colliderTransform = other.transform.parent;
            SetFunctionalityAreaAvailability(false, false, false, false, true);
        }
        else if (other.CompareTag(ClashArenaController.ObjectType.Pushable.ToString() + "PositiveEdge"))
        {
            _colliderTransform = other.transform.parent.parent;
            _objectMovingPointTransform = other.transform.GetChild(0);
            SetFunctionalityAreaAvailability(false, false, true, false, false);
        }
        else if (other.CompareTag(ClashArenaController.ObjectType.Pushable.ToString() + "NegativeEdge"))
        {
            _colliderTransform = other.transform.parent.parent;
            _objectMovingPointTransform = other.transform.GetChild(0);
            SetFunctionalityAreaAvailability(false, false, false, true, false);
        }
        if (other.CompareTag("ObjectEntryArea"))
        {
            _isInDeliveryArea = true;
        }
        else
        {
            _isInDeliveryArea = false;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        SetFunctionalityAreaAvailability(false, false, false, false, false);
    }

    private void SetFunctionalityAreaAvailability(bool isInPickableArea, bool isInBlockButtonArea, bool isInPushablePositiveArea, bool isInPushableNegativeArea, bool isInConveyorButtonArea)
    {
        _isInPickableArea = isInPickableArea;
        _isInBlockButtonArea = isInBlockButtonArea;
        _isInPushablePositiveArea = isInPushablePositiveArea;
        _isInPushableNegativeArea = isInPushableNegativeArea;
        _isInConveyorButtonArea = isInConveyorButtonArea;
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
    
    [ServerRpc(RequireOwnership = false)]
    private void SetConveyorStopCooldownServerRpc(float cooldown)
    {
        SetConveyorStopCooldownClientRpc(cooldown);
    }

    [ClientRpc]
    private void SetConveyorStopCooldownClientRpc(float cooldown)
    {
        // Execute this on all clients except the local one
        GameManager.Instance.AudioManager.Instantplay(SoundName.PressingButton, transform.position);
        _clashSceneUI.SetConveyorStopCooldownSliderValue(cooldown);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetConveyorStopCooldownTimerServerRpc(float value)
    {
        _multiplayerController.conveyorButtonCooldownTimer = value;
        SetConveyorStopCooldownTimerClientRpc(value);
    }

    [ClientRpc]
    private void SetConveyorStopCooldownTimerClientRpc(float value)
    {
        _multiplayerController.conveyorButtonCooldownTimer = value;
    }



    private void PushAndPull(Transform objectTransform)
    {
        MovableObject movableObject = objectTransform.GetComponent<MovableObject>();
        movableObject.Move(_moveObjectDirection, _playerController.GetTeamId());
    }

    public void SetMovableObjectAsParent(Transform movableObjectParent) {
        SetMovableObjectAsParentServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetMovableObjectAsParentServerRpc() {
        SetMovableObjectAsParentClientRpc();
    }
    
    [ClientRpc]
    private void SetMovableObjectAsParentClientRpc() {
    
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
    
    
    private void PickUp()
    {
        if(_isObjectPickedUp || !IsLocalPlayer)
            return;
        _isObjectPickedUp = true;
        // _pickableObject = objectGeneratorTransform.GetComponent<PickableObjectGenerator>();
        // _pickableObject.Pick();
        GameManager.Instance.AudioManager.Instantplay(SoundName.PickBox, transform.position);
        PickableObject.SpawnObject(_playerController);
        _holdingPickLayerWeight = 1;
        SetLayerWeightServerRpc(_pickingLayer, _holdingPickLayerWeight);
        
    }
    
    private void DropDown()
    {
        if(!IsLocalPlayer)
            return;
        _isObjectPickedUp = false;
        GameManager.Instance.AudioManager.Instantplay(SoundName.DropBox, transform.position);
        PickableObject.DestroyObject(_playerController.GetChild());
        _holdingPickLayerWeight = 0;
        SetLayerWeightServerRpc(_pickingLayer, _holdingPickLayerWeight);
        ObjectDelivery.SpawnResourceBoxOnDeliveryPath(_playerController);
    }
    
    
    private void DropDownAndDestroy()
    {
        if(!IsLocalPlayer)
            return;
        _isObjectPickedUp = false;
        GameManager.Instance.AudioManager.Instantplay(SoundName.DropBox, transform.position);
        PickableObject.DestroyObject(_playerController.GetChild());
        _holdingPickLayerWeight = 0;
        SetLayerWeightServerRpc(_pickingLayer, _holdingPickLayerWeight);
    }
    

    private void Press()
    {
        PressServerRpc();
    }

    private void HideAllKeyBindings()
    {
        _clashArenaController.keyBindingUis.ForEach(keyBindingUi => keyBindingUi.Hide());
    }

    private void ShowKeyBindingByType(KeyBindingType type, int id = -1)
    {
        if (id == -1)
        {
            _clashArenaController.keyBindingUis.First(keyBindingUi => keyBindingUi.type == type).Show();   
        }
        else
        {
            _clashArenaController.keyBindingUis.First(keyBindingUi => keyBindingUi.type == type && keyBindingUi.id == id).Show();
        }
    }
    
    private void HideKeyBindingHelper()
    {
        _clashArenaController.keyBindingHelperUi.Hide();
    }

    private void ShowKeyBindingHelperByType(KeyBindingType type)
    {
        _clashArenaController.keyBindingHelperUi.Show(type);
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
