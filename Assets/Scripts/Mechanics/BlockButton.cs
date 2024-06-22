using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BlockButton : NetworkBehaviour
{
    public bool isAbleToPress;
    [SerializeField] private Animator blockFencesAnimator;
    [SerializeField] private GameObject pickableArea;
    [SerializeField] private GameObject objectDeliveryArea;
    private bool _blockPickableArea;

    // private static BlockButton _instance;
    // public static BlockButton Instance => _instance;
    private void Awake()
    {
        // if (_instance == null)
        // {
        //     _instance = this;
        // }
        _blockPickableArea = false;
        isAbleToPress = true;
        BlockArea(_blockPickableArea);
    }

    // if not blockPickableArea => blockObjectDeliveryArea
    private void BlockArea(bool blockPickableArea)
    {
        pickableArea.SetActive(!blockPickableArea);
        objectDeliveryArea.SetActive(blockPickableArea);
    }
    
    public void BlockButtonBehavior()
    {
        PressServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PressServerRpc()
    {
        PressClientRpc();
    }

    [ClientRpc]
    private void PressClientRpc()
    {
        StartCoroutine(PressCR());
    }

    private IEnumerator PressCR()
    {
        _blockPickableArea = !_blockPickableArea;
        isAbleToPress = false;
        GameManager.Instance.AudioManager.Instantplay(SoundName.PressingButton, transform.position);
        GameManager.Instance.AudioManager.Instantplay(SoundName.GatesMovement, transform.position);
        yield return new WaitForSeconds(.05f);
        GameManager.Instance.AudioManager.Instantplay(SoundName.GatesMovement, transform.position);
        PlayAnimationServerRpc(_blockPickableArea);
        yield return new WaitForSeconds(.4f);
        BlockArea(_blockPickableArea);
        yield return new WaitForSeconds(.5f);
        isAbleToPress = true;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void PlayAnimationServerRpc(bool blockPickableArea)
    {
        PlayAnimationClientRpc(blockPickableArea);
    }

    [ClientRpc]
    private void PlayAnimationClientRpc(bool blockPickableArea)
    {
        // Play the animation on all clients
        if (blockPickableArea)
        {
            blockFencesAnimator.Play("BlockPickableArea");   
        }
        else
        {
            blockFencesAnimator.Play("BlockDeliveryArea");
        }
    }
}
