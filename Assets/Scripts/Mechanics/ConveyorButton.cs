using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorButton : MonoBehaviour
{
    public void ConveyorButtonBehavior()
    {
        List<ObjectDelivery> deliveryObjects = FindObjectsOfType<ObjectDelivery>().ToList();
        foreach (var deliveryObject in deliveryObjects)
        {
            deliveryObject.SetMovement();
        }
    }
}
