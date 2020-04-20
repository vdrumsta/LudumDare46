using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

[Serializable]
public enum StatType
{
    Hunger,
    Sunshine,
    Happiness,
    Water
}

[Serializable]
public class StatTypeClass
{
    public StatType statName;
    [Range(0, 100)] public float value;
    public float reducePerSecond = 1f;

    // When the value drop below this number, 
    // the plants wants to eat items of this stat type
    public float maxValueSatisfaction = 50f;

    public StatTypeClass(StatType statName = StatType.Happiness, float value = 0f, 
        float reducePerSecond = 0f, float maxValueSatisfaction = 50f)
    {
        this.statName = statName;
        this.value = value;
        this.reducePerSecond = reducePerSecond;
        this.maxValueSatisfaction = maxValueSatisfaction;
    }
}

[Serializable]
public class StatSliderClass
{
    public StatType statName;
    public Slider slider;
    public Gradient colorGradient;
    public Image fill;
    public float sliderValue;
    public float SliderValue
    {
        get
        {
            return sliderValue;
        }

        set
        {
            sliderValue = value;
            var currentColor = colorGradient.Evaluate(value / 100);
            fill.color = currentColor;
            slider.value = value;
        }
    }
}


public class PlantController : MonoBehaviour
{
    public Animator stemAnimator;
    public Animator headAnimator;

    #region Head Animations
    private int isHappy_hash = Animator.StringToHash("IsHappy");
    private int attack_hash = Animator.StringToHash("Attack");
    private int eat_hash = Animator.StringToHash("Eat");
    private int die_hash = Animator.StringToHash("Die");

    public bool isHappyAnimation { get { return headAnimator.GetBool(isHappy_hash); } set { headAnimator.SetBool(isHappy_hash, value); } }
    public bool attackAnimation { get { return headAnimator.GetBool(attack_hash); } set { headAnimator.SetBool(attack_hash, value); } }
    public bool eatAnimation { get { return headAnimator.GetBool(eat_hash); } set { headAnimator.SetBool(eat_hash, value); } }
    public bool dieAnimation { get { return headAnimator.GetBool(die_hash); } set { headAnimator.SetBool(die_hash, value); } }

    #endregion

    // Change stats dictionary if you wanna change the stat, not the list
    public List<StatTypeClass> statsList = new List<StatTypeClass>();
    public Dictionary<StatType, float> stats = new Dictionary<StatType, float>();
    public List<StatSliderClass> statBars = new List<StatSliderClass>();

    public PlantGrow growScript;
    public float eatRadius;
    private float startingEatRadius;
    public PlantRotate rotateScript;
    public float angleOfAttack = 5f;
    private List<GameObject> swallowedItems = new List<GameObject>();
    public Transform spitPoint;
    public float spitForce = 1f;
    private bool isDead;
    public GameObject deathScreen;

    private bool isAttacking;
    public bool IsAttacking
    {
        get
        {
            return isAttacking;
        }

        private set
        {
            isAttacking = value;
            stemAnimator.SetBool("attack", value);
            attackAnimation = value;

            if (value)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Plant/Move");
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startingEatRadius = eatRadius;

        // Populate stats dictionary
        foreach (var stat in statsList)
        {
            stats.Add(stat.statName, stat.value);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            ReduceStats();

            var newTarget = PickNewTarget();

            FindAndAttackTarget(newTarget);

            UpdateStatsBars();

            isHappyAnimation = IsStatSatisfied(StatType.Happiness);

            // For debugging to see the stat values in editor
            UpdateStatsList();

            // For debugging
            if (Input.GetKeyDown(KeyCode.Space))
            {
                stats[StatType.Hunger] -= 20;
            }

            GrowEatRadius();
        }
    }

    private void GrowEatRadius()
    {
        if (growScript)
        {
            eatRadius = startingEatRadius * growScript.newGrowScale;
        }
    }

    public void AddStat(StatType statType, float amount)
    {
        if(!isDead)
        {
            stats[statType] += amount;
        }
    }

    private void ReduceStats()
    {
        if (!isDead)
        {
            foreach (var stat in statsList)
            {
                stats[stat.statName] -= stat.reducePerSecond * Time.deltaTime;
                stats[stat.statName] = Mathf.Clamp(stats[stat.statName], 0, 100);
                if (stats[stat.statName] <= 0)
                {
                    Die();
                    break;
                }
            }
        }
    }

    private void Die()
    {
        isDead = true;
        dieAnimation = true;
        if (deathScreen)
        {
            deathScreen.SetActive(true);
        }
    }

    private void UpdateStatsBars()
    {
        foreach(var bar in statBars)
        {
            bar.SliderValue = stats[bar.statName];
        }
    }

    private void FindAndAttackTarget(AttackableObject newTarget)
    {
        if (newTarget)
        {
            rotateScript.SetTarget(newTarget.transform);

            // If target is within the angle of attack, then attack
            if (targetWithinAttackAngle(newTarget))
            {
                IsAttacking = true;
            }
            else
            {
                IsAttacking = false;
            }
        }
        else
        {
            rotateScript.SetTarget(null);
            IsAttacking = false;
        }
    }

    private void UpdateStatsList()
    {
        foreach(var stat in statsList)
        {
            stat.value = stats[stat.statName];
        }
    }

    public float GetStat(StatType statType)
    {
        return stats[statType];
    }

    public bool IsStatSatisfied(StatType statType)
    {
        StatTypeClass stat = new StatTypeClass();
        foreach(StatTypeClass statTypeClass in statsList)
        {
            if(statTypeClass.statName == statType)
            {
                stat = statTypeClass;
                break;
            }
        }
        return GetStat(statType) > stat.maxValueSatisfaction;
    }

    private AttackableObject PickNewTarget()
    {
        // Retrieve all components in the area that represent eatable objects
        var attackableObjects = FindObjectsOfType<AttackableObject>();
        List<AttackableObject> attackableObjectsInRange = new List<AttackableObject>();

        // Check which components are in range
        foreach (var attackableObject in attackableObjects)
        {
            if (!(attackableObject.isAttackable)) continue;

            var targetPos = attackableObject.transform.position;

            if (Vector3.Distance(targetPos, transform.position) < eatRadius)
            {
                attackableObjectsInRange.Add(attackableObject);
            }
        }

        if (attackableObjectsInRange.Count <= 0) return null;

        // Get a list of all unsatisfied stats
        List<StatTypeClass> unsatisfiedStats = GetUnsatisfiedStats();

        // Order unsatisfied stats based on which one is the lowest
        unsatisfiedStats = unsatisfiedStats.OrderBy(t => t.value).ToList();

        AttackableObject mostValueableObject = null;
        float highestStatValue = 0;

        // Based on the order of the lowest stats, look for item which provides
        // most of that stat
        foreach (var stat in unsatisfiedStats)
        {
            foreach (var objInRange in attackableObjectsInRange)
            {
                if (objInRange.statFillValues.ContainsKey(stat.statName) 
                    && objInRange.statFillValues[stat.statName] > highestStatValue)
                {
                    mostValueableObject = objInRange;
                    highestStatValue = objInRange.statFillValues[stat.statName];
                }
            }
        }

        return mostValueableObject;
    }

    private List<StatTypeClass> GetUnsatisfiedStats()
    {
        List<StatTypeClass> unsatisfiedStats = new List<StatTypeClass>();
        foreach(var stat in statsList)
        {
            if (!IsStatSatisfied(stat.statName))
            {
                unsatisfiedStats.Add(stat);
            }
        }

        return unsatisfiedStats;
    }

    private bool targetWithinAttackAngle(AttackableObject target)
    {
        Vector3 lookDirection = transform.forward;
        lookDirection.y = 0;

        // Determine which direction to rotate towards
        Vector3 targetDirection = target.transform.position - transform.position;
        targetDirection.y = 0;

        float angleToTarget = Vector3.Angle(lookDirection, targetDirection);

        return angleToTarget < angleOfAttack;
    }

    public void Swallow(GameObject itemToSwallow, float spitAfterSecond)
    {
        itemToSwallow.transform.SetParent(null);
        swallowedItems.Add(itemToSwallow);
        itemToSwallow.SetActive(false);
        StartCoroutine(SpitAfterTime(itemToSwallow, spitAfterSecond));
    }

    IEnumerator SpitAfterTime(GameObject swallowedItem, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (swallowedItems.Contains(swallowedItem))
        {
            swallowedItems.Remove(swallowedItem);
        }

        swallowedItem.transform.position = spitPoint.position;
        swallowedItem.SetActive(true);

        var itemRb = swallowedItem.GetComponent<Rigidbody>();
        itemRb?.AddForce(transform.forward * spitForce, ForceMode.VelocityChange);

        var attackableObjScript = swallowedItem.GetComponent<AttackableObject>();
        StartCoroutine(attackableObjScript.ChangeIsAttackableForTime(false, 2));

        var pickableItemScript = swallowedItem.GetComponent<PickableItem>();
        if (pickableItemScript)
        {
            pickableItemScript.itemInUse = false;
        }
    }
}
