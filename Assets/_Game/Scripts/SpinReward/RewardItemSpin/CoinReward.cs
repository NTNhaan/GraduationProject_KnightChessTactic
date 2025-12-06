using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class CoinReward : IRewardSpinHandler
{
    private int value;

    public CoinReward(int value)
    {
        this.value = value;
    }

    public void ApplyReward()
    {
        Debug.Log($"Reward Coin {value}");
        UITopController.Instance.ShowTab(() =>
        {
            // DBController.Instance.COIN += value;
            CoinController.Instance.AddCoin(value);
            // EventDispatcher.Push(EventId.OnCoinChanged); 
        });
    }


}