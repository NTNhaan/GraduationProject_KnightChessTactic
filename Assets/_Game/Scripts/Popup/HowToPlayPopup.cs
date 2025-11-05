using Audio;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using Popup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HowToPlayPopup : PopUpBase
{
    [Header("HowToPlay Popup")]
    [SerializeField] private Text textGuide;
    [SerializeField] private Transform Point1;
    [SerializeField] private Transform Point2;
    [SerializeField] private Transform Point3;
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
    
    #region HowToPlayPopup
        [ContextMenu("Show HowToPlay Popup")]
        public void ShowHowToPlayPopUp()
        {
            AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
            InGameData.GAME_STATE = GameState.PauseGame;
            EventDispatcher.Push(EventId.OnGameStateChanged);
            if(!DBController.Instance.TUTORIAL_COMPLETED) 
                EventDispatcher.Push(EventId.OnTutorialShow);
            EventManager.PasueGame();
            ShowPopUp(0f, 0.5f, async () =>
            {
                AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
                await Point1.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
                AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
                await Point2.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
                AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
                await Point3.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
                
                textGuide.DOFade(1f, 0.5f);
                btnClose.interactable = true;
                btnClose.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            });
        }
        [ContextMenu("Hide HowToPlay Popup")]
        public void HideHowToPlayPopUp()
        {
            AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupClose);
            DoHideHowToPlayPopup(() =>
            {
                HidePopUp(-1800f, 0.5f, ()=>
                {
                    if (!DBController.Instance.TUTORIAL_COMPLETED)
                    {
                        EventDispatcher.Push(EventId.OnTutorialHide);
                    }
                    else
                    {
                        InGameData.GAME_STATE = GameState.PlayingGame;
                        EventDispatcher.Push(EventId.OnGameStateChanged);
                        EventManager.ResumeGame();    
                    }
                }); 
            });
        }
    
        private async UniTask DoHideHowToPlayPopup(UnityAction onComplete = null)
        {
            btnClose.interactable = false;
            var t1 = Point1.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
            var t2 = Point2.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
            var t3 = Point3.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
            await UniTask.WhenAll(t1, t2, t3);
            textGuide.DOFade(0f, 0.5f);
            btnClose.transform.DOScale(0f, 0.3f).SetEase(Ease.OutBack);
            onComplete?.Invoke();
        }
        #endregion
    
}