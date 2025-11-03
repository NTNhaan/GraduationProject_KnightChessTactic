using System;
using System.Threading.Tasks;
using Audio;
using UnityEngine;
using Data;
using DG.Tweening;
using Popup;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class PopupController : Singleton<PopupController>
{
    [Header("Coin UI")] 
    [SerializeField] private Transform coinBanner;
    [SerializeField] private Text numberCoin;
    
    [SerializeField] private Transform shopBtn;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Transform firstTransform;
    
    [Header("Lose Popup")]
    [SerializeField] private LosePopup popUpGameOver;
    [SerializeField] private RectTransform character;
    [SerializeField] private Image shine;
    [SerializeField] private Transform btnRetry;
    [SerializeField] private Transform btnHome;
    
    [Header("GiveUp Popup")]
    [SerializeField] private GiveUpPopup popUpGiveUp;
    [SerializeField] private Transform imgHeart;
    [SerializeField] private Transform btnContinue;
    [SerializeField] private Transform btnQuit;
    
    [Header("Pause Popup")]
    [SerializeField] private PausePopup popUpPauseGame;
    [SerializeField] private Transform btnRestart;
    [SerializeField] private Transform btnLoadHome;
    [SerializeField] private Transform imgSound;
    [SerializeField] private Transform imgMusic;
    [SerializeField] private Transform imgVibrate;
    [SerializeField] private Transform imgQuit;
    
    [Header("LeaderBoard Popup")]
    [SerializeField] private LeaderBoardPopup popUpLeaderBoard;
    // [SerializeField] private Transform btnBack;
    [SerializeField] private Transform btnClose;
    
    [Header("HowToPlay Popup")]
    [SerializeField] private HowToPlayPopup popUpHowToPlay;
    [SerializeField] private Text textGuide;
    [SerializeField] private Transform Point1;
    [SerializeField] private Transform Point2;
    [SerializeField] private Transform Point3;
    [SerializeField] private Transform btnOut;
    
    [Header("Confirm Popup")]
    [SerializeField] private ConfirmPopup popUpConfirm;
    [SerializeField] private Transform btnCloseConfirm;
    [SerializeField] private Button btnConfirmYes;
    [SerializeField] private Transform btnConfirmNo;
    
    [Header("Notify Popup")]
    [SerializeField] private NotifyPopup  popUpNotify;
    [SerializeField] private Transform btnCloseNotify;
    [SerializeField] private Text textContent;
    
    [Header("Reward Popup")]
    [SerializeField] private RewardPopup popUpReward;
    [SerializeField] private RectTransform rectReward;
    [SerializeField] private Animator giftAnim;
    [SerializeField] private Image shineRotate;
    [SerializeField] private ParticleSystem confettiParticle;
    [SerializeField] private ParticleSystem shineParticle;
    [SerializeField] private Transform coinReward;
    public PausePopup pausePopup => popUpPauseGame;

    private bool isFirstClick = true;

    public void OnEnable()
    {
        EventDispatcher.Register(EventId.OnCoinChanged, UpdateCoinUI);
    }
    public void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.OnCoinChanged, UpdateCoinUI);
    }
    private void Start()
    {
        DOTween.Init();
    }

    private void UpdateCoinUI(object data = null)
    {
        numberCoin.text = DBController.Instance.COIN.ToString();
    }
    
    #region LosePopup
    [ContextMenu("Show GameOver Popup")]
    public void ShowGameOverPopUp()
    {
        InGameData.GAME_STATE = GameState.GameOver;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        popUpGameOver.ShowPopUp(0f, 0.5f, () =>
        {
            DoShowLosePopup();
        });
    }
    [ContextMenu("Hide GameOver Popup")]
    public async UniTask HideGameOverPopUp(UnityAction onComplete = null)
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupClose);
        var task = btnRetry.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var task1 = btnHome.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.WhenAll(task, task1);
        shine.DOFade(0f, 0.5f);
        coinBanner.DOScale(0f, 0.3f).SetEase(Ease.OutBack);
        character.DOAnchorPosY(0f, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            popUpGameOver.HidePopUp(-2000f, 0.5f); 
            onComplete?.Invoke();
        });
    }

    public async UniTask DoShowLosePopup()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Progress);
        character.DOAnchorPosY(600, .5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            AudioController.Instance.PlayEffect(Sound.Name.Sound_Reward);
            shine.DOFade(0.3f, .5f);
            UpdateCoinUI();
            coinBanner.DOScale(1f, .3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                var time = TimeManager.Instance.ElapsedTime;
                if (DBController.Instance.TUTORIAL_COMPLETED && time >= 30f)
                {
                    ShowRewardPopUp();
                }
                else
                {
                    btnRetry.DOScale(1f, .3f).SetEase(Ease.OutBack);
                    btnHome.DOScale(1f, .3f).SetEase(Ease.OutBack);
                }
            });
        });
    }
    #endregion
    
    #region GiveUpPopup
    [ContextMenu("Show GiveUp Popup")]
    public void ShowGiveUpPopUp()
    {
        InGameData.GAME_STATE = GameState.GiveUp;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        popUpGiveUp.ShowPopUp(0f, 0.5f, () =>
        {
            DoShowGiveUpPopup();
        });
    }
    [ContextMenu("Hide GiveUp Popup")]
    public async UniTask HideGiveUpPopUp(UnityAction onCompleted = null)
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupClose);
        var task1 = btnContinue.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var task2 = btnQuit.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.WhenAll(task1, task2);
        coinBanner.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        imgHeart.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            popUpGiveUp.HidePopUp(-2000f, 0.5f, () =>
            {
                onCompleted?.Invoke();
                if (InGameData.GAME_STATE != GameState.PlayingGame)
                {
                    ShowGameOverPopUp();   
                }
            }); 
        });
    }

    private async UniTask DoShowGiveUpPopup()
    {
        var task1 = btnContinue.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        var task2 = btnQuit.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        await UniTask.WhenAll(task1, task2);
        imgHeart.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        UpdateCoinUI();
        coinBanner.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }
    #endregion
    
    #region PausePopup
    [ContextMenu("Show Pause Popup")]
    public void ShowPausePopUp()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        InGameData.PRE_STATE = InGameData.GAME_STATE;
        InGameData.GAME_STATE = GameState.PauseGame;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        EventManager.PasueGame();
        popUpPauseGame.ShowPopUp(0f, 0.5f, () =>
        {
            DoShowPausePopup();
        });
    }

    [ContextMenu("Hide Pause Popup")]
    public void OnClickHidePausePopup()
    {
        HidePausePopUp();
    }
    public async UniTask HidePausePopUp(UnityAction onCompleted = null)
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupClose);
        var t1 = imgQuit.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var t2 = btnRestart.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var t3 = btnLoadHome.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.WhenAll(t1, t2, t3);
        var t4 = imgSound.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var t5 = imgMusic.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var t6 = imgVibrate.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.WhenAll(t4, t5, t6);
        
        popUpPauseGame.HidePopUp(-1800f, 0.5f, ()=>
        {
            InGameData.GAME_STATE = GameState.PlayingGame;
            EventDispatcher.Push(EventId.OnGameStateChanged);
            EventManager.ResumeGame();  
            
            onCompleted?.Invoke();
        });
    }

    public async UniTask DoShowPausePopup()
    {
        var task1 = btnRestart.DOScale(1f, 0.2f).SetEase(Ease.OutBack).ToUniTask();
        var task2 = btnLoadHome.DOScale(1f, 0.2f).SetEase(Ease.OutBack).ToUniTask();
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        await UniTask.WhenAll(task1, task2);
        
        imgSound.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        imgMusic.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        imgVibrate.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        imgQuit.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
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
        popUpLeaderBoard.ShowPopUp(0f, 0.3f, async () =>
        {
            // var t1 = btnBack.DOScale(1f, .3f).SetEase(Ease.OutBack).ToUniTask();
            await btnClose.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
            AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
            // await UniTask.WhenAll(t1, t2);
            
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
        // var t1 = btnBack.DOScale(0f, .3f).SetEase(Ease.InBack).ToUniTask();
        await btnClose.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        // await UniTask.WhenAll(t1, t2);
        
        popUpLeaderBoard.HidePopUp(-1800f, 0.3f, ()=>
        {
            InGameData.GAME_STATE = GameState.PlayingGame;
            EventDispatcher.Push(EventId.OnGameStateChanged);
            EventManager.ResumeGame();  
        });
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
        popUpHowToPlay.ShowPopUp(0f, 0.5f, async () =>
        {
            AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
            await Point1.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
            AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
            await Point2.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
            AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
            await Point3.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
            
            textGuide.DOFade(1f, 0.5f);
            btnOut.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        });
    }
    [ContextMenu("Hide HowToPlay Popup")]
    public void HideHowToPlayPopUp()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupClose);
        DoHideHowToPlayPopup(() =>
        {
            popUpHowToPlay.HidePopUp(-1800f, 0.5f, ()=>
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
        var t1 = Point1.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var t2 = Point2.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        var t3 = Point3.DOScale(0f, 0.3f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.WhenAll(t1, t2, t3);
        textGuide.DOFade(0f, 0.5f);
        btnOut.DOScale(0f, 0.3f).SetEase(Ease.OutBack);
        onComplete?.Invoke();
    }
    #endregion
    
    #region ConfirmPopup
    [ContextMenu("Show Confirm Popup")]
    public void ShowConfirmPopUp(UnityAction onYes = null, UnityAction onNo = null)
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        InGameData.GAME_STATE = GameState.PauseGame;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        EventManager.PasueGame();
        popUpConfirm.ShowPopUp(0f, 0.3f, async () =>
        {
            await btnConfirmYes.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
            await btnConfirmNo.DOScale(1f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
            AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
            btnCloseConfirm.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            
            btnConfirmYes.onClick.RemoveAllListeners();
            btnConfirmYes.onClick.AddListener(() =>
            {
                HideConfirmPopUp();
                onYes?.Invoke();
            });

        });
    }
    
    [ContextMenu("Hide Confirm Popup")]
    public void HideConfirmPopUp()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        InGameData.GAME_STATE = GameState.PauseGame;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        EventManager.PasueGame();
        DoHidePopupConfirm();
    }

    public async UniTask DoHidePopupConfirm()
    {
        var t1 = btnConfirmYes.transform.DOScale(0f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        var t2 = btnConfirmNo.DOScale(0f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Icon_Appear);
        await UniTask.WhenAll(t1, t2);
        btnCloseConfirm.DOScale(0f, 0.3f).SetEase(Ease.OutBack);
        popUpConfirm.HidePopUp(-1800f, 0.3f);
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
        popUpNotify.ShowPopUp(0f, 0.3f, async () =>
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
                btnCloseNotify.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            }).ToUniTask();
        });
    }
    
    [ContextMenu("Hide Notify Popup")]
    public void HideNotifyPopUp()
    {
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
        await btnCloseNotify.DOScale(0f, 0.3f).SetEase(Ease.OutBack).ToUniTask();
        textContent.DOFade(0f, 0.3f).OnComplete(() =>
        {
            popUpNotify.HidePopUp(-1800f, 0.3f);
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
    
        
    #region RewardPopup
    [ContextMenu("Show Reward Popup")]
    public async UniTask ShowRewardPopUp()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        InGameData.GAME_STATE = GameState.PauseGame;
        EventDispatcher.Push(EventId.OnGameStateChanged);
        EventManager.PasueGame();
        popUpReward.ShowPopUp(0f, 0.3f, async () =>
        {
            AudioController.Instance.PlayEffect(Sound.Name.Sound_Celebrate);
            await rectReward.DOScale(1f, 0.5f).SetEase(Ease.OutBack).ToUniTask();
            await rectReward.DOAnchorPosY(500, 0.5f).SetEase(Ease.OutBack).ToUniTask();
            
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
                    Vector3 firstPosition = firstTransform.transform.position;
                    AnimatorHelper.Instance.ObjectFly(firstPosition, 10, shopBtn, parentTransform, () =>
                    {
                        CoinController.Instance.AddCoin(GameConfig.COIN_PLUS, numberCoin, ()  =>
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
        var t4 = coinBanner.DOScale(0f, 0.5f).SetEase(Ease.InBack).ToUniTask();
        await UniTask.WhenAll(t3, t4);

        await rectReward.DOAnchorPosY(-20, 0.5f).SetEase(Ease.OutBack).ToUniTask();
        await rectReward.DOScale(0f, 0.5f).SetEase(Ease.OutBack).ToUniTask();
        
        giftAnim.SetBool("isClose", true);
        popUpReward.HidePopUp(-1800f, 0.3f, () =>
        {
            btnRetry.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            btnHome.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        });
    }
    #endregion
}
