using Audio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;

public class ShopScreen : ScreenBase
{
    [Header("Shop IAP")]
    [SerializeField] private RectTransform bannerShop;
    [SerializeField, Tooltip("Mid Panel")] private Transform contentScrollView;
    [SerializeField] private Button btnBack;
    // [SerializeField] private Transform bannerCoin;
    [SerializeField] private Image imgBackground;
    
    #region Override Methods
    public override void ShowScreen(UnityAction onComplete)
    {
        base.ShowScreen(onComplete);
    }

    public override void HideScreen(UnityAction onComplete)
    {
        base.HideScreen(onComplete);
    }

    public override void LoadCoinUI()
    {
        base.LoadCoinUI();
    }

    // public void UpdateCoinUI()
    // {
    //     txtCoin.text = $"{DBController.Instance.COIN}";
    // }
    #endregion
    
    public async UniTask DoShowShopIAP()
    {
        // btnBack.interactable = true;
        // await bannerShop.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        // await bannerShop.DOAnchorPosY(-250, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        // var t2 = btnBack.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        // // var t3 = bannerCoin.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        // AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        // await UniTask.WhenAll(t2);
        // contentScrollView.DOScale(1f, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        // {
        //     imgBackground.DOFade(1f, 0.5f);
        // });
    }
    public async UniTask DoHideShopIAP(UnityAction onComplete = null)
    {
        // btnBack.interactable = false;
        // var t1 = btnBack.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        // // var t2 = bannerCoin.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        // await UniTask.WhenAll(t1);
        // onComplete?.Invoke();
        //
        // bannerShop.DOAnchorPosY(-800, 0.3f).SetEase(Ease.InBack);
        // bannerShop.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        // contentScrollView.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        // imgBackground.DOFade(0f, 0.3f);
    }
}