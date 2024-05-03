using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BlockButton : NetworkBehaviour
{
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
        PlayAnimationServerRpc(_blockPickableArea);
        yield return new WaitForSeconds(1);
        BlockArea(_blockPickableArea);
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
