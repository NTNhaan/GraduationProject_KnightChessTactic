using Audio;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using Popup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NotifyPopup : PopUpBase
{
    [Header("Notify Popup")]
    [SerializeField] private Button btnCloseNotify;
    [SerializeField] private Text textContent;
    
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
    
   #region NotifyPopup
    [ContextMenu("Show Notify Popup")]
    public void ShowNotifyPopUp()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        InGameData.GAME_STATE = GameState.PauseGame;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        EventManager.PasueGame();
        ShowPopUp(0f, 0.3f, async () =>
        {
            // if (ScreenController.Instance.CurScreen == ScreenGame.ShopScreen)
            // {
            //     SetTextNotify("You don't have enough coins to buy this item");
            // }
            // else if (ScreenController.Instance.CurScreen == ScreenGame.GamePlayScreen)
            // {
            //     SetTextNotify("You don't have enough coins to revive");
            // }

            await textContent.DOFade(1f, 0.5f).OnComplete(() =>
            {
                btnCloseNotify.interactable = true;
                btnCloseNotify.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            }).ToUniTask();
        });
    }
    
    [ContextMenu("Hide Notify Popup")]
    public void HideNotifyPopUp()
    {
        btnCloseNotify.interactable = false;
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        DoHidePopupNotify();
    }

    public void SetTextNotify(string text)
    {
        Debug.Log($"CheckTextChange: {text}");
        textContent.text = text;
    }
    public async UniTask DoHidePopupNotify()
    {
        await btnCloseNotify.transform.DOScale(0f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        textContent.DOFade(0f, 0.3f).OnComplete(() =>
        {
            HidePopUp(-1800f, 0.3f);
            if (!DBController.Instance.TUTORIAL_COMPLETED)
            {
                InGameData.GAME_STATE = GameState.PauseGame;
                EventDispatcher.Push(EventId.OnGameStateChanged);
                EventManager.PasueGame();   
            }
            else
            {
                InGameData.GAME_STATE = GameState.PlayingGame;
                EventDispatcher.Push(EventId.OnGameStateChanged);
                EventManager.ResumeGame();     
            }
        });
    }
    #endregion
}