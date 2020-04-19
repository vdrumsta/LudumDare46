using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackableObject : MonoBehaviour, IObservable<PlantController>
{
    public Dictionary<StatType, float> statFillValues = new Dictionary<StatType, float>();
    public List<StatTypeClass> statFillValuesList = new List<StatTypeClass>();
    public bool destroyUponAttack = true;
    public LayerMask damageObjectLayerMask;
    private List<IObserver<PlantController>> _observers = new List<IObserver<PlantController>>();
    public bool isAttackable = true;

    void Start()
    {
        RefreshAttackableObject();
    }

    public void RefreshAttackableObject()
    {
        statFillValues.Clear();
        foreach (var statFillValue in statFillValuesList)
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

    public void FillPlantStats(PlantController plantController)
    {
        foreach (var stat in statFillValues)
        {
            plantController.AddStat(stat.Key, stat.Value);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if other object layer is part of damage object layer mask
        if (damageObjectLayerMask == (damageObjectLayerMask | (1 << other.gameObject.layer)))
        {
            
            var rootObject = other.gameObject.GetComponent<ColliderRootReference>().root;
            var plantController = rootObject.GetComponent<PlantController>();

            if (!plantController.IsAttacking) return;

            // Inform observers that they were touched
            foreach (var observer in _observers)
            {
                observer.OnNext(plantController);
            }

            FillPlantStats(plantController);
            if (destroyUponAttack)
                Destroy(gameObject);
        }
    }

    // Used to subscribe to the observer list which gets called when the object gets attacked
    public IDisposable Subscribe(IObserver<PlantController> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
        return new Unsubscriber(_observers, observer);
    }
    
    // Used for unsubscribing from the observer list
    private class Unsubscriber : IDisposable
    {
        private List<IObserver<PlantController>> _observers;
        private IObserver<PlantController> _observer;

        public Unsubscriber(List<IObserver<PlantController>> observers, IObserver<PlantController> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }

    public IEnumerator ChangeIsAttackableForTime(bool targetAttackableState, float seconds)
    {
        isAttackable = targetAttackableState;
        yield return new WaitForSeconds(seconds);
        isAttackable = !targetAttackableState;
    }
}
