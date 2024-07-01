using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueDataReader : MonoBehaviour
{
    private string fileName = "Data/DialogueData";
    public List<DialogueData> dialogueData;
    
    private void Awake()
    {
        dialogueData = ReadDialoguesData(fileName);
    }

    private List<DialogueData> ReadDialoguesData(string filename)
    {
        int currentLineNumber = 0;
        int columnCount = 0;
        TextAsset txt = Resources.Load<TextAsset>(filename);
        string[] lines = txt.text.Split('\n');
        dialogueData = new List<DialogueData>();
        foreach (string line in lines)
        {

            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            string[] lineSplitted = CSVParser.Split(line);
            currentLineNumber++;
            if (lineSplitted[0] == "")
            {
                continue;
            }

            if (currentLineNumber == 1)
            {
                continue;
            }

            // if (currentLineNumber == 31)
            // {
            //     Debug.Log(currentLineNumber);   
            // }
            int id = int.Parse(lineSplitted[0]);
            string hint = lineSplitted[1];
            int nextId = int.Parse(lineSplitted[2]);
            CharacterType type = (CharacterType)Enum.Parse(typeof(CharacterType), lineSplitted[3]);

            DialogueData data = new DialogueData(id, hint, nextId, type);
            dialogueData.Add(data);
        }

        return dialogueData;
    }

    public DialogueData GetDialogueData(int id)
    {
        return dialogueData.First(i => i.Id == id);
    }
}
