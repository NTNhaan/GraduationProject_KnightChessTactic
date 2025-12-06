using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpReward : IRewardSpinHandler
{
    private int value;

    public ExpReward(int value)
    {
        this.value = value;
    }
    public void ApplyReward()
    {
        Debug.Log($"Reward Exp {value}");
        UITopController.Instance.ShowTab(() =>
        {
            ExpBarController.Instance.AddExp(value);
        });
    }


}