using Popup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Audio;
using Data;
using UnityEngine.Serialization;

public class SpinRewardPopup : PopUpBase
{

    [SerializeField] private Transform btnClose;
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
    
     #region SpinRewardPopup
    [ContextMenu("Show SpinReward Popup")]
    public async UniTask ShowSpinRewardPopUp()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        // InGameData.GAME_STATE = GameState.PauseGame;
        // EventDispatcher.Push(EventId.OnGameStateChanged);
        // EventManager.PasueGame();
        TabController.Instance.HideBottomTab();
        ShowPopUp(0f, 0.3f, () =>
        {
            btnClose.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        });
    }
    
    [ContextMenu("Hide SpinReward Popup")]
    public void HideSpinRewardPopUp()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        // InGameData.GAME_STATE = GameState.PauseGame;
        // EventDispatcher.Push(EventId.OnGameStateChanged);
        // EventManager.PasueGame();
        HidePopUp(2000f, 0.3f);
        btnClose.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        TabController.Instance.ShowBottomTab();
    }
    
    public async UniTask DoHidePopupDailyRW()
    {
        // btnClose.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        //
        // Day1.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        // Day2.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        // Day3.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        //
        // Day4.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        // Day5.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        // Day6.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        // Day7.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        //
        // HidePopUp(2000f, 0.3f, () =>
        // {
        //     TabController.Instance.ShowBottomTab();
        // });
    }
    #endregion
}