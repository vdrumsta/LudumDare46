using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public bool stayStill;

    private Rigidbody itemRigidbody;
    private GameObject objectPlacedOn;

    // Start is called before the first frame update
    void Start()
    {
        itemRigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stayStill)
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
        //shoudl stay static and attack to a parent object
        stayStill = true;
        transform.parent = placedOnObject;
        transform.position = placedOnObject.position;
        transform.rotation = placedOnObject.rotation;
    }

    public void DropItem()
    {
        // should turn on physics and roll around
        transform.parent = null;
        stayStill = false;
    }
}
