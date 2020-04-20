using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArms : MonoBehaviour
{
    private PickableItem itemAvailable;
    private PlaceableSurface placebaleSurfaceAvailable;
    private SpawnableSurface spawnableSurfaceAvailable;

    public delegate void PlayerArmsDelegate();
    public PlayerArmsDelegate onPickUpDelegate;
    public PlayerArmsDelegate onDropDelegate;

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
            if (!placebaleSurfaceAvailable && itemAvailable && !itemAvailable.itemInUse)// pick up item
            {
                itemInHand = itemAvailable;
                itemAvailable = null;
                itemInHand.PlaceItemAtLocation(transform);
                onPickUpDelegate?.Invoke();
            }
            else if (placebaleSurfaceAvailable)
            {
                if (itemInHand && !placebaleSurfaceAvailable.itemOnSurface)// place item on surface
                {
                    placebaleSurfaceAvailable.PlaceItemOnMe(ItemInHand);
                    itemInHand.DropItem(placebaleSurfaceAvailable);
                    ItemInHand = null;
                }
                else if (placebaleSurfaceAvailable.itemOnSurface && !itemInHand)// take item from surface
                {
                    ItemInHand = placebaleSurfaceAvailable.TakeItemFromMe();
                    itemAvailable = null;
                    itemInHand.PlaceItemAtLocation(transform);
                }
            }
            else if (spawnableSurfaceAvailable && ItemInHand == null)// spawn and get item from surface
            {
                ItemInHand = spawnableSurfaceAvailable.GetObjectFromSurface();
                if (itemInHand)// check to see if we got an item
                {
                    itemInHand.PlaceItemAtLocation(transform);
                }
            }
            else if (itemInHand)// drop item
            {
                itemInHand.DropItem(null);
                ItemInHand = null;
                onDropDelegate?.Invoke();
            }
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            if(itemInHand && itemInHand is UsableItem)
            {
                UsableItem item = itemInHand as UsableItem;
                if (item.percentageFull > 0 && !item.autoStart)
                {
                    item.StartUse();
                }
            }
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            if (itemInHand && itemInHand is UsableItem)
            {
                UsableItem item = itemInHand as UsableItem;
                if (!item.autoStart)
                {
                    item.StopUse();
                }
            }
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        PickableItem item = other.GetComponent<PickableItem>();
        PlaceableSurface placeableSurface = other.GetComponent<PlaceableSurface>();
        SpawnableSurface spawnableSurface = other.GetComponent<SpawnableSurface>();

        if (item && !ItemInHand)
        {
            itemAvailable = item;
        }

        if (placeableSurface)
        {
            placebaleSurfaceAvailable = placeableSurface;
        }

        if (spawnableSurface)
        {
            spawnableSurfaceAvailable = spawnableSurface;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PickableItem item = other.GetComponent<PickableItem>();
        PlaceableSurface placeableSurface = other.GetComponent<PlaceableSurface>();
        SpawnableSurface spawnableSurface = other.GetComponent<SpawnableSurface>();

        if (item && !ItemInHand)
        {
            itemAvailable = null;
        }

        if (placeableSurface)
        {
            placebaleSurfaceAvailable = null;
        }

        if (spawnableSurface)
        {
            spawnableSurfaceAvailable = null;
        }
    }
}
