using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class KeyBindingHelperUI : MonoBehaviour
{
    [SerializeField] private TMP_Text helperText;
    
    private static readonly Dictionary<KeyBindingType, string> keyBindingsHelper = new Dictionary<KeyBindingType, string>
    {
        { KeyBindingType.DeliveryArea, "Press F to Deliver the Box" },
        { KeyBindingType.PickableArea, "Press F to Pick a Box" },
        { KeyBindingType.BlockButtonArea, "Press F to Interact" },
        { KeyBindingType.ConveyorButtonArea, "Press F to Interact" },
        { KeyBindingType.PushAndPullArea, "Hold J to Push\n" +
                                          "Hold K to Pull" },
        { KeyBindingType.DropBox, "Press F to Drop the Box" },
    };

    private void Awake()
    {
        Hide();
    }

    public void Show(KeyBindingType type)
    {
        if (keyBindingsHelper.TryGetValue(type, out string helperTextValue))
        {
            helperText.text = helperTextValue;
            gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("KeyBindingType not found in dictionary");
        }
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
