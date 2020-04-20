﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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

    public StatTypeClass(StatType statName, float value, 
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
    public Animator animator;

    // Change stats dictionary if you wanna change the stat, not the list
    public List<StatTypeClass> statsList = new List<StatTypeClass>();
    public Dictionary<StatType, float> stats = new Dictionary<StatType, float>();
    public List<StatSliderClass> statBars = new List<StatSliderClass>();

    public float eatRadius;
    public PlantRotate rotateScript;
    public float angleOfAttack = 5f;
    private List<GameObject> swallowedItems = new List<GameObject>();
    public Transform spitPoint;
    public float spitForce = 1f;

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
            animator.SetBool("attack", value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Populate stats dictionary
        foreach (var stat in statsList)
        {
            stats.Add(stat.statName, stat.value);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ReduceStats();

        var newTarget = PickNewTarget();

        FindAndAttackTarget(newTarget);

        UpdateStatsBars();

        // For debugging to see the stat values in editor
        UpdateStatsList();

        // For debugging
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stats[StatType.Hunger] -= 20;
        }
    }

    public void AddStat(StatType statType, float amount)
    {
        stats[statType] += amount;
    }

    private void ReduceStats()
    {
        foreach(var stat in statsList)
        {
            stats[stat.statName] -= stat.reducePerSecond * Time.deltaTime;
            stats[stat.statName] = Mathf.Clamp(stats[stat.statName], 0, 100);
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

    public StatType GetLowestStat()
    {
        // Default is hunger
        StatType minStatKey = StatType.Hunger;
        float minStatValue = float.MaxValue;

        foreach(var stat in stats)
        {
            if (stat.Value < minStatValue && stat.Value < GetStatMinSatisfied(stat.Key))
            {
                minStatKey = stat.Key;
                minStatValue = stat.Value;
            }
        }

        return (StatType) minStatKey;
    }

    private float GetStatMinSatisfied(StatType statType)
    {
        foreach(var stat in statsList)
        {
            if (stat.statName == statType)
            {
                return stat.maxValueSatisfaction;
            }
        }

        return 100;
    }

    public float GetStat(StatType statType)
    {
        return stats[statType];
    }

    private AttackableObject PickNewTarget()
    {
        // Retrieve all components in the area that represent eatable objects
        var attackableObjects = FindObjectsOfType<AttackableObject>();
        List<AttackableObject> attackableObjectsInRange = new List<AttackableObject>();
        foreach (var attackableObject in attackableObjects)
        {
            if (!(attackableObject.isAttackable)) continue;

            var levelPosition = attackableObject.transform.position;
            levelPosition.y = transform.position.y;

            if (Vector3.Distance(levelPosition, transform.position) < eatRadius)
            {
                attackableObjectsInRange.Add(attackableObject);
            }
        }
        
        if (attackableObjectsInRange.Count <= 0) return null;

        Debug.Log("Objects in range = " + attackableObjectsInRange.Count);

        // Based on which stat is the lowest stat for the plant, 
        // target the one that gives the most in that stat
        StatType lowestStatType = GetLowestStat();
        AttackableObject mostValueableObject = null;
        float highestStatValue = 0;

        for (int i = 0; i < attackableObjectsInRange.Count; i++)
        {
            var currentObjectStatValue = attackableObjectsInRange[i].GetStatFillValue(lowestStatType);
            if(currentObjectStatValue > highestStatValue)
            {
                mostValueableObject = attackableObjectsInRange[i];
                highestStatValue = currentObjectStatValue;
            }
        }

        return mostValueableObject;
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
    }
}
