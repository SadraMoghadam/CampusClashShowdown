using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerVisuals", menuName = "Player/Player Visuals")]
public class PlayerVisualScriptableObject : ScriptableObject
{
    public Mesh[] HeadMeshes;
    public Mesh[] BodyMeshes;
}
