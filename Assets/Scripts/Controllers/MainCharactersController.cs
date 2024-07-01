using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum CharacterType
{
    Player,
    Mangiagalli,
    Golgi,
}

public class MainCharactersController : MonoBehaviour
{
    [SerializeField] private Sprite playerSprite;
    [SerializeField] private Sprite mangiagalliSprite;
    [SerializeField] private Sprite golgiSprite;

    public Sprite GetCharacterSprite(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.Player:
                return playerSprite;
            case CharacterType.Mangiagalli:
                return mangiagalliSprite;
            case CharacterType.Golgi:
                return golgiSprite;
            default:
                return playerSprite;
        }
    }
}
