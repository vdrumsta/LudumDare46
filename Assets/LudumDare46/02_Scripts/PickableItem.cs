using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [System.NonSerialized]
    public bool itemInUse;

    private Rigidbody itemRigidbody;
    protected GameObject objectPlacedOn;

    public delegate void ItemDelegate(PickableItem item);
    public ItemDelegate itemDestroyedDelegate;

    private Vector3 spawnPosition;
    public int maxDistanceFromSpawn = 15;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        itemRigidbody = this.GetComponent<Rigidbody>();
        spawnPosition = this.transform.position;
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

        if(Mathf.Abs(Vector3.Distance(transform.position, spawnPosition)) > maxDistanceFromSpawn)
        {
            if(this is ConsumableItem)
            {
                Destroy(this.gameObject);
            }
            else
            {
                transform.position = spawnPosition;
            }
        }
    }

    private void OnDestroy()
    {
        itemDestroyedDelegate?.Invoke(this);
    }

    public virtual void PlaceItemAtLocation(Transform placedOnObject)
    {
        //should stay static and attack to a parent object
        itemInUse = true;
        transform.parent = placedOnObject;
        transform.position = placedOnObject.position;
        transform.rotation = placedOnObject.rotation;
    }

    public virtual void DropItem(PlaceableSurface surface)
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
