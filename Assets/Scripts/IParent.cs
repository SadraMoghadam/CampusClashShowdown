using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IParent<T>
{
    public Transform GetChildFollowTransform();

    public void SetChild(T child);

    public T GetChild();

    public void ClearChild();

    public bool HasChild();

    public NetworkObject GetNetworkObject();

    public Color GetTeamColor();
    
    public int GetTeamId();
    
    public TeamCharacteristicsScriptableObject GetTeamCharacteristics();
}
