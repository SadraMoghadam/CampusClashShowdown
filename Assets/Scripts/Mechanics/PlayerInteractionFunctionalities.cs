using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractionFunctionalities : NetworkBehaviour
{
    
    [SerializeField] private float distanceToInteract = 0.2f;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private Transform mainMapTransform;
    private int _holdingLayer = 1;
    private int _pressingLayer = 2;
    private int _holdingPushLayerWeight = 0;
    private int _holdingPickLayerWeight = 0;
    private int _pressingLayerWeight = 0;
    private PlayerController _playerController;
    private KeyCode _interactKey;
    private Transform _pickedObjectTransform;
    private bool _isObjectPickedUp = false;
    private bool _isObjectPushed = false;
    private Animator _animator;
    private Transform _colliderTransform;

    private void Start()
    {
        if (mainMapTransform == null)
        {
            mainMapTransform = GameObject.Find("Main Map").transform;
        }
    }

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
        }
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (IsLocalPlayer)
        {
            Interact();
        }
    }

    private void FixedUpdate()
    {

        if (IsLocalPlayer)
        {
            if (_isObjectPushed)
            {
                Push(_colliderTransform);
            }
            else if (_holdingPushLayerWeight == 0 && !_isObjectPickedUp)
            {
                SetLayerWeightServerRpc(_holdingLayer, _holdingPushLayerWeight);
            }
        }
    }


    // ReSharper disable Unity.PerformanceAnalysis
    private void Interact()
    {
        RaycastHit hit;
        Debug.DrawLine(transform.position + Vector3.up, transform.position + transform.forward * distanceToInteract + Vector3.up);
        if (_isObjectPickedUp)
        {
            // Drop the object
            // _interactionFunctionalities.DropDown(_animator, hit.transform);
            
            _holdingPushLayerWeight = 1;
            if (Input.GetKeyDown(_interactKey))
            {
                DropDown();
            }
        }
        else if (Physics.Raycast(transform.position, transform.forward + Vector3.up, out hit, distanceToInteract, layerMaskInteract))
        {
            // Debug.Log(hit.collider.tag);

            _colliderTransform = hit.collider.transform;
            if (hit.collider.CompareTag(ClashArenaController.ObjectType.Pickable.ToString()))
            {
                // Pick up the object
                if (Input.GetKeyDown(_interactKey))
                {
                    PickUp(_colliderTransform);
                }
            }
            else if (hit.collider.CompareTag(ClashArenaController.ObjectType.Interactable.ToString()))
            {
                if (Input.GetKeyDown(_interactKey))
                {
                    Press();
                }
            }
            else if (hit.collider.CompareTag(ClashArenaController.ObjectType.Pushable.ToString()))
            {
                if (Input.GetKey(_interactKey))
                {
                    // _animator.SetLayerWeight(_holdingLayer, 1);
                    _holdingPushLayerWeight = 1;
                    _isObjectPushed = true;
                    // Push(_colliderTransform);
                }
                else
                {
                    // _animator.SetLayerWeight(_holdingLayer, 0);
                    _holdingPushLayerWeight = 0;
                    _isObjectPushed = false;
                }
                SetLayerWeightServerRpc(_holdingLayer, _holdingPushLayerWeight);
            }
        }
        else
        {
            _holdingPushLayerWeight = 0;
            _isObjectPushed = false;
        }
    }
    
    
    public void Push(Transform objectTransform)
    {
        MovableObject movableObject = objectTransform.GetComponent<MovableObject>();
        movableObject.Move(1);
        
    }
    
    public void Pull(Transform objectTransform)
    {
        
    }

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

    public void PickUp(Transform objectTransform)
    {
        _holdingPickLayerWeight = 1;
        SetLayerWeightServerRpc(_holdingLayer, _holdingPickLayerWeight);
        // _animator.SetLayerWeight(_holdingLayer, 1); // Activate the layer
        _pickedObjectTransform = objectTransform;
        objectTransform.parent = transform;
        objectTransform.GetComponent<Rigidbody>().isKinematic = true;
        objectTransform.localPosition = new Vector3(0, 0.28f, 0.55f);
        objectTransform.localRotation = Quaternion.identity;
        _isObjectPickedUp = true;
    }
    
    public void DropDown()
    {
        _pickedObjectTransform.parent = mainMapTransform;
        _holdingPickLayerWeight = 0;
        SetLayerWeightServerRpc(_holdingLayer, _holdingPickLayerWeight);
        // _animator.SetLayerWeight(_holdingLayer, 0); // Deactivate the layer
        _pickedObjectTransform.GetComponent<Rigidbody>().isKinematic = false;
        _isObjectPickedUp = false;   
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
