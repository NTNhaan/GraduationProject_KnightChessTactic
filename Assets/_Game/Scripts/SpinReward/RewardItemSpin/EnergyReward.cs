using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyReward : IRewardSpinHandler
{
    private int value;

    public EnergyReward(int value)
    {
        this.value = value;
    }
    public void ApplyReward()
    {
        Debug.Log($"Reward Energy {value}");
        EnergyController.Instance.AddEnergy(value);
    }
}