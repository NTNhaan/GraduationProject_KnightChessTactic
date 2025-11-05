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
    
    [Header("Shop IAP")]
    [SerializeField] private ShopIAPScreen shopIAPScreen;
    
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
        // Debug.Log($"CurrentState: {CurScreen}");
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
                mainScreen.ShowScreen(async ()=> await mainScreen.DoShowMainScreen());
                CurScreen = ScreenGame.MainScreen;
                break;
            case ScreenGame.ShopScreen:
                shopIAPScreen.ShowTransition();
                shopIAPScreen.ShowScreen(async ()=> await shopIAPScreen.DoShowShopIAP());
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
                mainScreen.DoHideMainScreen().ContinueWith(()=> mainScreen.HideScreen(onComplete));
                break;
            case ScreenGame.ShopScreen:
                shopIAPScreen.DoHideShopIAP().ContinueWith(()=> shopIAPScreen.HideScreen(onComplete));
                break;

            default:
                break;
        }
    }
}