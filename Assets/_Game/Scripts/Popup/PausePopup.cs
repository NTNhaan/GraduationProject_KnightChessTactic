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

public class PausePopup : PopUpBase
{
    [Header("Pause Menu")]
    [SerializeField] private Image imgSound;
    [SerializeField] private Image imgMusic;
    [SerializeField] private Image imgVibration;
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
        PopupController.Instance?.HidePausePopUp( () =>
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
        PopupController.Instance?.HidePausePopUp( () =>
        {
            InGameData.GAME_STATE = GameState.Loading;
            InGameData.NEXT_STATE = GameState.SelectSkin;
            InGameData.GAME_SCENE = SceneType.GamePlayScene;
            InGameData.RestartGame = true;
            InGameData.NextLevel = false;
            SceneController.Instance?.ChangeScene(SceneType.GamePlayScene);
        });
    }
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
}
