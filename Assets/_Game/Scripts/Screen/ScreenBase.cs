using System.Collections;
using Audio;
using Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using LineManager;

public class ScreenBase : MonoBehaviour
{
    [Header("=====Variables Override Scene Base=====")]
    [SerializeField] protected Animator[] anims;
    [SerializeField] protected GameObject[] gobjPanels;
    [SerializeField] protected ScreenGame screen;
    [SerializeField] private Transform coinBanner;
    [SerializeField] private Text numberCoin;
    [SerializeField] protected Animator animTransition;
    [SerializeField] private CanvasGroup canvasGroup;
    public GameObject[] GobjPanels { get => gobjPanels; }
    public ScreenGame Screen { get => screen; set => screen = value; }
    protected DBController _db;

    private void Start()
    {
        // canvasGroup.blocksRaycasts = false;
        _db = DBController.Instance;
    }

    public virtual void ShowScreen(UnityAction onComplete = null)
    {
        // AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        if (gobjPanels != null)
        {
            foreach (var panel in gobjPanels)
            {
                if (panel != null)
                {
                    panel.SetActive(true);
                }
            }
        }
        if (anims != null)
        {
            foreach (var animator in anims)
            {
                if (animator != null)
                {
                    animator.enabled = true;
                    animator.SetBool("isShow", true);
                }
            }
        }
        DOVirtual.DelayedCall(0.6f, () =>
        {
            LoadCoinUI();
            coinBanner.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            onComplete?.Invoke();
        });

    }
    public virtual void HideScreen(UnityAction onComplete = null)
    {
        // AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupClose);
        coinBanner.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        if (anims != null)
        {
            foreach (var animator in anims)
            {
                if (animator != null)
                    animator.SetBool("isShow", false);
            }
        }
        HideTransition();
        DOVirtual.DelayedCall(1f, () =>
        {
            if (gobjPanels != null)
            {
                foreach (var panel in gobjPanels)
                {
                    if (panel != null)
                        panel.SetActive(false);
                }
            }
            onComplete?.Invoke();
        });
    }
    public void HideTransition()
    {
        animTransition.gameObject.SetActive(true);
        if (animTransition != null)
        {
            canvasGroup.blocksRaycasts = false;
            animTransition.SetBool("isStransition", false);
        }
    }
   public void ShowTransition()
    {
        Debug.Log("Showtransition");
        if (animTransition != null)
        {
            canvasGroup.blocksRaycasts = true;
            animTransition.SetBool("isStransition", true);
            AnimatorHelper.Instance?.WaitForStateComplete(animTransition, "Show");
            animTransition.gameObject.SetActive(false);
        }
    }
   
    public virtual void LoadCoinUI()
    {
        // numberCoin.text = DBController.Instance.COIN.ToString("n0");
        Debug.Log("============== Load UI");
    }
}
public enum ScreenGame
{
    MainScreen = 0,
    GamePlayScreen = 1,
    ShopScreen = 2,
    LeaderBoardScreen = 3,
}
