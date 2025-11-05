using System;
using Audio;
using Data;
using Popup;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GiveUpPopup : PopUpBase
{
    [Header("GiveUp Popup")]
    [SerializeField] private Transform imgHeart;
    [SerializeField] private Button btnRetry;
    [SerializeField] private Button btnQuit;
    
    #region Overrides Func
    public override void ShowPopUp(float posY, float duration, UnityAction onComplete = null)
    {
        ShowCover(0.5f, () =>
        {
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
    
    public void OnClickContinnue()
    {
        var coin = DBController.Instance.COIN;
        if (coin >= GameConfig.COIN_REVIVE)
        {
            CoinController.Instance.SpendCoin(GameConfig.COIN_REVIVE);
            InGameData.GIVE_UP_COUNT++;
            HideGiveUpPopUp(() =>
            {
                InGameData.GAME_STATE = GameState.PlayingGame;
                EventDispatcher.Push(EventId.OnPlayerRevive);
            });   
        }
        else
        {
            PopupController.Instance.ChangeTextNotify("You don't have enough coins to revive");
            PopupController.Instance.ClickShowNotifyPopUp();
        }
    }
    public void OnClickQuit()
    {
        InGameData.GIVE_UP_COUNT = 0;
        InGameData.GAME_STATE = GameState.GameOver;
        HideGiveUpPopUp();
    }
    
    #region GiveUpPopup
    [ContextMenu("Show GiveUp Popup")]
    public void ShowGiveUpPopUp()
    {
        InitStateButton(true);
        InGameData.GAME_STATE = GameState.GiveUp;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        ShowPopUp(0f, 0.5f, () =>
        {
            DoShowGiveUpPopup();
        });
    }
    [ContextMenu("Hide GiveUp Popup")]
    public async UniTask HideGiveUpPopUp(UnityAction onCompleted = null)
    {
        InitStateButton(false);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupClose);
        var task1 = btnRetry.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var task2 = btnQuit.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.WhenAll(task1, task2);
        imgHeart.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            HidePopUp(-2000f, 0.5f, () =>
            {
                onCompleted?.Invoke();
                if (InGameData.GAME_STATE != GameState.PlayingGame)
                {
                    PopupController.Instance.ClickShowLosePopUp();   
                }
            }); 
        });
    }
    private async UniTask DoShowGiveUpPopup()
    {
        var task1 = btnRetry.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        var task2 = btnQuit.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        await UniTask.WhenAll(task1, task2);
        imgHeart.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
    }
    public void InitStateButton(bool state)
    {
        btnRetry.interactable = state;
        btnQuit.interactable = state;
    }
    #endregion
}