using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionFunctionalities : MonoBehaviour
{
    
    [SerializeField] private float distanceToInteract = 0.2f;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private Transform mainMapTransform;
    private int _holdingLayer = 1;
    private int _pressingLayer = 2;
    private PlayerController _playerController;
    private KeyCode _interactKey;
    private Transform _pickedObjectTransform;
    private bool _isObjectPickedUp = false;
    private Animator _animator;

    private void Start()
    {
        if (mainMapTransform == null)
        {
            mainMapTransform = GameObject.Find("Main Map").transform;
        }
        _playerController = ClashArenaController.Instance.playerController;
        
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
        Interact();
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
            if (Input.GetKeyDown(_interactKey))
            {
                DropDown();
            }
        }
        else if (Physics.Raycast(transform.position, transform.forward + Vector3.up, out hit, distanceToInteract, layerMaskInteract))
        {
            // Debug.Log(hit.collider.tag);

            Transform colliderTransform = hit.collider.transform;
            if (hit.collider.CompareTag(ClashArenaController.ObjectType.Pickable.ToString()))
            {
                // Pick up the object
                if (Input.GetKeyDown(_interactKey))
                {
                    PickUp(colliderTransform);
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
                    _animator.SetLayerWeight(_holdingLayer, 1);
                    Push(colliderTransform);
                }
                else
                {
                    _animator.SetLayerWeight(_holdingLayer, 0);
                }
            }
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
        StartCoroutine(PressCR());
    }

    private IEnumerator PressCR()
    {
        _animator.SetLayerWeight(_pressingLayer, 1);
        _animator.Play(_animator.GetCurrentAnimatorClipInfo(_pressingLayer)[0].clip.name, _pressingLayer, 0f);
        yield return new WaitForSeconds(.8f);
        _animator.SetLayerWeight(_pressingLayer, 0);
    }
    
    public void PickUp(Transform objectTransform)
    {
        _animator.SetLayerWeight(_holdingLayer, 1); // Activate the layer
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
        _animator.SetLayerWeight(_holdingLayer, 0); // Deactivate the layer
        _pickedObjectTransform.GetComponent<Rigidbody>().isKinematic = false;
        _isObjectPickedUp = false;   
    }
}
