using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableSurface : MonoBehaviour
{
    public Transform positionOfItem;
    public PickableItem itemOnSurface;

    public UsableItem.UseType typeSurfaceAdds;

    private void Update()
    {
        if(itemOnSurface is UsableItem)
        {
            UsableItem item = itemOnSurface as UsableItem;
            if (!item.isFull)
            {
                item.AddFromSurface(typeSurfaceAdds);
            }
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
