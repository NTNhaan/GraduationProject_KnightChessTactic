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
    [SerializeField] private HomeScreen homeScreen;
    
    [Header("Shop IAP")]
    [SerializeField] private ShopScreen shopScreen;
    
    [Header("Current Screen")]
    [SerializeField] private ScreenGame curScreen;
    private ScreenGame _preScreen;
    public ScreenGame CurScreen
    {
        get => curScreen;
        set => curScreen = value;
    }
    public HomeScreen HomeScreen { get => homeScreen; }
    public ScreenGame PreScreen { get => _preScreen; set => _preScreen = value; }

    private void Start()
    {
        StartAsync().Forget();
    }

    private async UniTask StartAsync()
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
        HideCurScreen(() => homeScreen.ClickStartButton());
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
                homeScreen.ShowTransition();
                homeScreen.ShowScreen(async ()=> await homeScreen.DoShowMainScreen());
                CurScreen = ScreenGame.MainScreen;
                break;
            case ScreenGame.ShopScreen:
                shopScreen.ShowTransition();
                shopScreen.ShowScreen(async ()=> await shopScreen.DoShowShopIAP());
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
                homeScreen.DoHideMainScreen().ContinueWith(()=> homeScreen.HideScreen(onComplete));
                break;
            case ScreenGame.ShopScreen:
                shopScreen.DoHideShopIAP().ContinueWith(()=> shopScreen.HideScreen(onComplete));
                break;

            default:
                break;
        }
    }
}