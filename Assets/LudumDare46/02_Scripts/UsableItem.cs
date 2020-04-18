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
    public float percentageFull;
    public GameObject visualElement;

    protected override void Start()
    {
        base.Start();
        visualElement.SetActive(percentageFull > 0);
    }

    public void UseItem(int amountToAdd)
    {
        Debug.Log("USSSSEEEE");
        if (percentageFull > 0)
        {
            visualElement.SetActive(percentageFull > 0);
            percentageFull -= amountToAdd;
            Debug.Log("used item");
        }
    }

    public void AddFromSurface(UseType typeToAdd, int amountToAdd)
    {
        if (percentageFull <100 && typeToAdd == canGetFromSurface)
        { 
            visualElement.SetActive(percentageFull > 0);
            percentageFull += amountToAdd;
        }
    }
}
