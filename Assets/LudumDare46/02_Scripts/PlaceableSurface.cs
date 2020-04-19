using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableSurface : MonoBehaviour
{
    public Transform positionOfItem;
    public PickableItem itemOnSurface;
    public bool consumesItem;
    
    private Coroutine hasItemOnCoroutine;

    [SerializeField]
    private float fillSpeed;

    public UseType typeSurfaceAdds;
    public UseType typeToConsume;

    private void Update()
    {
        if(itemOnSurface is UsableItem)
        {
            UsableItem item = itemOnSurface as UsableItem;
            
            if (item.percentageFull != 100 && hasItemOnCoroutine == null)
            {
                hasItemOnCoroutine = StartCoroutine(HasItemOnRoutine(item));
            }
            else if (item.percentageFull == 100 && hasItemOnCoroutine != null)
            {
                StopCoroutine(hasItemOnCoroutine);
                hasItemOnCoroutine = null;
            }
        }
        else if (itemOnSurface is ConsumableItem && (itemOnSurface as ConsumableItem).canBeUsedFor == typeToConsume)
        {
            Debug.Log("Consume item");
            Destroy(itemOnSurface.gameObject);
            itemOnSurface = null;
        }
        else if(hasItemOnCoroutine != null)
        {
            StopCoroutine(hasItemOnCoroutine);
            hasItemOnCoroutine = null;
        }
    }

    private IEnumerator HasItemOnRoutine(UsableItem item)
    {
        while (true)
        {
            yield return new WaitForSeconds(fillSpeed);
            item.AddFromSurface(typeSurfaceAdds, 1);
        }
    }

    public void PlaceItemOnMe(PickableItem item)
    {
        itemOnSurface = item;

        // Stop an autoStart item using charge when it's placed 
        if (itemOnSurface is UsableItem)
        {
            UsableItem usableTempItem = itemOnSurface as UsableItem;
            if (usableTempItem.autoStart)
            {
                usableTempItem.StopUse();
            }
        }
    }

    public PickableItem TakeItemFromMe()
    {
        PickableItem tempItem = itemOnSurface;
        itemOnSurface = null;

        // Start using the item if it's autoStart
        if (tempItem is UsableItem)
        {
            UsableItem usableTempItem = tempItem as UsableItem;
            if (usableTempItem.percentageFull > 0 && usableTempItem.autoStart)
            {
                usableTempItem.StartUse();
            }
        }

        return tempItem;
    }
}
