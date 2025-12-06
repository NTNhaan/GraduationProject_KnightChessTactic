using UnityEngine;
using System;
using Data;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;

public class CoinController : Singleton<CoinController>
{
    [SerializeField] private Text textCoin;
    
    public static event Action<int, int> OnCoinChanged;
    public int CurrentCoin => DBController.Instance.COIN;
    
    public void AddCoin(int amount, UnityAction onComplete = null)
    {
        int oldCoin = DBController.Instance.COIN;
        int newCoin = Mathf.Max(0, oldCoin + amount);
    
    
        DOTween.To(() => oldCoin, x => oldCoin = x, newCoin, 1f)
            .OnUpdate(() =>
            {
                textCoin.text = oldCoin.ToString();
            })
            .OnComplete(() =>
            {
                DBController.Instance.COIN = newCoin;
                // EventDispatcher.Push(EventId.OnCoinAdded, amount);
                EventDispatcher.Push(EventId.OnCoinChanged, newCoin);
                onComplete?.Invoke();
            });
    }

    public void SpendCoin(int amount)
    {
        if (amount <= 0) return;
        if (CurrentCoin < amount) return;

        int old = CurrentCoin;
        DBController.Instance.COIN -= amount;

        OnCoinChanged?.Invoke(old, CurrentCoin);
        EventDispatcher.Push(EventId.OnCoinChanged, DBController.Instance.COIN);
    }

    public void SetCoin(int value)
    {
        int old = CurrentCoin;
        DBController.Instance.COIN = Mathf.Max(0, value);

        OnCoinChanged?.Invoke(old, CurrentCoin);
        EventDispatcher.Push(EventId.OnCoinChanged, DBController.Instance.COIN);
    }
}