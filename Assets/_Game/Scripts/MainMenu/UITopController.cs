using System;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UITopController : Singleton<UITopController>
{
    [SerializeField] private RectTransform rectTopBanner;
    [SerializeField] private CoinUI coinUI;
    [SerializeField] private EnergyUI energyUI;
    [SerializeField] private ExpBarView expBarView;
    
    public CoinUI CoinUI => coinUI;
    public EnergyUI EnergyUI => energyUI;
    public ExpBarView ExpBarView => expBarView;
    
    public void ShowTab(UnityAction onComplete = null)
    {
        rectTopBanner.DOAnchorPosY(0, 0.5f).OnComplete(() =>
        {
            onComplete?.Invoke(); 
        });
    } 
    public void HideTab()
    {
        rectTopBanner.DOAnchorPosY(500, 0.5f);
    }
}
