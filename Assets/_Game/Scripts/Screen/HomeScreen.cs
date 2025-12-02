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
    // [SerializeField] private Button btnDailyReward;
    [SerializeField] private Animator btnDailyReward;
    [SerializeField] private Animator btnSpinReward;
    [SerializeField] private Button btnLevelMode;
    [SerializeField] private Button btnClassicMode;
    [SerializeField] private Button btnPveMode;
    
    [Header("Effect Logo")]
    [SerializeField] private RectTransform rectShield;
    [SerializeField] private Transform trfBannerLogo;
    [SerializeField] private Transform trfKTxt;
    [SerializeField] private Transform trfNTxt;
    [SerializeField] private Transform trfITxt;
    [SerializeField] private Transform trfGTxt;
    [SerializeField] private Transform trfHTxt;
    [SerializeField] private Transform trfTTxt;
    [SerializeField] private Transform trfChessTacticTxt;
    [SerializeField] private Transform trfRibbon;
    [SerializeField] private RectTransform rectSwordLeft;
    [SerializeField] private RectTransform rectSwordRight;
    
    [Header("=====HightScore MainScene=====")]
    [SerializeField] private Text hightScoreText;
    [SerializeField] private Text coinText;
    [SerializeField] private Text textLevel;
    
    private bool stopIdleAnim = false;

    public void OnEnable()
    {
        EventDispatcher.Register(EventId.OnCoinChanged, UpdateCoinUI);
    }
    public void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.OnCoinChanged, UpdateCoinUI);
        stopIdleAnim = true;
    }
    async UniTask Start()
    {
        // InitGame();

        await UniTask.WaitUntil(() => InGameData.GAME_STATE == GameState.LoadingDone);
        // topAnim.SetBool("isShow", true);
        // bottomAnim.SetBool("isShow", true);
        // AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        StartIdleLoop();
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
        coinText.text = DBController.Instance.COIN.ToString("n0");
    }
    #endregion

    public void ClickStartButton()
    {
        var level = DBController.Instance.LEVEL;
        InGameData.GAME_SCENE = SceneType.GameScene;
        InGameData.GAME_STATE = GameState.Loading;
        DoHideMainScreen().ContinueWith(()=> SceneController.Instance.ChangeScene(InGameData.GAME_SCENE));
    }
    public void OnClickShowPausePopup()
    {
        PopupController.Instance.ClickShowPausePopUp();
    }
    public void OnClickShowDailyRW()
    {
        PopupController.Instance.ClickShowDailyRWPopUp();
    }
    public void OnClickShowSpinRW()
    {
        PopupController.Instance.ClickShowSpinRWPopUp();
    }
    public void InitGame()
    {
        textLevel.text = DBController.Instance.LEVEL.ToString();
        hightScoreText.text = DBController.Instance.BEST_SCORE.ToString();

        UpdateCoinUI();
        if (DBController.Instance.MUSIC)
            AudioController.Instance.PlayMusic(Sound.Name.Music_Menu);
    }
    private Animator GetAnimatorByType(IdleAnimType type)
    {
        switch (type)
        {
            case IdleAnimType.DailyReward:
                return btnDailyReward;
            case IdleAnimType.SpinReward:
                return btnSpinReward;
            default:
                return null;
        }
    }
    private async UniTask PlayIdleAnimation(IdleAnimType type)
    {
        Animator anim = GetAnimatorByType(type);
        if (anim == null) return;

        anim.SetBool("isJump", true);
        if (type == IdleAnimType.DailyReward)
        {
            await AnimatorHelper.Instance.WaitForStateComplete(anim, "Anim_Gift");
        }
        else
        {
            await AnimatorHelper.Instance.WaitForStateComplete(anim, "Anim_SpinRun");
        }
        anim.SetBool("isJump", false);
    }
    private async void StartIdleLoop()
    {
        stopIdleAnim = false;

        while (!stopIdleAnim)
        {
            float waitTime = UnityEngine.Random.Range(4f, 9f);
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime),
                cancellationToken: this.GetCancellationTokenOnDestroy());

            if (stopIdleAnim) break;

            // random pick 1 trong 2
            IdleAnimType chosen = (UnityEngine.Random.value > 0.5f)
                ? IdleAnimType.DailyReward
                : IdleAnimType.SpinReward;

            await PlayIdleAnimation(chosen);
        }
    }
    #region Show/Hide Main Screen
    public async UniTask DoShowMainScreen() // knight
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        await rectShield.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        await trfBannerLogo.DOScale(1f, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        await trfKTxt.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        await trfNTxt.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        await trfITxt.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        await trfGTxt.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        await trfHTxt.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        await trfTTxt.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        
        trfChessTacticTxt.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        await trfRibbon.DOScaleX(1f, 0.5f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();

        Vector2 swordRightPos = rectShield.anchoredPosition + new Vector2(217, 5);
        rectSwordRight.anchoredPosition = new Vector2(1000, swordRightPos.y + 800);
        await rectSwordRight.DOAnchorPos(swordRightPos, 0.3f)
            .SetEase(Ease.OutBack)  
            .AsyncWaitForCompletion();
        
        Vector2 swordLeftPos = rectShield.anchoredPosition + new Vector2(-217, 5);
        rectSwordLeft.anchoredPosition = new Vector2(-1000, swordLeftPos.y + 800);
        await rectSwordLeft.DOAnchorPos(swordLeftPos, 0.3f)
            .SetEase(Ease.OutBack)  
            .AsyncWaitForCompletion();

        UITopController.Instance.ShowTab();
        var t1 = btnSpinReward.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        var t2 = btnDailyReward.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        var t3 = btnPveMode.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        var t4 = btnLevelMode.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        var t5 = btnClassicMode.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        await UniTask.WhenAll(t1, t2, t3, t4, t5);
        
        TabController.Instance.ShowBottomTab();
        EventDispatcher.Push(EventId.OnMainScreen);
    }

    public async UniTask DoHideMainScreen(UnityAction onComplete = null)
    {
        UITopController.Instance.HideTab();
        TabController.Instance.HideBottomTab();
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
    #endregion
}

public enum IdleAnimType
{
    DailyReward,
    SpinReward
}