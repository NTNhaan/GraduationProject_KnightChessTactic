using System;
using Data;
using Popup;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
public class GiveUpPopup : PopUpBase
{
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
            PopupController.Instance.HideGiveUpPopUp(() =>
            {
                InGameData.GAME_STATE = GameState.PlayingGame;
                OnPlayerRevive(); 
            });   
        }
        else
        {
            PopupController.Instance.SetTextNotify("You don't have enough coins to revive");
            PopupController.Instance.ShowNotifyPopUp();
        }
    }

    public void OnClickQuit()
    {
        InGameData.GIVE_UP_COUNT = 0;
        InGameData.GAME_STATE = GameState.GameOver;
        PopupController.Instance.HideGiveUpPopUp();
    }

    public void OnPlayerRevive()
    {
        EventDispatcher.Push(EventId.OnPlayerRevive);
    }
}