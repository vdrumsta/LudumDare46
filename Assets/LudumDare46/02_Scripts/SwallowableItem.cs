using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwallowableItem : MonoBehaviour, IObserver<PlantController>
{
    public float spitAfterSecond;
    public AttackableObject attackableScript;

    // Start is called before the first frame update
    void Start()
    {
        if (attackableScript)
            attackableScript.Subscribe(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(PlantController plantController)
    {
        plantController.Swallow(gameObject, spitAfterSecond);
    }
}
