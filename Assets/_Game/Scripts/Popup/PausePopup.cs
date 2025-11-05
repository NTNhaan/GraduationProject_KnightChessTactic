using System;
using Audio;
using Data;
using DG.Tweening;
using Popup;
using Setting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;

public class PausePopup : PopUpBase
{
    [Header("Pause Menu")]
    [SerializeField] private Image imgSound;
    [SerializeField] private Image imgMusic;
    [SerializeField] private Image imgVibration;
    [SerializeField] private Button btnRestart;
    [SerializeField] private Button btnLoadHome; 
    [SerializeField] private Button btnQuit;
    [SerializeField] private ButtonType[] sprtSound;
    [SerializeField] private ButtonType[] sprtMusic;
    [SerializeField] private ButtonType[] sprtVibration;
    private bool isPause = false;
    
    
    #region Overrides Func
    public override void ShowPopUp(float posY, float duration, UnityAction onComplete = null)
    {
        ShowCover(0.5f, () =>
        {
            InitSetting();
            base.ShowPopUp(posY, duration, onComplete);
        });
    }
    public override void HidePopUp(float posY, float duration, UnityAction onComplete = null)
    {
        base.HidePopUp(posY, duration, () =>
        {
            onComplete?.Invoke();
            // tfmPopup.gameObject.SetActive(false);
            HideCover();
        });
    }

    public override void ShowCover(float duration = 0.5f, UnityAction onComplete = null)
    {
        base.ShowCover(duration, onComplete);
    }


    public override void HideCover(UnityAction onComplete = null)
    {
        base.HideCover(onComplete);
    }
    #endregion
    
    public void OnClickContinueGame()
    {
        HidePopUp(-1800f, 1f, () =>
        {
            EventDispatcher.Push(EventId.OnSoundClick);
        });
    }
    public void OnClickLoadMainMenu()
    {
        EventDispatcher.Push(EventId.OnSoundClick);
        DoHidePausePopUp( () =>
        {
            InGameData.GAME_STATE = GameState.Loading;
            InGameData.NEXT_STATE = GameState.SelectSkin;
            InGameData.GAME_SCENE = SceneType.MainScene;
            InGameData.RestartGame = false;
            InGameData.NextLevel = false;
            SceneController.Instance?.ChangeScene(SceneType.MainScene);
        });
    }
    public void OnClickRestartGame()
    {
        EventDispatcher.Push(EventId.OnSoundClick);
        DoHidePausePopUp( () =>
        {
            InGameData.GAME_STATE = GameState.Loading;
            InGameData.NEXT_STATE = GameState.SelectSkin;
            InGameData.GAME_SCENE = SceneType.GamePlayScene;
            InGameData.RestartGame = true;
            InGameData.NextLevel = false;
            SceneController.Instance?.ChangeScene(SceneType.GamePlayScene);
        });
    }
    
    #region PausePopup
    public void ShowPausePopUp()
    {
        InitStateButton(true);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        InGameData.PRE_STATE = InGameData.GAME_STATE;
        InGameData.GAME_STATE = GameState.PauseGame;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        EventManager.PasueGame();
        ShowPopUp(0f, 0.5f, () =>
        {
            DoShowPausePopup();
        });
    }
    public void HidePausePopup()
    {
        EventDispatcher.Push(EventId.OnSoundClick);
        DoHidePausePopUp();
    }
    [ContextMenu("Show Pause Popup")]
    public async UniTask DoShowPausePopup()
    {
        var task1 = btnRestart.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).ToUniTask();
        var task2 = btnLoadHome.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).ToUniTask();
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        await UniTask.WhenAll(task1, task2);
        
        imgSound.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        imgMusic.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        imgVibration.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        btnQuit.interactable = true;
        btnQuit.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
    }
    [ContextMenu("Hide Pause Popup")]
    public async UniTask DoHidePausePopUp(UnityAction onCompleted = null)
    {
        InitStateButton(false);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupClose);
        var t1 = btnQuit.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var t2 = btnRestart.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var t3 = btnLoadHome.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.WhenAll(t1, t2, t3);
        var t4 = imgSound.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var t5 = imgMusic.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var t6 = imgVibration.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.WhenAll(t4, t5, t6);
        
        HidePopUp(-1800f, 0.5f, ()=>
        {
            InGameData.GAME_STATE = GameState.PlayingGame;
            EventDispatcher.Push(EventId.OnGameStateChanged);
            EventManager.ResumeGame();  
            
            onCompleted?.Invoke();
        });
    }
    #endregion
    
    #region Setting Buttons
    public void OnClickSoundBtn()
    {
        EventDispatcher.Push(EventId.OnSoundClick);
        SettingCtrl.Instance.SetSound();
        SettingCtrl.Instance.UpdateSettingImage(imgSound, sprtSound, DBController.Instance.SOUND);
    }

    public void OnClickMusicBtn()
    {
        EventDispatcher.Push(EventId.OnSoundClick);
        SettingCtrl.Instance.SetMusic();
        SettingCtrl.Instance.UpdateSettingImage(imgMusic, sprtMusic, DBController.Instance.MUSIC);
    }

    public void OnClickVibrateBtn()
    {
        Debug.Log($"CheckClick 1");
        EventDispatcher.Push(EventId.OnSoundClick);
        SettingCtrl.Instance?.SetVibration();
        SettingCtrl.Instance?.UpdateSettingImage(imgVibration, sprtVibration, DBController.Instance.VIBRATE);
    }
    public void InitSetting()
    {
        Debug.Log($"InitSettingPause");
        SettingCtrl.Instance.UpdateSettingImage(imgSound, sprtSound, DBController.Instance.SOUND);
        SettingCtrl.Instance.UpdateSettingImage(imgVibration, sprtVibration, DBController.Instance.VIBRATE);
        SettingCtrl.Instance.UpdateSettingImage(imgMusic, sprtMusic, DBController.Instance.MUSIC);
    }
    public void InitStateButton(bool state)
    {
        btnQuit.interactable = state;
        btnRestart.interactable = state;
        btnLoadHome.interactable = state;
    }
    #endregion
}
