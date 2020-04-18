﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}

public class PlantController : MonoBehaviour
{
    public Animator animator;

    // This is only used for initial values. 
    // Change stats dictionary if you wanna change the stat
    public List<StatTypeClass> statsList = new List<StatTypeClass>();
    public Dictionary<StatType, float> stats = new Dictionary<StatType, float>();
    public float eatRadius;
    public PlantRotate rotateScript;
    public float angleOfAttack = 5f;

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
        var newTarget = PickNewTarget();


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

        // For debugging to see the stat values in editor
        UpdateStatsList();
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
        StatType minStatKey = StatType.Happiness;
        float minStatValue = float.MaxValue;

        foreach(var stat in stats)
        {
            if (stat.Value < minStatValue)
            {
                minStatKey = stat.Key;
                minStatValue = stat.Value;
            }
        }

        return (StatType) minStatKey;
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
            var levelPosition = attackableObject.transform.position;
            levelPosition.y = transform.position.y;

            if (Vector3.Distance(levelPosition, transform.position) < eatRadius)
            {
                attackableObjectsInRange.Add(attackableObject);
            }
        }
        
        if (attackableObjectsInRange.Count <= 0) return null;

        // Based on which stat is the lowest stat for the plant, 
        // target the one that gives the most in that stat
        StatType lowestStatType = GetLowestStat();
        AttackableObject mostValueableObject = attackableObjectsInRange[0];

        for (int i = 1; i < attackableObjectsInRange.Count; i++)
        {
            if (attackableObjectsInRange[i].GetStatFillValue(lowestStatType) >
                mostValueableObject.GetStatFillValue(lowestStatType))
            {
                mostValueableObject = attackableObjectsInRange[i];
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
}
