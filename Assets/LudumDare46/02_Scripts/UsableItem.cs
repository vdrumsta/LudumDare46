using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItem : PickableItem
{
    public enum UseType
    {
        PourWater,
        FillWithWater,

    }

    public UseType canGetFromSurface;
    public UseType canBeUsedFor;
    public bool isFull;
    public GameObject visualElement;

    protected override void Start()
    {
        base.Start();
        visualElement.SetActive(isFull);
    }

    public void UseItem()
    {
        if (isFull)
        {
            isFull = false;
            visualElement.SetActive(isFull);
            Debug.Log("used item");
        }
    }

    public void AddFromSurface(UseType typeToAdd)
    {
        if (!isFull && typeToAdd == canGetFromSurface)
        { 
            isFull = true;
            visualElement.SetActive(isFull);
        }
    }
}
