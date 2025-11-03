using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using UnityEngine.UI;

public class ScreenController : Singleton<ScreenController>
{
    [Header("Main Screen")]
    [SerializeField] private MainScreen mainScreen;
    [SerializeField] private Transform leaderBoard;
    [SerializeField] private Transform shopIAP;
    [SerializeField] private Transform playBtn;
    [SerializeField] private Transform soundBtn;
    [SerializeField] private Transform musicBtn;
    [SerializeField] private Transform vibrateBtn;
    [SerializeField] private Transform bannerHighScore;
    [SerializeField] private RectTransform logoInGame;
    
    [Header("Shop IAP")]
    [SerializeField] private ShopIAPScreen shopIAPScreen;
    [SerializeField] private RectTransform bannerShop;
    [SerializeField] private Transform contentScrollView;
    [SerializeField] private Transform btnNext;
    [SerializeField] private Transform bannerCoin;
    [SerializeField] private Image imgBackground;
    
    [Header("Current Screen")]
    [SerializeField] private ScreenGame curScreen;
    private ScreenGame _preScreen;
    public ScreenGame CurScreen
    {
        get => curScreen;
        set => curScreen = value;
    }
    public MainScreen MainScreen { get => mainScreen; }
    public ScreenGame PreScreen { get => _preScreen; set => _preScreen = value; }

    private async UniTask Start()
    {
        await UniTask.WaitUntil(()=> InGameData.GAME_STATE == GameState.LoadingDone);
        CurScreen = ScreenGame.MainScreen;
        InGameData.GAME_STATE = GameState.MainMenu;
        ShowScreen(CurScreen);
    }

    void Update()
    {
        Debug.Log($"CurrentState: {CurScreen}");
    }
    
    public void OnPlayClick()
    {
        EventDispatcher.Push(EventId.OnSoundClick);
        HideCurScreen(() => mainScreen.ClickStartButton());
        CurScreen = ScreenGame.GamePlayScreen;
    }
    public void OnClickButton()
    {
        EventDispatcher.Push(EventId.OnSoundClick);
        switch (CurScreen)
        {
            case ScreenGame.MainScreen:
                ChangeScreen(ScreenGame.ShopScreen); 
                break;
            case ScreenGame.ShopScreen:
                ChangeScreen(ScreenGame.MainScreen);
                break;
            default:
                break;
        }
    }
    public void ChangeScreen(ScreenGame screen)
    {
        HideCurScreen(() =>
        {
            ShowScreen(screen);
        });
    }
    public void ShowScreen(ScreenGame screen)
    {
        switch (screen)
        {
            case ScreenGame.MainScreen:
                mainScreen.ShowTransition();
                mainScreen.ShowScreen(null);
                CurScreen = ScreenGame.MainScreen;
                DoShowMainScreen();
                break;
            case ScreenGame.ShopScreen:
                shopIAPScreen.ShowTransition();
                shopIAPScreen.ShowScreen(()=> DoShowShopIAP());
                CurScreen = ScreenGame.ShopScreen;
                break;

            default:
                break;
        }
    }
    public void HideCurScreen(UnityAction onComplete)
    {
        switch (CurScreen)
        {
            case ScreenGame.MainScreen:
                DoHideMainScreen(() => mainScreen.HideScreen(onComplete));
                break;
            case ScreenGame.ShopScreen:
                DoHideShopIAP(()=> shopIAPScreen.HideScreen(onComplete));
                break;

            default:
                break;
        }
    }

    public async UniTask DoShowMainScreen()
    {
        await logoInGame.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
        await logoInGame.DOAnchorPosY(-400, .3f).SetEase(Ease.OutBack).ToUniTask();

        var t1 = soundBtn.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
        var t2 = vibrateBtn.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
        var t3 = musicBtn.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        await UniTask.WhenAll(t1, t2, t3);

        var t4 = playBtn.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
        var t5 = leaderBoard.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
        var t6 = shopIAP.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
        var t7 = bannerHighScore.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        EventDispatcher.Push(EventId.OnMainScreen); 
        await UniTask.WhenAll(t4, t5, t6, t7);
    }

    public async UniTask DoHideMainScreen(UnityAction onComplete = null)
    {
        var t4 = playBtn.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();
        var t5 = leaderBoard.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();
        var t6 = shopIAP.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();
        var t7 = bannerHighScore.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();
        
        var t1 = soundBtn.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();
        var t2 = vibrateBtn.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();
        var t3 = musicBtn.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();

        await UniTask.WhenAll(t1, t2, t3, t4, t5, t6, t7);
        
        onComplete?.Invoke();
        
        logoInGame.DOAnchorPosY(-1000, .3f).SetEase(Ease.InBack);
        logoInGame.DOScale(0f, .3f).SetEase(Ease.InBack);
    }

    public async UniTask DoShowShopIAP()
    {
        await bannerShop.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        await bannerShop.DOAnchorPosY(-250, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        
        var t2 = btnNext.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        var t3 = bannerCoin.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        await UniTask.WhenAll(t2, t3);
        contentScrollView.DOScale(1f, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            imgBackground.DOFade(1f, 0.5f);
        });
    }
    public async UniTask DoHideShopIAP(UnityAction onComplete = null)
    {
        var t1 = btnNext.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var t2 = bannerCoin.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.WhenAll(t1, t2);
        onComplete?.Invoke();

        bannerShop.DOAnchorPosY(-800, 0.3f).SetEase(Ease.InBack);
        bannerShop.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        contentScrollView.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        imgBackground.DOFade(0f, 0.3f);
    }
}