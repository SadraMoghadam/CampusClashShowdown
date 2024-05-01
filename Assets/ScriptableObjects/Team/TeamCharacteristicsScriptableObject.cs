using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeamCharacteristics", menuName = "Team/Team Characteristics")]
public class TeamCharacteristicsScriptableObject : ScriptableObject
{
    public string name;
    public Color color;
    public int maxPlayers;
}
