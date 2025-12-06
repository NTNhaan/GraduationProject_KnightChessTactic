using Cysharp.Threading.Tasks;
using DG.Tweening;
using Spin;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class ScreenGetRW : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private Text txtValue;
    [SerializeField] private Sprite sprExp;
    [SerializeField] private GameObject gobjRW;
    [SerializeField] private GameObject gobjCover;
    [SerializeField] private GameObject btnClaim;

    private Tween scaleTween;
    public void OnShowScreen(int value, Sprite rewardIcon)
    {
        ShowReward(value, rewardIcon);
    }
    public void OnShowScreen(int expValue)
    {
        ShowReward(expValue, sprExp);
    }

    private void ShowReward(int value, Sprite icon)
    {
        Debug.Log($"[Reward Popup] Show Reward +{value}");

        scaleTween?.Kill();

        gobjCover.SetActive(true);
        gobjRW.SetActive(true);

        btnClaim.SetActive(false);
        gobjRW.transform.localScale = Vector3.zero;

        imgIcon.sprite = icon;
        txtValue.text = $"+{value}";

        scaleTween = gobjRW.transform
            .DOScale(Vector3.one, 0.45f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => btnClaim.SetActive(true));
    }
    public void OnHideScreen()
    {
        scaleTween?.Kill();

        gobjRW.SetActive(false);
        gobjCover.SetActive(false);
        btnClaim.SetActive(false);
        UITopController.Instance.HideTab();
    }

    public void OnCloseClick()
    {
        OnHideScreen();
        SpinController.Instance.AddAccumulateCountSpin();
        
        int cur = DBController.Instance.COUNT_SPINT;
        int max = GameConfig.ACCUMULATE;
        float fill = max > 0 ? (float)cur / max : 0f;

        EventDispatcher.Push(EventId.OnAccumulateChanged, new Vector3(cur, max, fill));
    }
}
