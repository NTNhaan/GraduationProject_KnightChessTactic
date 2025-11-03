using System;
using Audio;
using Data;
using DefaultNamespace;
using DG.Tweening;
using Popup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LosePopup : PopUpBase
{
    [SerializeField] private Text highScorePanel;
    [SerializeField] private Text timePanel;

    private void OnEnable()
    {
        EventDispatcher.Register(EventId.OnBestScoreChange, OnHandleBestScoreChange);
        EventDispatcher.Register(EventId.OnSaveTime, OnTimeChange);
    }

    private void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.OnBestScoreChange, OnHandleBestScoreChange);
        EventDispatcher.RemoveCallback(EventId.OnSaveTime, OnTimeChange);
    }
    
    #region Overrides Func
    public override void ShowPopUp(float posY, float duration, UnityAction onComplete = null)
    {
        ShowCover(0.5f, () => { base.ShowPopUp(posY, duration, onComplete); });
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


    private void Start()
    {
        highScorePanel.text = DBController.Instance.BEST_SCORE.ToString();
    }

    public void OnClickRestartGame()
    {
        InGameData.GIVE_UP_COUNT = 0;
        EventDispatcher.Push(EventId.OnSoundClick);
        PopupController.Instance?.HideGameOverPopUp( () =>
        {
            InGameData.RestartGame = true; // nen luu bien dem trong database
            InGameData.NextLevel = false;
            ScoreController.Instance.Score = 0;
            InGameData.GAME_STATE = GameState.Loading;
            InGameData.GAME_SCENE = SceneType.GamePlayScene;
            SceneController.Instance?.ChangeScene(InGameData.GAME_SCENE);
        });
    }

    public void OnClickLoadMainMenu()
    {
        InGameData.GIVE_UP_COUNT = 0;
        EventDispatcher.Push(EventId.OnSoundClick);
        PopupController.Instance?.HideGameOverPopUp( () =>
        {
            InGameData.RestartGame = false;
            InGameData.NextLevel = false;

            InGameData.GAME_STATE = GameState.Loading;
            InGameData.NEXT_STATE = GameState.SelectSkin;
            InGameData.GAME_SCENE = SceneType.MainScene;
            SceneController.Instance?.ChangeScene(SceneType.MainScene);
        });
    }

    public void OnHandleBestScoreChange(object data = null)
    {
        Debug.Log($"CheckScorePanel {highScorePanel.text}");
        highScorePanel.text = DBController.Instance.BEST_SCORE.ToString();
    }

    public void OnTimeChange(object data = null)
    {
        TimeSpan time = TimeSpan.FromSeconds(DBController.Instance.LAST_PLAY_TIME);
        timePanel.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
    }
}