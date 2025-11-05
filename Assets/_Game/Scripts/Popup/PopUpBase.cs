using Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
namespace Popup
{
    public class PopUpBase : MonoBehaviour
    {
        [Header("=====Variables Override PopUp Base=====")]
        [SerializeField] protected Transform tfmPopup;
        [SerializeField] protected Image imgCover;
        [SerializeField] private Transform coinBanner;
        [SerializeField] private Text numberCoin;
        
        public Transform TfmPopup { get => tfmPopup; }

        public virtual void ShowPopUp(float posY = 0, float duration = 0, UnityAction onComplete = null)
        {
            TfmPopup.gameObject.SetActive(true);
            TfmPopup.DOLocalMoveY(posY, duration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                LoadCoinUI();
                coinBanner.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
                onComplete?.Invoke();
            });
        }
        public virtual void HidePopUp(float posY, float duration, UnityAction onComplete = null)
        {
            coinBanner.DOScale(0f, 0.3f).SetEase(Ease.InBack);
            TfmPopup.DOLocalMoveY(posY, duration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    TfmPopup.gameObject.SetActive(false);
                    onComplete?.Invoke();
                });
        }
        public virtual void ShowCover(float duration=0.5f, UnityAction onComplete = null)
        {
            imgCover.gameObject.SetActive(true);
            imgCover.DOFade(0.9921569f, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                onComplete?.Invoke();
            });

        }
        public virtual void HideCover(UnityAction onComplete = null)
        {
            imgCover.DOFade(0f, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                imgCover.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }
        public virtual void LoadCoinUI()
        {
            numberCoin.text = DBController.Instance.COIN.ToString("n0");
            Debug.Log("============== Load UI");
        }
    }
}