using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private DialoguePanel dialoguePanel;
    private DialogueData _currentDialogue;
    private int _dialogueStartId;

    private void Awake()
    {
        
        if(dialoguePanel.gameObject.activeSelf)
            dialoguePanel.gameObject.SetActive(false);
    }


    public void Show(int dialogueId)
    {
        // List<int> shownIds = PlayerPrefsManager.GetIntList(PlayerPrefsKeys.ShownDialogues);
        // if(shownIds.Contains(dialogueId))
        //     return;
        _dialogueStartId = dialogueId;
        dialoguePanel.gameObject.SetActive(true);
        FadeDialoguePanelInAndOut(true, .5f);
        SetDialogue(dialogueId);
    }

    public void OnNextClicked()
    {
        if (_currentDialogue.NextId == 0)
        {
            Close();
        }
        else
        {
            SetDialogue(_currentDialogue.NextId);   
        }
    }
    
    
    
    public void OnSkipClicked()
    {
        Close();
    }
    
    public void OnPreviousClicked()
    {
        SetDialogue(_currentDialogue.Id - 1);
    }

    private void Close()
    {
        // List<int> shownDialogueIds = PlayerPrefsManager.GetIntList(PlayerPrefsKeys.ShownDialogues);
        // shownDialogueIds.Add(_dialogueStartId);
        // PlayerPrefsManager.SetIntList(PlayerPrefsKeys.ShownDialogues, shownDialogueIds);
        // dialoguePanel.gameObject.SetActive(false);
        StartCoroutine(DisableDialoguePanel(.5f));
        FadeDialoguePanelInAndOut(false, .5f);
    }

    private void SetDialogue(int id)
    {
        // if(_campusController == null)
        //     _campusController = GameController.Instance;
        _currentDialogue = CampusController.Instance.DialogueDataReader.GetDialogueData(id);
        dialoguePanel.Setup(_currentDialogue.Dialogue, _currentDialogue.NextId == 0, _currentDialogue.Id == _dialogueStartId, _currentDialogue.CharacterType);
    }

    private void FadeDialoguePanelInAndOut(bool fadeIn, float duration = 1)
    {
        Transform dialoguePanelBackground = dialoguePanel.transform.GetChild(0);
        Transform dialoguePanelChildrenContainer = dialoguePanel.transform.GetChild(1);
        
        StartCoroutine(Utilities.FadeInAndOutCR(dialoguePanelBackground.gameObject, fadeIn, duration, 0));
        StartCoroutine(Utilities.FadeInAndOutCR(dialoguePanelChildrenContainer.gameObject, fadeIn, duration, .5f));
        List<Transform> allChildren = GetAllChildren(dialoguePanelChildrenContainer);
        for (int i = 0; i < allChildren.Count; i++)
        {
            if (fadeIn == true && (i == 7))
            {
                StartCoroutine(Utilities.FadeInAndOutCR(
                    allChildren[i].gameObject, fadeIn, duration, .5f));
                continue;  
            } 
            StartCoroutine(Utilities.FadeInAndOutCR(
                allChildren[i].gameObject, fadeIn, duration));
        }
    }
    
    public List<Transform> GetAllChildren(Transform parent)
    {
        List<Transform> allChildren = new List<Transform>();
        GetChildrenRecursive(parent, allChildren);
        return allChildren;
    }

    private void GetChildrenRecursive(Transform parent, List<Transform> allChildren)
    {
        foreach (Transform child in parent)
        {
            allChildren.Add(child);
            GetChildrenRecursive(child, allChildren);
        }
    }

    private IEnumerator DisableDialoguePanel(float duration)
    {
        yield return new WaitForSeconds(duration);
        dialoguePanel.gameObject.SetActive(false);
        // if (!_campusController.IsInLockView)
        // {
        //     _campusController.EnableLook();
        //     _campusController.EnableAllKeys();
        //     _campusController.HideCursor();   
        // }
    }
    
    public bool IsPanelActive()
    {
        return dialoguePanel.gameObject.activeSelf;
    }
}
