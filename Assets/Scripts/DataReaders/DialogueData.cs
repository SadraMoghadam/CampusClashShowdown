using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DialogueData
{
    public int Id;
    public string Dialogue;
    public int NextId;
    public CharacterType CharacterType;

    public DialogueData(int id, string dialogue, int nextId, CharacterType characterType)
    {
        Id = id;
        Dialogue = dialogue;
        NextId = nextId;
        CharacterType = characterType;
    }

    public override string ToString()
    {
        return "dialogue: " + Dialogue;
    }
}
