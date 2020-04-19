using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject interactionHitBox;
    public Image fillBar;

    [SerializeField]
    private float useSpeed;

    private Coroutine useItemCoroutine;

    protected override void Start()
    {
        base.Start();
        visualElement.SetActive(percentageFull > 0);
        interactionHitBox.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        fillBar.fillAmount = percentageFull/100f;
    }

    public void StartUse()
    {
        useItemCoroutine = StartCoroutine(UseItemRoutine());
        interactionHitBox.SetActive(true);
    }

    public void StopUse()
    {
        StopCoroutine(useItemCoroutine);
        useItemCoroutine = null;
        interactionHitBox.SetActive(false);
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
            visualElement.SetActive(percentageFull > 0);
            percentageFull -= amountToAdd;
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
