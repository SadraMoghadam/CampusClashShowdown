using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAvatarCustomization : MonoBehaviour
{
    [SerializeField] private Button bodyPlusButton;
    [SerializeField] private Button bodyMinusButton;
    [SerializeField] private Button headPlusButton;
    [SerializeField] private Button headMinusButton;
    [SerializeField] private PlayerCustomization playerCustomization;
    // Start is called before the first frame update
    private void Awake()
    {
        bodyPlusButton.onClick.AddListener(() =>
        {
            Debug.Log("pressed Button");
        playerCustomization.ChangeBodyPart(BodyPartType.Body, true);
        });  
        
        bodyMinusButton.onClick.AddListener(() =>
        {
            Debug.Log("pressed Button");
        playerCustomization.ChangeBodyPart(BodyPartType.Body, false);
        });  

        headPlusButton.onClick.AddListener(() =>
        {
            Debug.Log("pressed Head Button");
        playerCustomization.ChangeBodyPart(BodyPartType.Head, true);        
        });
        
        headMinusButton.onClick.AddListener(() =>
        {
             Debug.Log("pressed Head Button");
        playerCustomization.ChangeBodyPart(BodyPartType.Head, false);        
        });
    }

    }
  