using System;
using Audio;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Setting;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Data;
using DG.Tweening;
using UnityEngine.Serialization;

public class HomeScreen : ScreenBase
{
    [FormerlySerializedAs("dailyReward")]
    [Header("Main Screen")]
    [SerializeField] private Button btnDailyReward;
    [SerializeField] private Button btnSpinReward;
    [SerializeField] private Button btnLevelMode;
    [SerializeField] private Button btnClassicMode;
    [SerializeField] private Button btnPveMode;

    [Header("Time Daily Reward")]
    [SerializeField] private Text txtTimeRemain;


    // [Header("=====HightScore MainScene=====")]
    // [SerializeField] private Text hightScoreText;
    // [SerializeField] private Text coinText;
    // [SerializeField] private Text textLevel;

    [Header("UI Animation")]
    [SerializeField] private UIAnimationSequence topGroup;
    [SerializeField] private UIAnimationGroup midGroup;
    [SerializeField] private UIAnimationGroup bottomGroup;
    private bool stopIdleAnim = false;

    public void OnEnable()
    {
        EventDispatcher.Register(EventId.OnUpdateTimeRemain, OnUpdateTimeRemain);
        EventDispatcher.Register(EventId.OnGameStateChanged, OnGameStateChanged);
    }
    public void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.OnUpdateTimeRemain, OnUpdateTimeRemain);
        EventDispatcher.RemoveCallback(EventId.OnGameStateChanged, OnGameStateChanged);
        stopIdleAnim = true;
    }

    private void OnGameStateChanged(object data = null)
    {
        bool isInteractable = InGameData.GAME_STATE != GameState.Loading;
        SetButtonsInteractable(isInteractable);
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (btnDailyReward != null) btnDailyReward.interactable = interactable;
        if (btnSpinReward != null) btnSpinReward.interactable = interactable;
        if (btnLevelMode != null) btnLevelMode.interactable = interactable;
        if (btnClassicMode != null) btnClassicMode.interactable = interactable;
        if (btnPveMode != null) btnPveMode.interactable = interactable;
    }
    async UniTask Start()
    {
        // InitGame();
        InitExp();

        await UniTask.WaitUntil(() => InGameData.GAME_STATE == GameState.LoadingDone);
        // topAnim.SetBool("isShow", true);
        // bottomAnim.SetBool("isShow", true);
        // AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        // StartIdleLoop();
    }
    #region Override Methods
    public override void ShowScreen(UnityAction onComplete)
    {
        LoadCoinUI();
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

    // public void UpdateCoinUI(object data = null)
    // {
    //     coinText.text = DBController.Instance.COIN.ToString("n0");
    // }
    #endregion

    public void ClickStartButton()
    {
        if (InGameData.GAME_STATE == GameState.Loading)
            return;

        var level = DBController.Instance.LEVEL;
        InGameData.GAME_SCENE = SceneType.GameScene;
        InGameData.GAME_STATE = GameState.Loading;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        DoHideMainScreen().ContinueWith(() => SceneController.Instance.ChangeScene(InGameData.GAME_SCENE));
    }
    public void OnClickShowPausePopup()
    {
        if (InGameData.GAME_STATE == GameState.Loading)
            return;
        PopupController.Instance.ClickShowPausePopUp();
    }
    public void OnClickShowDailyRW()
    {
        if (InGameData.GAME_STATE == GameState.Loading)
            return;
        PopupController.Instance.ClickShowDailyRWPopUp();
    }
    public void OnClickShowSpinRW()
    {
        if (InGameData.GAME_STATE == GameState.Loading)
            return;
        PopupController.Instance.ClickShowSpinRWPopUp();
    }
    public void InitGame()
    {
        // textLevel.text = DBController.Instance.LEVEL.ToString();
        // hightScoreText.text = DBController.Instance.BEST_SCORE.ToString();
        // UpdateCoinUI();
        if (DBController.Instance.MUSIC)
            AudioController.Instance.PlayMusic(Sound.Name.Music_Menu);
    }

    public void InitExp()
    {
        ExpBarController.Instance.Init();
    }

    private void OnUpdateTimeRemain(object data)
    {
        txtTimeRemain.text = (string)data;
    }

    #region Show/Hide Main Screen
    public async UniTask DoShowMainScreen()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);

        await topGroup.Play();
        await midGroup.PlayShowAll();
        await bottomGroup.PlayShowAll();

        UITopController.Instance.ShowTab();
        TabController.Instance.ShowBottomTab();
        
        EventDispatcher.Push(EventId.OnMainScreen);
        SetButtonsInteractable(true);
    }

    public async UniTask DoHideMainScreen(UnityAction onComplete = null)
    {
        UITopController.Instance.HideTab();
        TabController.Instance.HideBottomTab();
    }
    #endregion
    
    #region Setting Buttons
    public void OnClickSoundButton()
    {
        EventDispatcher.Push(EventId.OnSoundClick);
        SettingCtrl.Instance.SetSound();
    }
    public void OnClickVibrationButton()
    {
        EventDispatcher.Push(EventId.OnSoundClick);
        SettingCtrl.Instance.SetVibration();
    }
    public void OnClickMusicButton()
    {
        EventDispatcher.Push(EventId.OnSoundClick);
        SettingCtrl.Instance.SetMusic();
    }
    #endregion
}