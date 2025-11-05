using Audio;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
namespace Popup
{
    public class LeaderBoardPopup : PopUpBase
    {
        [Header("LeaderBoard Popup")]
        [SerializeField] private Button btnClose;
        
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
        
        #region LeaderBoardPopup
        [ContextMenu("Show LeaderBoard Popup")]
        public async UniTask ShowLeaderBoardPopUp()
        {
            AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
            InGameData.GAME_STATE = GameState.PauseGame;
            EventDispatcher.Push(EventId.OnGameStateChanged);
            EventManager.PasueGame();
            ShowPopUp(0f, 0.3f, async () =>
            {
                btnClose.interactable = true;
                await btnClose.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
                AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
            
                await UniTask.Delay(400);
                await LeaderBoardController.Instance.ScrollToUser();
                await UniTask.Delay(600);
                await LeaderBoardController.Instance.TryAnimateUserClimb();
            });
        }
        [ContextMenu("Hide LeaderBoard Popup")]
        public async UniTask HideLeaderBoardPopUp()
        {
            AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupClose);
            btnClose.interactable = false;
            await btnClose.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        
            HidePopUp(-1800f, 0.3f, ()=>
            {
                InGameData.GAME_STATE = GameState.PlayingGame;
                EventDispatcher.Push(EventId.OnGameStateChanged);
                EventManager.ResumeGame();  
            });
        }
        #endregion
    }
}