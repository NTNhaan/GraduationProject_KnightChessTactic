using System;
using Audio;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Setting;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Data;
public class MainScreen : ScreenBase
{
    [Header("===== UI Setting =====")]
    [SerializeField] private Image imgSound;
    [SerializeField] private Image imgMusic;
    [SerializeField] private Image imgVibration;

    [SerializeField] private ButtonType[] sprtSound;
    [SerializeField] private ButtonType[] sprtMusic;
    [SerializeField] private ButtonType[] sprtVibration;
    
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
        
        await UniTask.WaitUntil(()=> InGameData.GAME_STATE ==GameState.LoadingDone);
        topAnim.SetBool("isShow", true);
        bottomAnim.SetBool("isShow", true);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);

    }
    #region Override Methods
    public override void ShowScreen(UnityAction onComplete)
    {
        LoadUI();
        base.ShowScreen(onComplete);
    }

    public override void HideScreen(UnityAction onComplete)
    {
        base.HideScreen(onComplete);
    }

    public override void LoadUI()
    {
        base.LoadUI();
    }

    public void UpdateCoinUI(object data = null)
    {
        CoinText.text = DBController.Instance.COIN.ToString("n0");
    }
    #endregion
    
    public void ClickStartButton()
    {
        var level = DBController.Instance.LEVEL;
        InGameData.GAME_SCENE = SceneType.GamePlayScene;
        InGameData.GAME_STATE = GameState.Loading;
        SceneController.Instance?.ChangeScene(InGameData.GAME_SCENE);
    }
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
    public void OnClickShowLeaderBoard()
    {
        PopupController.Instance?.ShowLeaderBoardPopUp();
    }
    public void InitGame()
    {
        textLevel.text = DBController.Instance.LEVEL.ToString();
        hightScoreText.text = DBController.Instance.BEST_SCORE.ToString();
        
        UpdateCoinUI();
        SettingCtrl.Instance.InjectUI(imgSound, imgMusic, imgVibration,
            sprtSound, sprtMusic, sprtVibration);
        
        // SettingCtrl.Instance.InitSetting();
        if(DBController.Instance.MUSIC)
            AudioController.Instance.PlayMusic(Sound.Name.Music_Menu);
    }
}
