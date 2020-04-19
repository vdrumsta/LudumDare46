using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItemUseComponent : MonoBehaviour
{
    private UsableItem usableItem;
    private Coroutine addingToPlantCoroutine;
    PlantController currentPlant;

    private void Start()
    {
        usableItem = this.GetComponentInParent<UsableItem>();
        usableItem.onStartDelegate += StartUsing;
        usableItem.onStopDelegate += Stopusing;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlantController plant = other.GetComponent<PlantController>();
        if (plant)
        {
            currentPlant = plant;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlantController plant = other.GetComponent<PlantController>();
        if (plant)
        {
            currentPlant = null;
        }
    }

    private void StartUsing()
    {
        if (currentPlant)
        {
            addingToPlantCoroutine = StartCoroutine(AddToPlantRoutine(currentPlant));
        }
    }

    private void Stopusing()
    {
        if (addingToPlantCoroutine != null)
        {
            StopCoroutine(addingToPlantCoroutine);
            addingToPlantCoroutine = null;
        }
    }

    private IEnumerator AddToPlantRoutine(PlantController plant)
    {
        do
        {
            yield return new WaitForSeconds(usableItem.useSpeed);
            plant.AddStat(usableItem.statTypeForPlant, 1);

        } while (true);
    }
}
