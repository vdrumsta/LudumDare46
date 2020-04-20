using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UseType
{
    PourWater,
    FillWithWater,
    ChargeElectronics,
    SunshinePlant,
    RawFood,
    CookedFood
}

public class UsableItem : PickableItem
{
    [FMODUnity.EventRef]
    public string turnOnSound = "";
    private FMOD.Studio.EventInstance soundEvent;

    public UseType canGetFromSurface;
    public UseType canBeUsedFor;
    public StatType statTypeForPlant;
    public float percentageFull;
    public GameObject visualElement;
    public UsableItemUseComponent interactionHitBox;
    public float useSpeed;
    public float addSpeed;
    public bool autoStart;
    public LineRenderer fillBar;
    public float fillBarDistanceAboveItem = 0.5f;
    private float[] fillBarPoints;
    private Vector3 fillBarLocalStartingPos;

    public delegate void UsableItemDelegate();
    public UsableItemDelegate onStartDelegate;
    public UsableItemDelegate onStopDelegate;

    private Coroutine useItemCoroutine;

    protected override void Start()
    {
        base.Start();

        if (visualElement)
        {
            visualElement.SetActive(percentageFull > 0);
        }

        
        if (fillBar)
        {
            // Get the start and end point of the fillBar
            fillBarPoints = new float[2];
            fillBarPoints[0] = fillBar.GetPosition(0).x;
            fillBarPoints[1] = fillBar.GetPosition(1).x;

            // Get the starting pos of fillBar
            fillBarLocalStartingPos = fillBar.transform.localPosition;
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if (fillBar)
        {
            // Change fill bar fill
            var endPointRatio = Mathf.Lerp(fillBarPoints[0], fillBarPoints[1], percentageFull / 100f);
            Vector3 fillBarNewEndPoint = new Vector3(endPointRatio, 0, 0);
            fillBar.SetPosition(1, fillBarNewEndPoint);

            // Keep the item above the usable item
            var newPos = transform.position;
            newPos.y = transform.position.y + fillBarDistanceAboveItem;
            //newPos.y = transform.position.y + fillBarDistanceAboveItem;
            fillBar.transform.position = newPos;

            // Set rotation to always be the same
            fillBar.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    public void StartUse()
    {
        // Fmod play sound
        if (turnOnSound.Length != 0)
        {
            soundEvent = FMODUnity.RuntimeManager.CreateInstance(turnOnSound);
            soundEvent.start();
            Debug.Log("Playing turn on sound");
        }

        onStartDelegate?.Invoke();
        useItemCoroutine = StartCoroutine(UseItemRoutine());
    }

    public void StopUse()
    {
        soundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        onStopDelegate?.Invoke();
        if (useItemCoroutine != null)
        {
            StopCoroutine(useItemCoroutine);
        }
        useItemCoroutine = null;
    }

    private IEnumerator UseItemRoutine()
    {
        do
        {
            yield return new WaitForSeconds(useSpeed);
            RemoveFromItem(1);

            // if autoStart, call onStartDelegate which will check if plant is in range
            // and fill its stats
            if (autoStart)
            {
                onStartDelegate?.Invoke();
            }

            if (percentageFull <= 0)
            {
                StopUse();
                break;
            }

        } while (true);
    }

    public void RemoveFromItem(int amountToAdd)
    {
        if (percentageFull > 0)
        {
            if (visualElement)
            {
                visualElement.SetActive(percentageFull > 0);
            }
            percentageFull -= amountToAdd;
        }
    }

    public void AddFromSurface(UseType typeToAdd, int amountToAdd)
    {
        if (percentageFull <100 && typeToAdd == canGetFromSurface)
        {
            if (visualElement)
            {
                visualElement.SetActive(percentageFull > 0);
            }
            percentageFull += amountToAdd;
        }
    }

    public override void PlaceItemAtLocation(Transform placedOnObject)
    {
        base.PlaceItemAtLocation(placedOnObject);
    }

    public override void DropItem(PlaceableSurface surface)
    {
        base.DropItem(surface);
        AttackableObject attackable = GetComponent<AttackableObject>();

        if (attackable)
        {
            if(canBeUsedFor == UseType.CookedFood)
            {
                attackable.statFillValuesList.Clear();
                attackable.statFillValuesList.Add(new StatTypeClass(StatType.Hunger, percentageFull, 0f));
                attackable.statFillValuesList.Add(new StatTypeClass(StatType.Happiness, (percentageFull > 0.2f) ? 0.3f : -0.1f, 0f));
            }
            attackable.RefreshAttackableObject();
        }
    }
}
