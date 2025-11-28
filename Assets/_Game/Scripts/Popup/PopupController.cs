using System;
using System.Threading.Tasks;
using Audio;
using UnityEngine;
using Data;
using DG.Tweening;
using Popup;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class PopupController : Singleton<PopupController>
{
    [Header("Coin UI")] 
    [SerializeField] private Transform coinBanner;
    [SerializeField] private Text numberCoin;
    
    [SerializeField] private Transform shopBtn;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Transform firstTransform;
    
    [Header("Lose Popup")]
    [SerializeField] private LosePopup popUpGameOver;
    
    [Header("GiveUp Popup")]
    [SerializeField] private GiveUpPopup popUpGiveUp;
    
    [Header("Pause Popup")]
    [SerializeField] private PausePopup popUpPauseGame;
    
    [Header("LeaderBoard Popup")]
    [SerializeField] private LeaderBoardPopup popUpLeaderBoard;
    
    [Header("HowToPlay Popup")]
    [SerializeField] private HowToPlayPopup popUpHowToPlay;
    
    [Header("Confirm Popup")]
    [SerializeField] private ConfirmPopup popUpConfirm;
    
    [Header("Notify Popup")]
    [SerializeField] private NotifyPopup  popUpNotify;
    
    [Header("Reward Popup")]
    [SerializeField] private RewardPopup popUpReward;

    [Header("Daily Reward Popup")]
    [SerializeField] private DailyRewardPopup popUpDailyRW;
    
    [Header("Spin Reward Popup")]
    [SerializeField] private SpinRewardPopup popUpSpinRW;
    
    private bool isFirstClick = true;

    public void OnEnable()
    {
        EventDispatcher.Register(EventId.OnCoinChanged, UpdateCoinUI);
    }
    public void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.OnCoinChanged, UpdateCoinUI);
    }

    private void UpdateCoinUI(object data = null)
    {
        numberCoin.text = DBController.Instance.COIN.ToString();
    }
    
    #region ------------LosePopup------------
    [ContextMenu("Show Lose Popup")]
    public void ClickShowLosePopUp()
    {
        popUpGameOver.ShowGameOverPopUp();
    }    
    [ContextMenu("Hide Lose Popup")]
    public void ClickHideLosePopUp()
    {
        popUpGameOver.HideGameOverPopUp();
    }
    #endregion
    
    #region ------------GiveUpPopup------------
    [ContextMenu("Show GiveUp Popup")]
    public void ClickShowGiveUpPopUp()
    {
        popUpGiveUp.ShowGiveUpPopUp();
    }    
    [ContextMenu("Hide GiveUp Popup")]
    public void ClickHideGiveUpPopUp()
    {
        popUpGiveUp.HideGiveUpPopUp();
    }
    #endregion
    
    #region ------------PausePopup------------
    [ContextMenu("Show Pause Popup")]
    public void ClickShowPausePopUp()
    {
        popUpPauseGame.ShowPausePopUp();
    }    
    [ContextMenu("Hide Pause Popup")]
    public void ClickHidePausePopUp()
    {
        popUpPauseGame.HidePausePopup();
    }
    #endregion
    
    #region ------------LeaderBoardPopup------------
    
    [ContextMenu("Show LeaderBoard Popup")]
    public void ClickShowLeaderBoardPopUp()
    {
        popUpLeaderBoard.ShowLeaderBoardPopUp();
    }    
    [ContextMenu("Hide LeaderBoard Popup")]
    public void ClickHideLeaderBoardPopUp()
    {
        popUpLeaderBoard.HideLeaderBoardPopUp();
    }
    #endregion
    
    #region ------------HowToPlayPopup------------
    [ContextMenu("Show HowToPlay Popup")]
    public void ClickShowHowToPlayPopUp()
    {
        popUpHowToPlay.ShowHowToPlayPopUp();
    }    
    [ContextMenu("Hide HowToPlay Popup")]
    public void ClickHideHowToPlayPopUp()
    {
        popUpHowToPlay.HideHowToPlayPopUp();
    }
    #endregion
    
    #region ------------ConfirmPopup------------
    [ContextMenu("Show Confirm Popup")]
    public void ClickShowConfirmPopUp(UnityAction onYes = null, UnityAction onNo = null)
    {
        popUpConfirm.ShowConfirmPopUp(onYes, onNo);
    } 
    [ContextMenu("Hide Confirm Popup")]
    public void ClickHideConfirmPopUp()
    {
        popUpConfirm.HideConfirmPopUp();
    }
    #endregion
    
    #region ------------NotifyPopup------------
    [ContextMenu("Show Notify Popup")]
    public void ClickShowNotifyPopUp()
    {
        popUpNotify.ShowNotifyPopUp();
    } 
    [ContextMenu("Hide Notify Popup")]
    public void ClickHideNotifyPopUp()
    {
        popUpNotify.HideNotifyPopUp();
    }
    public void ChangeTextNotify(string text)
    {
        popUpNotify.SetTextNotify(text);
    }
    #endregion
    
        
    #region ------------RewardPopup------------
    [ContextMenu("Show Reward Popup")]
    public void ClickShowRewardPopUp()
    {
        popUpReward.ShowRewardPopUp();
    } 
    [ContextMenu("Hide Reward Popup")]
    public void ClickHideRewardPopUp()
    {
        popUpReward.HideRewardPopUp();
    }
    #endregion
    
    #region ------------DailyRWPopup------------
    [ContextMenu("Show DailyReward Popup")]
    public void ClickShowDailyRWPopUp()
    {
        popUpDailyRW.ShowDailyRewardPopUp();
    }    
    [ContextMenu("Hide DailyReward Popup")]
    public void ClickHideDailyRWPopUp()
    {
        popUpDailyRW.HideDailyRewardPopUp();
    }
    #endregion
    
    #region ------------SpinRWPopup------------
    [ContextMenu("Show SpinReward Popup")]
    public void ClickShowSpinRWPopUp()
    {
        popUpSpinRW.ShowSpinRewardPopUp();
    }    
    [ContextMenu("Hide SpinReward Popup")]
    public void ClickHideSpinRWPopUp()
    {
        popUpSpinRW.HideSpinRewardPopUp();;
    }
    #endregion
}
