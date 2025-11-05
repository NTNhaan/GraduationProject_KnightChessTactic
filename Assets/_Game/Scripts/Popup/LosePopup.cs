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
using Cysharp.Threading.Tasks;
public class LosePopup : PopUpBase
{
    [Header("Lose Popup")]
    [SerializeField] private RectTransform character;
    [SerializeField] private Image shine;
    [SerializeField] private Button btnRetry;
    [SerializeField] private Button btnHome;
    
    [SerializeField] private Text highScorePanel;
    [SerializeField] private Text timePanel;

    private void OnEnable()
    {
        EventDispatcher.Register(EventId.OnBestScoreChange, OnHandleBestScoreChange);
        EventDispatcher.Register(EventId.OnSaveTime, OnTimeChange);
        EventDispatcher.Register(EventId.OnHidePopupReward, OnHidePopupReward);
    }

    private void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.OnBestScoreChange, OnHandleBestScoreChange);
        EventDispatcher.RemoveCallback(EventId.OnSaveTime, OnTimeChange);
        EventDispatcher.RemoveCallback(EventId.OnHidePopupReward, OnHidePopupReward);
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
        HideGameOverPopUp( () =>
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
        HideGameOverPopUp( () =>
        {
            InGameData.RestartGame = false;
            InGameData.NextLevel = false;

            InGameData.GAME_STATE = GameState.Loading;
            InGameData.NEXT_STATE = GameState.SelectSkin;
            InGameData.GAME_SCENE = SceneType.MainScene;
            SceneController.Instance?.ChangeScene(SceneType.MainScene);
        });
    }

    #region Event Handlers
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
    public void OnHidePopupReward(object data = null)
    {
        btnRetry.transform.DOScale(1f, .3f).SetEase(Ease.OutBack);
        btnHome.transform.DOScale(1f, .3f).SetEase(Ease.OutBack);
    }
    #endregion
    
    
    #region LosePopup
    [ContextMenu("Show GameOver Popup")]
    public void ShowGameOverPopUp()
    {
        InitStateButton(true);
        InGameData.GAME_STATE = GameState.GameOver;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        ShowPopUp(0f, 0.5f, () =>
        {
            DoShowLosePopup();
        });
    }
    [ContextMenu("Hide GameOver Popup")]
    public async UniTask HideGameOverPopUp(UnityAction onComplete = null)
    {
        InitStateButton(false);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupClose);
        var task = btnRetry.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var task1 = btnHome.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.WhenAll(task, task1);
        shine.DOFade(0f, 0.5f);
        // coinBanner.DOScale(0f, 0.3f).SetEase(Ease.OutBack);
        character.DOAnchorPosY(0f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            HidePopUp(-2000f, 0.5f); 
            onComplete?.Invoke();
        });
    }

    public async UniTask DoShowLosePopup()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Progress);
        character.DOAnchorPosY(600, .5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            AudioController.Instance.PlayEffect(Sound.Name.Sound_Reward);
            shine.DOFade(0.3f, .5f);
            
            var time = TimeManager.Instance.ElapsedTime;
            if (DBController.Instance.TUTORIAL_COMPLETED && time >= 30f)
            {
                PopupController.Instance.ClickShowRewardPopUp();
            }
            else
            {
                btnRetry.transform.DOScale(1f, .3f).SetEase(Ease.OutBack);
                btnHome.transform.DOScale(1f, .3f).SetEase(Ease.OutBack);
            }
        });
    }
    public void InitStateButton(bool state)
    {
        btnRetry.interactable = state;
        btnHome.interactable = state;
    }
    #endregion
}