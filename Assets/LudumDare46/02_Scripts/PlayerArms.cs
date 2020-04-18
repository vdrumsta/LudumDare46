using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArms : MonoBehaviour
{
    public bool hasItemInHand;

    private PickableItem itemAvailable;
    private PickableItem itemInHand;
    public PickableItem ItemInHand
    {
        get
        {
            return itemInHand;
        }

        private set
        {
            itemInHand = value;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (itemAvailable)
            {
                ItemInHand = itemAvailable;
                itemAvailable = null;
                itemInHand.PlaceItemAtLocation(transform);
                Debug.Log("Picked up the " + ItemInHand.name);
            }
            else if (itemInHand)
            {
                itemInHand.DropItem();
                ItemInHand = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PickableItem item = other.GetComponent<PickableItem>();
        if (item && !ItemInHand)
        {
            Debug.Log("Can pick up " + item.name);
            itemAvailable = item;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PickableItem item = other.GetComponent<PickableItem>();
        if (item && !ItemInHand)
        {
            itemAvailable = null;
        }
    }
}
