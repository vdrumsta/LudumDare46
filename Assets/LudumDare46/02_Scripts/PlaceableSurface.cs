using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FMOD;
using Debug = UnityEngine.Debug;

public class PlaceableSurface : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string placeDownSound = "";
    private FMOD.Studio.EventInstance soundEvent;

    public Transform positionOfItem;
    public PickableItem itemOnSurface;
    
    private Coroutine hasItemOnCoroutine;

    [SerializeField]
    private float fillSpeed;

    public UseType typeSurfaceAdds;

    [Header("Consumable surface")]
    public bool consumesItem;
    public UseType typeToConsume;
    public PickableItem itemToSpawn;
    public int numberNeeded;
    private int amountOnSurface;
    public TextMeshProUGUI textForConsumeables;


    private void Start()
    {
        // FMOD
        if (placeDownSound.Length != 0)
        {
            //Debug.Log(gameObject.name + " attached sound = " + placeDownSound);
            
            //FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundEvent);
        }
    }

    private void Update()
    {
        if (consumesItem)
        {
            ConsumableItem consumableItem = itemOnSurface as ConsumableItem;
            if (amountOnSurface == numberNeeded && itemOnSurface == null)
            {
                amountOnSurface = 0;
                PlaceItemOnMe(Instantiate(itemToSpawn, positionOfItem.position, positionOfItem.rotation, transform));
            }
            else if (consumableItem && consumableItem.canBeUsedFor == typeToConsume)
            {
                Debug.Log("Consume item");
                Destroy(itemOnSurface.gameObject);
                itemOnSurface = null;
                amountOnSurface++;
            }

            textForConsumeables.text = amountOnSurface + "/" + numberNeeded;
        }

        if (itemOnSurface is UsableItem)
        {
            UsableItem item = itemOnSurface as UsableItem;

            if (item.percentageFull != 100 && hasItemOnCoroutine == null)
            {
                hasItemOnCoroutine = StartCoroutine(HasItemOnRoutine(item));

                if (placeDownSound.Length != 0)
                {
                    soundEvent = FMODUnity.RuntimeManager.CreateInstance(placeDownSound);
                    soundEvent.start();
                    Debug.Log("Playing put down sound");
                }
            }
            else if (item.percentageFull == 100 && hasItemOnCoroutine != null)
            {
                StopCoroutine(hasItemOnCoroutine);
                hasItemOnCoroutine = null;
            }
        }
        else if (hasItemOnCoroutine != null)
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
