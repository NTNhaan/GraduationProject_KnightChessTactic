using Audio;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using Popup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmPopup : PopUpBase
{
    [Header("Confirm Popup")]
    [SerializeField] private Button btnCloseConfirm;
    [SerializeField] private Button btnConfirmYes;
    [SerializeField] private Button btnConfirmNo;

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
    
    #region ConfirmPopup
    [ContextMenu("Show Confirm Popup")]
    public void ShowConfirmPopUp(UnityAction onYes = null, UnityAction onNo = null)
    {
        Debug.Log("ShowConfirmPopup");
        InitStateButton(true);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        InGameData.GAME_STATE = GameState.PauseGame;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        // EventManager.PasueGame();
        ShowPopUp(0f, 0.3f, async () =>
        {
            await btnConfirmYes.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
            await btnConfirmNo.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
            AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
            btnCloseConfirm.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            
            btnConfirmYes.onClick.RemoveAllListeners();
            btnConfirmYes.onClick.AddListener(async () =>
            {
                InitStateButton(false);
                
                await HideConfirmPopUp();
                onYes?.Invoke();
            });

        });
    }
    
    [ContextMenu("Hide Confirm Popup")]
    public async UniTask HideConfirmPopUp()
    {
        Debug.Log("HideConfirmPopup");
        InitStateButton(false);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        InGameData.GAME_STATE = GameState.PauseGame;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        // EventManager.PasueGame();
        DoHidePopupConfirm();
    }

    public async UniTask DoHidePopupConfirm()
    {
        var t1 = btnConfirmYes.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var t2 = btnConfirmNo.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        await UniTask.WhenAll(t1, t2);
        btnCloseConfirm.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        HidePopUp(-1800f, 0.3f);
    }
    public void InitStateButton(bool state)
    {
        btnCloseConfirm.interactable = state;
        btnConfirmYes.interactable = state;
        btnConfirmNo.interactable = state;
    }
    #endregion
}