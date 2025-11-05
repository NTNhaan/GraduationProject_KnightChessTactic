using Popup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Audio;
using Data;
using UnityEngine.Serialization;

public class RewardPopup : PopUpBase
{
    [SerializeField] private Text coinField;
    [SerializeField] private Transform startCoinPos;
    [SerializeField] private Transform endCoinPos;
    [SerializeField] private Transform parentTransform;
    [FormerlySerializedAs("rectReward")]
    [Header("Reward Popup")]
    [SerializeField] private RectTransform rectRewardBanner;
    [SerializeField] private Animator giftAnim;
    [SerializeField] private Image shineRotate;
    [SerializeField] private ParticleSystem confettiParticle;
    [SerializeField] private ParticleSystem shineParticle;
    [SerializeField] private Transform coinReward;
    
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
    [ContextMenu("Show Reward Popup")]
    public async UniTask ShowRewardPopUp()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        InGameData.GAME_STATE = GameState.PauseGame;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        EventManager.PasueGame();
        ShowPopUp(0f, 0.3f, async () =>
        {
            AudioController.Instance.PlayEffect(Sound.Name.Sound_Celebrate);
            await rectRewardBanner.DOScale(1f, 0.5f).SetEase(Ease.OutBack).ToUniTask();
            await rectRewardBanner.DOAnchorPosY(500, 0.5f).SetEase(Ease.OutBack).ToUniTask();
            
            giftAnim.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                AudioController.Instance.PlayEffect(Sound.Name.Sound_Gift);
                giftAnim.SetBool("isOpen", true); 
            });
            
            await UniTask.Delay(1000);
            shineParticle.Play();
            shineParticle.gameObject.SetActive(true);
            
            await AnimatorHelper.Instance.WaitForStateComplete(giftAnim, "Anim_Gift");
            giftAnim.SetBool("isOpen", false);
            
            confettiParticle.Play();
            AudioController.Instance.PlayEffect(Sound.Name.Sound_Confetti);
            confettiParticle.gameObject.SetActive(true);
            
            await AnimatorHelper.Instance.WaitForComplePartical(shineParticle, () =>
            {
                shineRotate.DOFade(1f, 0.5f);
                coinReward.DOScale(1f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    Vector3 firstPosition = startCoinPos.transform.position;
                    AnimatorHelper.Instance.ObjectFly(firstPosition, 10, endCoinPos, parentTransform, () =>
                    {
                        CoinController.Instance.AddCoin(GameConfig.COIN_PLUS, coinField, ()  =>
                        {
                            UniTask.Delay(2000);
                            HideRewardPopUp(); 
                        });
                    });
                });
            });
        });
    }
    
    [ContextMenu("Hide Reward Popup")]
    public void HideRewardPopUp()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        InGameData.GAME_STATE = GameState.PauseGame;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        EventManager.PasueGame();
        DoHidePopupReward();
    }
    
    public async UniTask DoHidePopupReward()
    {
        var t1 = shineRotate.DOFade(0f, 0.5f).SetEase(Ease.OutQuad).ToUniTask();
        var t2 = coinReward.DOScale(0f, 0.5f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.WhenAll(t1, t2);

        confettiParticle.gameObject.SetActive(false);
        shineParticle.gameObject.SetActive(false);
        
        var t3 = giftAnim.transform.DOScale(0f, 0.5f).SetEase(Ease.InBack).ToUniTask();
        // var t4 = coinBanner.DOScale(0f, 0.5f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.WhenAll(t3);

        await rectRewardBanner.DOAnchorPosY(-20, 0.5f).SetEase(Ease.OutBack).ToUniTask();
        await rectRewardBanner.DOScale(0f, 0.5f).SetEase(Ease.OutBack).ToUniTask();
        
        giftAnim.SetBool("isClose", true);
        HidePopUp(-1800f, 0.3f, () =>
        {
            EventDispatcher.Push(EventId.OnHidePopupReward);
            // btnRetry.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            // btnHome.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        });
    }
    #endregion
}