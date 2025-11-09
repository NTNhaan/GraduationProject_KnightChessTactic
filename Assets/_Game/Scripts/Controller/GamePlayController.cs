using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DefaultNamespace;
using UnityEngine.SceneManagement;
using Data;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Serialization;

public class GamePlayController : Singleton<GamePlayController>
{
    [SerializeField] private LineCtrl lineCtrl;
    [Header("TimeManager")]
    [SerializeField] private Text timeManager;
    [SerializeField] private GameObject setting;
    [SerializeField] private GameObject btnHowToPlay;
    [SerializeField] private GameObject btnBooster;
    [SerializeField] private GameObject bannerCoin;


    [Header("Booster")]
    [SerializeField] private Text textBooster;

    [SerializeField] private Image boosterFake;
    [SerializeField] private Image boosterGamePlay;
    [SerializeField] private Image handBooster;
    [SerializeField] private Sprite boosterActive;

    [Header("CheatGamePlay")]
    [SerializeField] private GameObject cheatGamePlay;
    private bool hasReversed = false;
    private int patternIndex = 0;
    public static GameState State { get; private set; }
    private bool checkLineState;
    private bool isActiveCheat = false;
    public bool CheckLineState
    {
        get { return checkLineState; }
        set { checkLineState = value; }
    }
    void OnEnable()
    {
        EventManager.OnAddPoints += ScoreController.Instance.AddPoints;
        EventManager.OnHPchanged += HandleHealthChanged;
        EventDispatcher.Register(EventId.OnHideLineGuide, OnHideLineGuide);
        EventDispatcher.Register(EventId.OnGameStateChanged, OnGameStateChanged);
    }
    void OnDisable()
    {
        EventManager.OnAddPoints -= ScoreController.Instance.AddPoints;
        EventManager.OnHPchanged -= HandleHealthChanged;
        EventDispatcher.RemoveCallback(EventId.OnHideLineGuide, OnHideLineGuide);
        EventDispatcher.RemoveCallback(EventId.OnGameStateChanged, OnGameStateChanged);
    }

    async UniTask Start()
    {
        checkLineState = true;
        Application.targetFrameRate = 60;
        await UniTask.WaitUntil(() => InGameData.GAME_STATE == GameState.LoadingDone);
        EventDispatcher.Push(EventId.OnGamePlayScreen);
        if (!DBController.Instance.TUTORIAL_COMPLETED)
        {
            ChangeState(GameState.Tutorial);
        }
        else
        {
            ChangeState(GameState.PlayingGame);
            TutorialPanel.Instance.ShowTutorial();
            lineCtrl.TurnOnLine();
            lineCtrl.StartFillLoop();
        }
    }
    void Update()
    {
        // Debug.Log($"CheckStateGame: {InGameData.PRE_STATE} - {InGameData.GAME_STATE}");
        if (!TimeManager.Instance
            || InGameData.GAME_STATE == GameState.PauseGame)
            return;

        float elapsed = TimeManager.Instance.ElapsedTime;
        if (elapsed <= 0f) return;
        // Debug.Log($"ElapsedTime: {elapsed} {GameConfig.REVERSE_TIME}");
        int currentStep = Mathf.FloorToInt(elapsed / GameConfig.REVERSE_TIME);
        // Debug.Log($"CurrentPattern: {currentStep} {patternIndex}");
        if (currentStep != patternIndex)
        {
            patternIndex = currentStep;
            ApplyClockPattern((ClockPattern)(patternIndex % 4));
        }
    }
    private void InitData(bool state)
    {
        if (state)
        {
            bannerCoin.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            setting.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            btnHowToPlay.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            btnBooster.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            timeManager.DOFade(1f, 0.5f);
        }
        else
        {
            bannerCoin.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
            setting.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
            btnHowToPlay.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
            btnBooster.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
            timeManager.DOFade(0f, 0.5f);
        }
    }
    public void ChangeState(GameState newState)
    {
        State = newState;

        switch (State)
        {
            case GameState.Tutorial:
                InGameData.GAME_STATE = GameState.Tutorial;
                InitData(false);
                EventDispatcher.Push(EventId.OnTutorialScreen);
                break;
            case GameState.PlayingGame:
                InGameData.GAME_STATE = GameState.PlayingGame;
                break;
            case GameState.GameOver:
                TutorialPanel.Instance.HideTutorial();
                if (DBController.Instance.TUTORIAL_COMPLETED)
                {
                    if (InGameData.GIVE_UP_COUNT != 0)
                    {
                        PopupController.Instance.ClickShowLosePopUp();
                    }
                    else
                    {
                        PopupController.Instance.ClickShowGiveUpPopUp();
                    }
                }
                else
                {
                    PopupController.Instance.ClickShowLosePopUp();
                }
                break;
        }
    }

    public void OnGameStateChanged(object data = null)
    {
        var newState = InGameData.GAME_STATE;
        switch (newState)
        {
            case GameState.PauseGame:
                if (InGameData.PRE_STATE == GameState.UseBooster)
                {
                    BoosterController.Instance.PauseBooster();
                }
                AudioController.Instance.StopEffect(Sound.Name.Sound_Clock);
                AudioController.Instance.StopEffect(Sound.Name.Sound_Clock_Stuck);
                break;
            case GameState.PlayingGame:
                if (InGameData.PRE_STATE == GameState.UseBooster)
                {
                    InGameData.GAME_STATE = GameState.UseBooster;
                    BoosterController.Instance.ContinueBooster();
                    AudioController.Instance.PlayEffect(Sound.Name.Sound_Clock_Stuck);
                }
                else
                {
                    AudioController.Instance.PlayEffect(Sound.Name.Sound_Clock);
                }
                break;
            case GameState.GameOver:
                BoosterController.Instance.CancelBooster();
                AudioController.Instance.StopEffect(Sound.Name.Sound_Clock);
                AudioController.Instance.StopEffect(Sound.Name.Sound_Clock_Stuck);
                break;
            case GameState.UseBooster:
                AudioController.Instance.PlayEffect(Sound.Name.Sound_Clock_Stuck);
                break;
            case GameState.GiveUp:
                AudioController.Instance.StopEffect(Sound.Name.Sound_Clock);
                AudioController.Instance.StopEffect(Sound.Name.Sound_Clock_Stuck);
                break;
        }
    }
    public void HandleHealthChanged(int damage)
    {
        ScoreController.Instance.TakeDamage(damage);

        if (ScoreController.Instance.GetHealthPlayer() <= 0)
        {
            ChangeState(GameState.GameOver);
        }
    }
    public void OnClickShowPausePopup()
    {
        PopupController.Instance.ClickShowPausePopUp();
    }
    public void OnClickShowHowToPlayPopup()
    {
        PopupController.Instance.ClickShowHowToPlayPopUp();
    }
    private void ApplyClockPattern(ClockPattern pattern)
    {
        // var clock = ClockController.Instance;
        // if (clock == null) return;
        // EventDispatcher.Push(EventId.OnClockPatternChanged, pattern);
        // switch (pattern)
        // {
        //     case ClockPattern.ReverseBoth:
        //         clock.ToggleDirection();
        //         Debug.Log($"[Pattern 1] Reverse both hands (elapsed: {TimeManager.Instance.ElapsedTime:F1}s)");
        //         break;

        //     case ClockPattern.HourClockwise_MinuteCCW:
        //         clock.HourHand.SetDirection(clockwise: true);
        //         clock.MinuteHand.SetDirection(clockwise: false);
        //         Debug.Log($"[Pattern 2] Hour clockwise, Minute counter-clockwise");
        //         break;

        //     case ClockPattern.HourCCW_MinuteClockwise:
        //         clock.HourHand.SetDirection(clockwise: false);
        //         clock.MinuteHand.SetDirection(clockwise: true);
        //         Debug.Log($"[Pattern 3] Hour counter-clockwise, Minute clockwise");
        //         break;

        //     case ClockPattern.NormalDirection:
        //         clock.HourHand.SetDirection(clockwise: true);
        //         clock.MinuteHand.SetDirection(clockwise: true);
        //         Debug.Log($"[Pattern 4] Both back to normal");
        //         break;
        // }
    }

    #region LineTutorial
    private void OnHideLineGuide(object data = null)
    {
        Debug.Log("CheckEventHidePopup");
        TutorialPanel.Instance.HideTutorial();
        checkLineState = false;
        TimeManager.Instance.StartTimer();
        AudioController.Instance.FadeBackgroundForAlert(0.3f);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Clock);
        InitData(true);

        if (DBController.Instance.GUIDE_BOOSTER == 0)
            BoosterGuide();
    }

    private async UniTask BoosterGuide()
    {
        await UniTask.Delay(1000);

        InGameData.GAME_STATE = GameState.PauseGame;
        TutorialPanel.Instance.ShowTutorial(() =>
        {
            boosterFake.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                handBooster.DOFade(1f, 0.3f);
                boosterGamePlay.transform.DOScale(0f, 0.3f);
            });
        });

        await UniTask.WaitUntil(() => DBController.Instance.GUIDE_BOOSTER == 1);
        boosterFake.transform.DOScale(0f, 0.3f);
        boosterGamePlay.transform.DOScale(1f, 0.3f);
        boosterFake.sprite = boosterActive;
        boosterGamePlay.sprite = boosterActive;
        TutorialPanel.Instance.HideTutorial();
    }
    #endregion

    #region CheatGamePlay

    public void OnClickShowCheatPanel()
    {
        isActiveCheat = !isActiveCheat;
        cheatGamePlay.SetActive(isActiveCheat);
    }
    public void OnClickCheatNextTutorial()
    {
        TutorialPanel.Instance.HideTutorial();
        DBController.Instance.TUTORIAL_COMPLETED = true;
        // SceneController.Instance?.ChangeScene(SceneType.GamePlayScene);
    }

    public void CheatAddCoin(int amount)
    {
        DBController.Instance.COIN += amount;
        EventDispatcher.Push(EventId.OnCoinChanged, DBController.Instance.COIN);
        Debug.Log($"[CHEAT] Added {amount} coins. Current coin: {DBController.Instance.COIN}");
    }
    #endregion
}
