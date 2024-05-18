using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour {


    [SerializeField] private SkinnedMeshRenderer headMeshRenderer;
    [SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;


    private Mesh _headMesh;
    private Mesh _bodyMesh;

    private void Awake() 
    {
        // SetPlayerMesh(MultiplayerController.Instance.GetPlayerHeadMesh(0),
        //     MultiplayerController.Instance.GetPlayerBodyMesh(0));
    }

    public void SetPlayerMesh(Mesh headMesh, Mesh bodyMesh)
    {
        headMeshRenderer.sharedMesh = headMesh;
        bodyMeshRenderer.sharedMesh = bodyMesh;
    }

}