using System.Collections;
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

public class PlantController : MonoBehaviour
{
    [Range(0, 100)] public float[] stats;
    public float eatRadius;
    public PlantRotate rotateScript;

    // Start is called before the first frame update
    void Start()
    {
        stats = new float[StatType.GetNames(typeof(StatType)).Length];
    }

    // Update is called once per frame
    void Update()
    {
        var newTarget = PickNewTarget();

        
        if (newTarget)
        {
            rotateScript.SetTarget(newTarget.transform);
        }
        else
        {
            rotateScript.SetTarget(null);
        }
    }

    public StatType GetLowestStat()
    {
        int minStatIndex = 0;

        for (int i = 1; i < stats.Length; i++)
        {
            if (stats[i] < stats[minStatIndex])
            {
                minStatIndex = i;
            }
        }

        return (StatType) minStatIndex;
    }

    public float GetStat(StatType statType)
    {
        return stats[(int)statType];
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

        Debug.Log("atck obj count = " + attackableObjects.Length + "; in range = "
            + attackableObjectsInRange.Count);
        if (attackableObjectsInRange.Count <= 0) return null;

        // Based on which stat is the lowest stat for the plant, 
        // target the one that gives the most in that stat
        StatType lowestStatType = GetLowestStat();
        Debug.Log(lowestStatType);
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
}
