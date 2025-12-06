using Energy;
using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Events;

public class EnergyController : Singleton<EnergyController>
{
    private void Start()
    {
        EnergyTimer.Instance.Init(this);
        EventDispatcher.Push(EventId.OnEnergyUpdate);
    }
    public void AddEnergy(int value, UnityAction onUse = null)
    {
        if (value >= 0)
        {
            var energy = DBController.Instance.ENERGY;
            energy += value;
            DBController.Instance.ENERGY = energy;
            EventDispatcher.Push(EventId.OnEnergyUpdate);
        }
        else
        {
            if (DBController.Instance.ENERGY >= Math.Abs(value))
            {
                var energy = DBController.Instance.ENERGY;
                energy += value;
                DBController.Instance.ENERGY = energy;
                EnergyTimer.Instance.StartCountdown().Forget();
                EventDispatcher.Push(EventId.OnEnergyUpdate);
                onUse?.Invoke();
            }
        }

    }
    public void UpdateEnergy(int value)
    {
        var energy = DBController.Instance.ENERGY;
        energy = value;
        DBController.Instance.ENERGY = energy;
        EventDispatcher.Push(EventId.OnEnergyUpdate);
    }
    public bool CheckEnoughEnergy(int value)
    {
        if (DBController.Instance.ENERGY >= value)
        {
            return true;
        }
        return false;
    }
}