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
    

    public UseType canGetFromSurface;
    public UseType canBeUsedFor;
    public StatType statTypeForPlant;
    public float percentageFull;
    public GameObject visualElement;
    public UsableItemUseComponent interactionHitBox;
    public Image fillBar;
    public float useSpeed;
    public bool autoStart;

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
    }

    protected override void Update()
    {
        base.Update();
        fillBar.fillAmount = percentageFull/100f;
    }

    public void StartUse()
    {
        onStartDelegate?.Invoke();
        useItemCoroutine = StartCoroutine(UseItemRoutine());
    }

    public void StopUse()
    {
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
                attackable.statFillValuesList.Add(new StatTypeClass(StatType.Happiness, (percentageFull > 0.5f) ? 0.3f : -0.2f, 0f));
            }
            attackable.RefreshAttackableObject();
        }
    }
}
