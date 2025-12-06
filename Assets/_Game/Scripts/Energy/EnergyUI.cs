using Energy;
using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Data;
using DG.Tweening;
using Spin;
using UnityEngine;
using UnityEngine.UI;

public class EnergyUI : MonoBehaviour
{
    [SerializeField] private Text txtValue;
    [SerializeField] private Text txtTime;
    [SerializeField] private Transform bannerTime;
    private void Awake()
    {
        EventDispatcher.Register(EventId.OnEnergyUpdate, UpdateValueUI);

    }
    private void Start()
    {
        EnergyTimer.Instance.ActionUpdateTimeUI += UpdateTimeUI;

        UpdateValueUI(null);
    }
    public void UpdateValueUI(object data)
    {
        txtValue.text = $"{DBController.Instance.ENERGY}/{GameConfig.MAX_ENERGY}";
        var isShowTime = DBController.Instance.ENERGY < GameConfig.MAX_ENERGY;
        txtTime.gameObject.SetActive(isShowTime);
        if (isShowTime)
        {
            bannerTime.DOLocalMoveY(-70f, 0.5f).SetEase(Ease.OutBack);   
        }
        else
        {
            bannerTime.DOLocalMoveY(-10f, 0.5f).SetEase(Ease.InBack);
        }
    }

    public void UpdateTimeUI(TimeSpan timeSpan)
    {
       
        txtTime.text =
            $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";

    }

    public void OnShowShopIAPClick()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Click);
        // ShopController.Instance.OnShowShop();
    }

    private void OnDestroy()
    {
        EventDispatcher.RemoveCallback(EventId.OnEnergyUpdate, UpdateValueUI);
        EnergyTimer.Instance.ActionUpdateTimeUI -= UpdateTimeUI;
    }
}