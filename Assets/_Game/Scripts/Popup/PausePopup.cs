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
    [SerializeField] private Button btnQuit;
    [SerializeField] private Button btnHome;
    [SerializeField] private GameObject btnLeft;
    [SerializeField] private GameObject btnRight;
    [SerializeField] private ButtonType[] sprtSound;
    [SerializeField] private ButtonType[] sprtMusic;
    [SerializeField] private ButtonType[] sprtVibration;
    private bool isPause = false;
    private bool isShowing = false;


    #region Overrides Func
    public override void ShowPopUp(float posY, float duration, UnityAction onComplete = null)
    {
        if (imgCover != null && !imgCover.gameObject.activeSelf)
        {
            imgCover.gameObject.SetActive(true);
            imgCover.color = new Color(imgCover.color.r, imgCover.color.g, imgCover.color.b, 0f); // Reset alpha
        }

        ShowCover(0.5f, () =>
        {
            InitSetting();
            UpdateButtonRightVisibility();
            base.ShowPopUp(posY, duration, onComplete);
        });
    }

    private void UpdateButtonRightVisibility()
    {
        if (btnRight != null)
        {
            btnRight.SetActive(InGameData.GAME_SCENE == SceneType.GameScene);
        }
    }
    public override void HidePopUp(float posY, float duration, UnityAction onComplete = null)
    {
        base.HidePopUp(posY, duration, () =>
        {
            onComplete?.Invoke();
            HideCover();
        });
    }

    public override void ShowCover(float duration = 0.5f, UnityAction onComplete = null)
    {
        // Đảm bảo cover được show ngay lập tức (không async)
        if (imgCover != null)
        {
            imgCover.gameObject.SetActive(true);
        }
        base.ShowCover(duration, onComplete);
    }


    public override void HideCover(UnityAction onComplete = null)
    {
        base.HideCover(onComplete);
    }
    #endregion

    public async void OnClickContinueGame()
    {
        InitStateButton(false);
        await ScaleButtonsToZero();
        HidePopUp(2000f, 0.5f, () =>
        {
            isShowing = false; // Reset flag
            InGameData.GAME_STATE = GameState.PlayingGame;
            EventDispatcher.Push(EventId.OnGameStateChanged);
            EventManager.ResumeGame();
            EventDispatcher.Push(EventId.OnSoundClick);
        });
    }

    public async void OnClickLoadMainMenu()
    {
        EventDispatcher.Push(EventId.OnSoundClick);

        InitStateButton(false);
        UITopController.Instance.HideTab();
        await ScaleButtonsToZero();

        await HidePopUpAsync(2000f, 0.5f);

        isShowing = false;
        InGameData.GAME_STATE = GameState.Loading;
        InGameData.GAME_SCENE = SceneType.MainScene;
        InGameData.RestartGame = false;
        InGameData.NextLevel = false;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        EventManager.PasueGame(); // Pause game khi load main menu

        // Change scene và await để đảm bảo hoàn thành
        if (SceneController.Instance != null)
        {
            await SceneController.Instance.ChangeScene(SceneType.MainScene);
        }
    }

    private async UniTask HidePopUpAsync(float posY, float duration)
    {
        var completionSource = new UniTaskCompletionSource();

        HidePopUp(posY, duration, () =>
        {
            completionSource.TrySetResult();
        });

        await completionSource.Task;
    }

    private async UniTask ScaleButtonsToZero()
    {
        // Scale tất cả buttons về 0
        var tasks = new System.Collections.Generic.List<UniTask>();

        if (btnQuit != null)
        {
            tasks.Add(btnQuit.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).AsyncWaitForCompletion().AsUniTask());
        }

        if (btnHome != null && btnHome.gameObject.activeSelf)
        {
            tasks.Add(btnHome.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).AsyncWaitForCompletion().AsUniTask());
        }

        if (imgSound != null)
        {
            tasks.Add(imgSound.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).AsyncWaitForCompletion().AsUniTask());
        }

        if (imgMusic != null)
        {
            tasks.Add(imgMusic.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).AsyncWaitForCompletion().AsUniTask());
        }

        if (imgVibration != null)
        {
            tasks.Add(imgVibration.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).AsyncWaitForCompletion().AsUniTask());
        }

        await UniTask.WhenAll(tasks);
    }
    public void OnClickRestartGame()
    {
        EventDispatcher.Push(EventId.OnSoundClick);
        // DoHidePausePopUp( () =>
        // {
        //     InGameData.GAME_STATE = GameState.Loading;
        //     InGameData.NEXT_STATE = GameState.SelectSkin;
        //     InGameData.GAME_SCENE = SceneType.GamePlayScene;
        //     InGameData.RestartGame = true;
        //     InGameData.NextLevel = false;
        //     SceneController.Instance?.ChangeScene(SceneType.GamePlayScene);
        // });
    }

    #region PausePopup
    public void ShowPausePopUp()
    {
        if (isShowing || (TfmPopup != null && TfmPopup.gameObject.activeSelf) || (imgCover != null && imgCover.gameObject.activeSelf))
        {
            return;
        }

        isShowing = true;
        InitStateButton(true);
        // AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        InGameData.PRE_STATE = InGameData.GAME_STATE;
        InGameData.GAME_STATE = GameState.PauseGame;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        EventManager.PasueGame();
        ShowPopUp(100f, 0.5f, () =>
        {
            DoShowPausePopup();
        });
    }
    public void HidePausePopup()
    {
        // EventDispatcher.Push(EventId.OnSoundClick);
        DoHidePausePopUp();
    }
    [ContextMenu("Show Pause Popup")]
    public async UniTask DoShowPausePopup()
    {
        InitStateButton(true);
        // Đảm bảo btnRight được set đúng theo scene (override InitStateButton nếu cần)
        UpdateButtonRightVisibility();

        await imgSound.transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        await imgMusic.transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        await imgVibration.transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        btnQuit.interactable = true;
        await btnQuit.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();

        // Chỉ DOScale button home nếu đang ở GameScene
        if (InGameData.GAME_SCENE == SceneType.GameScene)
        {
            btnHome.interactable = true;
            await btnHome.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        }
        // AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
    }
    [ContextMenu("Hide Pause Popup")]
    public async UniTask DoHidePausePopUp(UnityAction onCompleted = null)
    {
        InitStateButton(false);
        // AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupClose);
        await btnQuit.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).AsyncWaitForCompletion().AsUniTask();
        var t4 = imgSound.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).AsyncWaitForCompletion().AsUniTask();
        var t5 = imgMusic.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).AsyncWaitForCompletion().AsUniTask();
        var t6 = imgVibration.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).AsyncWaitForCompletion().AsUniTask();
        await UniTask.WhenAll(t4, t5, t6);

        HidePopUp(2000f, 0.5f, () =>
        {
            isShowing = false; // Reset flag khi popup đã đóng hoàn toàn
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
        EventDispatcher.Push(EventId.OnSoundClick);
        SettingCtrl.Instance?.SetVibration();
        SettingCtrl.Instance?.UpdateSettingImage(imgVibration, sprtVibration, DBController.Instance.VIBRATE);
    }
    public void InitSetting()
    {
        SettingCtrl.Instance.UpdateSettingImage(imgSound, sprtSound, DBController.Instance.SOUND);
        SettingCtrl.Instance.UpdateSettingImage(imgVibration, sprtVibration, DBController.Instance.VIBRATE);
        SettingCtrl.Instance.UpdateSettingImage(imgMusic, sprtMusic, DBController.Instance.MUSIC);
    }
    public void InitStateButton(bool state)
    {
        btnQuit.interactable = state;
        btnHome.interactable = state;
    }
    #endregion
}
