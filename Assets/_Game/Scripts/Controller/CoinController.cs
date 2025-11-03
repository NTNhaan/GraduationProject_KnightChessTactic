using UnityEngine;
using DG.Tweening;
using Data;
using UnityEngine.Events;
using UnityEngine.UI;

public class CoinController : Singleton<CoinController>
{
    public int CurrentCoin => DBController.Instance.COIN;

    public void AddCoin(int amount, Text textCoin, UnityAction onComplete = null)
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
    
    public bool SpendCoin(int amount)
    {
        if (DBController.Instance.COIN < amount)
            return false;

        DBController.Instance.COIN -= amount;
        // EventDispatcher.Push(EventId.OnCoinSpent, amount);
        EventDispatcher.Push(EventId.OnCoinChanged, DBController.Instance.COIN);
        return true;
    }
    
    public void SetCoin(int value)
    {
        DBController.Instance.COIN = Mathf.Max(0, value);
        EventDispatcher.Push(EventId.OnCoinChanged, DBController.Instance.COIN);
    }
}