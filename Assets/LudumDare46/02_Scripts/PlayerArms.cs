using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArms : MonoBehaviour
{
    private PickableItem itemAvailable;
    private PlaceableSurface surfaceAvailable;

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
            }
            else if (itemInHand)
            {
                itemInHand.DropItem(surfaceAvailable);
                ItemInHand = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PickableItem item = other.GetComponent<PickableItem>();
        PlaceableSurface surface = other.GetComponent<PlaceableSurface>();

        if (item && !ItemInHand)
        {
            itemAvailable = item;
        }

        if (surface)
        {
            Debug.Log("Placeable surface detected");
            surfaceAvailable = surface;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PickableItem item = other.GetComponent<PickableItem>();
        PlaceableSurface surface = other.GetComponent<PlaceableSurface>();

        if (item && !ItemInHand)
        {
            itemAvailable = null;
        }

        if (surface)
        {
            surfaceAvailable = null;
        }
    }
}
