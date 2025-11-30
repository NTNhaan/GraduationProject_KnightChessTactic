using System;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UITopController : Singleton<UITopController>
{
    [SerializeField] private RectTransform rectTopBanner;
    public void ShowTab()
    {
        rectTopBanner.DOAnchorPosY(0, 0.5f);
    }
    public void HideTab()
    {
        rectTopBanner.DOAnchorPosY(250, 0.5f);
    }
}
