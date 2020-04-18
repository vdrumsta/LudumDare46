using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArms : MonoBehaviour
{
    private PickableItem itemAvailable;
    private PlaceableSurface surfaceAvailable;
    private Coroutine useItemCoroutine;

    [SerializeField]
    private float useSpeed;

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
            if (itemAvailable && !itemAvailable.itemInUse)// pick up item
            {
                itemInHand = itemAvailable;
                itemAvailable = null;
                itemInHand.PlaceItemAtLocation(transform);
            }
            else if (surfaceAvailable)
            {
                if (itemInHand && !surfaceAvailable.itemOnSurface)// place item on surface
                {
                    surfaceAvailable.PlaceItemOnMe(ItemInHand);
                    itemInHand.DropItem(surfaceAvailable);
                    ItemInHand = null;
                }
                else if (surfaceAvailable.itemOnSurface && !itemInHand)// take item from surface
                {
                    ItemInHand = surfaceAvailable.TakeItemFromMe();
                    itemAvailable = null;
                    itemInHand.PlaceItemAtLocation(transform);
                }
            }
            else if (itemInHand)// drop item
            {
                itemInHand.DropItem(null);
                ItemInHand = null;
            }
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            if(itemInHand && itemInHand is UsableItem)
            {
                UsableItem item = itemInHand as UsableItem;
                if (item.percentageFull > 0)
                {
                    useItemCoroutine = StartCoroutine(UseItemRoutine(item));
                }
            }
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            if (useItemCoroutine != null)
            {
                StopCoroutine(useItemCoroutine);
                useItemCoroutine = null;
            }
        }
    }

    private IEnumerator UseItemRoutine(UsableItem item)
    {
        do
        {
            yield return new WaitForSeconds(useSpeed);
            item.UseItem(1);
            if (item.percentageFull <= 0)
            {
                useItemCoroutine = null;
                break;
            }
            
        } while (true);
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
