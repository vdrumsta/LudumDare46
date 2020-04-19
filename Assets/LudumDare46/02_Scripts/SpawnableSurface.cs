using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableSurface : MonoBehaviour
{
    public PickableItem objectToSpawn;
    public int maxNumberInExistance;

    private int currentNumberSpawned;

    public PickableItem GetObjectFromSurface()
    {
        if (objectToSpawn && currentNumberSpawned < maxNumberInExistance)
        {
            GameObject pickableObject = Instantiate(objectToSpawn.gameObject, transform.position, transform.rotation);
            currentNumberSpawned++;

            PickableItem itemInstance = pickableObject.GetComponent<PickableItem>();
            itemInstance.itemDestroyedDelegate += SpawnedObjectDestroyed;

            return itemInstance;
        }
        return null;
    }

    public void SpawnedObjectDestroyed(PickableItem item)
    {
        item.itemDestroyedDelegate -= SpawnedObjectDestroyed;
        currentNumberSpawned--;
    }
}
