using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackableObject : MonoBehaviour
{
    [Serializable]
    public class StatTypeClass
    {
        public StatType statName;
        [Range(0, 100)] public float value;
    }

    public Dictionary<StatType, float> statFillValues = new Dictionary<StatType, float>();

    public List<StatTypeClass> statFillValuesList = new List<StatTypeClass>();

    void Start()
    {
        foreach(var statFillValue in statFillValuesList)
        {
            statFillValues.Add(statFillValue.statName, statFillValue.value);
        }
    }

    public float GetStatFillValue(StatType stat)
    {
        if (statFillValues.ContainsKey(stat))
        {
            return statFillValues[stat];
        }
        else
        {
            return 0;
        }
    }
}
