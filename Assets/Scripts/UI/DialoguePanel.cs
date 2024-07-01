using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogue;
    [SerializeField] private TMP_Text nextButtonText;
    [SerializeField] private Button previousButton;
    [SerializeField] private Image characterImage;
    [SerializeField] private TMP_Text characterName;


    public void Setup(string dialogueText, bool disableNext = false, bool disablePrev = false, CharacterType characterType = CharacterType.Player)
    {
        // if (!Cursor.visible)
        // {
        //     GameController _gameController = GameController.Instance;
        //     _gameController.DisableLook();
        //     _gameController.DisableAllKeys();
        //     _gameController.ShowCursor();
        // }
        // dialogue.text = dialogueText;
        StartCoroutine(ShowDialogueText(dialogueText));
        nextButtonText.text = disableNext ? "Finish" : "Next";
        previousButton.interactable = !disablePrev;
        characterImage.sprite = CampusController.Instance.MainCharactersController.GetCharacterSprite(characterType);
        characterName.text = characterType.ToString();
    }
    
    IEnumerator ShowDialogueText(string fullText){
        for(int i = 0; i < fullText.Length + 1; i++){
            dialogue.text = fullText.Substring(0,i);
            yield return new WaitForSeconds(.01f);
        }
    }
}
