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
    Food
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
}
