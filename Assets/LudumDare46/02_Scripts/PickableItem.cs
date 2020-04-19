using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [System.NonSerialized]
    public bool itemInUse;

    private Rigidbody itemRigidbody;
    private GameObject objectPlacedOn;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        itemRigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (itemInUse)
        {
            itemRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
        else
        {
            itemRigidbody.constraints = RigidbodyConstraints.None;
        }
    }

    public void PlaceItemAtLocation(Transform placedOnObject)
    {
        //should stay static and attack to a parent object
        itemInUse = true;
        transform.parent = placedOnObject;
        transform.position = placedOnObject.position;
        transform.rotation = placedOnObject.rotation;
    }

    public void DropItem(PlaceableSurface surface)
    {
        // should turn on physics and roll around
        if (surface)
        {
            transform.parent = surface.transform;
            transform.position = surface.positionOfItem.position;
        }
        else
        {
            transform.parent = null;
            itemInUse = false;
        }
    }
}
