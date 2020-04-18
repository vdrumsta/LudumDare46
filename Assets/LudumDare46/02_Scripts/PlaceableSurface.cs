using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableSurface : MonoBehaviour
{
    public Transform positionOfItem;
    public PickableItem itemOnSurface;
    private Coroutine hasItemOnCoroutine;

    [SerializeField]
    private float fillSpeed;

    public UsableItem.UseType typeSurfaceAdds;

    private void Update()
    {
        if(itemOnSurface is UsableItem)
        {
            UsableItem item = itemOnSurface as UsableItem;
            if (item.percentageFull != 100 && hasItemOnCoroutine == null)
            {
                hasItemOnCoroutine = StartCoroutine(HasItemOnRoutine(item));
            }
            else if(item.percentageFull == 100)
            {
                StopCoroutine(hasItemOnCoroutine);
                hasItemOnCoroutine = null;
            }
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
    }

    public PickableItem TakeItemFromMe()
    {
        PickableItem tempItem = itemOnSurface;
        itemOnSurface = null;
        return tempItem;
    }
}
