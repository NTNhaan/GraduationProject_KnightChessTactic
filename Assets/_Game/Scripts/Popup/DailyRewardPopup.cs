using Popup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Audio;
using Data;
using UnityEngine.Serialization;

public class DailyRewardPopup : PopUpBase
{
    [SerializeField] private Transform Day1;
    [SerializeField] private Transform Day2;
    [SerializeField] private Transform Day3;
    [SerializeField] private Transform Day4;
    [SerializeField] private Transform Day5;
    [SerializeField] private Transform Day6;
    [SerializeField] private Transform Day7;
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
    
     #region RewardPopup
    [ContextMenu("Show Daily Reward Popup")]
    public async UniTask ShowDailyRewardPopUp()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        // InGameData.GAME_STATE = GameState.PauseGame;
        // EventDispatcher.Push(EventId.OnGameStateChanged);
        // EventManager.PasueGame();
        TabController.Instance.HideBottomTab();
        ShowPopUp(0f, 0.3f, async () =>
        {
            var t1 = Day1.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
            var t2 = Day2.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
            var t3 = Day3.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
            await UniTask.WhenAll(t1, t2, t3);
            var t4 = Day4.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
            var t5 = Day5.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
            var t6 = Day6.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
            await UniTask.WhenAll(t4, t5, t6);

            await Day7.DOScale(1f, 0.1f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
            await btnClose.DOScale(1f, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        });
    }
    
    [ContextMenu("Hide Reward Popup")]
    public void HideDailyRewardPopUp()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        // InGameData.GAME_STATE = GameState.PauseGame;
        // EventDispatcher.Push(EventId.OnGameStateChanged);
        // EventManager.PasueGame();
        DoHidePopupDailyRW();
    }
    
    public async UniTask DoHidePopupDailyRW()
    {
        btnClose.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        Day1.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        Day2.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        Day3.DOScale(0f, 0.3f).SetEase(Ease.InBack);
       
        Day4.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        Day5.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        Day6.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        Day7.DOScale(0f, 0.3f).SetEase(Ease.InBack);

        HidePopUp(2000, 0.3f, () =>
        {
            TabController.Instance.ShowBottomTab();
        });
    }
    #endregion
}