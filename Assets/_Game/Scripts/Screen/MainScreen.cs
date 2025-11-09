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

public class MainScreen : ScreenBase
{
    [Header("Main Screen")]
    [SerializeField] private Button dailyReward;
    [SerializeField] private Button spinReward;
    [SerializeField] private Button levelModeBtn;
    [SerializeField] private Button classicModeBtn;
    [SerializeField] private Button PveModeBtn;
    [SerializeField] private RectTransform topBanner;
    [SerializeField] private RectTransform logoInGame;


    [Header("=====HightScore MainScene=====")]
    [SerializeField] private Text hightScoreText;
    [SerializeField] private Text CoinText;
    [SerializeField] private Text textLevel;
    [SerializeField] private Animator topAnim;
    [SerializeField] private Animator bottomAnim;

    public void OnEnable()
    {
        EventDispatcher.Register(EventId.OnCoinChanged, UpdateCoinUI);
    }
    public void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.OnCoinChanged, UpdateCoinUI);
    }
    async UniTask Start()
    {
        InitGame();

        await UniTask.WaitUntil(() => InGameData.GAME_STATE == GameState.LoadingDone);
        topAnim.SetBool("isShow", true);
        bottomAnim.SetBool("isShow", true);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);

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

    public void UpdateCoinUI(object data = null)
    {
        CoinText.text = DBController.Instance.COIN.ToString("n0");
    }
    #endregion

    public void ClickStartButton()
    {
        var level = DBController.Instance.LEVEL;
        // InGameData.GAME_SCENE = SceneType.GamePlayScene;
        InGameData.GAME_STATE = GameState.Loading;
        SceneController.Instance?.ChangeScene(InGameData.GAME_SCENE);
    }
    public void OnClickShowLeaderBoard()
    {
        PopupController.Instance?.ClickShowLeaderBoardPopUp();
    }
    public void InitGame()
    {
        textLevel.text = DBController.Instance.LEVEL.ToString();
        hightScoreText.text = DBController.Instance.BEST_SCORE.ToString();

        UpdateCoinUI();
        if (DBController.Instance.MUSIC)
            AudioController.Instance.PlayMusic(Sound.Name.Music_Menu);
    }

    #region Show/Hide Main Screen
    public async UniTask DoShowMainScreen()
    {
        InitStateButton(true);
        await topBanner.DOAnchorPosY(0, 5f).SetEase(Ease.OutBack).ToUniTask();

        var t1 = spinReward.transform.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
        var t2 = dailyReward.transform.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        await UniTask.WhenAll(t1, t2);

        var t4 = PveModeBtn.transform.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
        var t5 = levelModeBtn.transform.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
        var t6 = classicModeBtn.transform.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        EventDispatcher.Push(EventId.OnMainScreen);
        await UniTask.WhenAll(t4, t5, t6);
    }

    public async UniTask DoHideMainScreen(UnityAction onComplete = null)
    {
        // InitStateButton(false);
        // var t4 = playBtn.transform.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();
        // var t5 = leaderBoard.transform.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();
        // var t6 = shopIAP.transform.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();
        // var t7 = bannerHighScore.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();

        // var t1 = imgSound.transform.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();
        // var t2 = imgVibration.transform.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();
        // var t3 = imgMusic.transform.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();

        // await UniTask.WhenAll(t1, t2, t3, t4, t5, t6, t7);

        // onComplete?.Invoke();

        // logoInGame.DOAnchorPosY(-1000, .3f).SetEase(Ease.InBack);
        // logoInGame.DOScale(0f, .3f).SetEase(Ease.InBack);
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
    public void InitStateButton(bool state)
    {
        // playBtn.interactable = state;
        // leaderBoard.interactable = state;
        // shopIAP.interactable = state;
    }
    #endregion
}
