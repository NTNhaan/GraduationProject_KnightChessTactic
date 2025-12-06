using System;
using System.Collections.Generic;
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
    
    [SerializeField] private List<DailyUI> lstDailyUi;
    [SerializeField] private GameObject gobjClaim;
    // [SerializeField] private GameObject gobjClaimX2;
    [SerializeField] private GameObject gobjClose;
    [SerializeField] private Text txtTimeRemain;
    [SerializeField] private GameObject gobjTimeBanner;
    // [SerializeField] GameObject gobjNextReward;
    [SerializeField] GameObject gobjDotRed;
    [SerializeField] Animator animator; 
    
    [Header("SPRITE FIELD")]
    [SerializeField] private Sprite sprCurrentDate;
    [SerializeField] private Sprite sprPassDate;
    [SerializeField] private Sprite sprCurrentDate7;
    [SerializeField] private Sprite sprPassDate7;
    
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
        UITopController.Instance.HideTab();
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
            UITopController.Instance.ShowTab();
        });
    }
    #endregion

    #region CoreFunction
    public void InitRewardUI()
    {
        for (int i = 0; i < lstDailyUi.Count; i++)
        {
            // lstDailyUi[i].txtDiamondValue.text = $"+{GameConfig.lstDiaReward[i]}";
            var data = GameConfig.lstDailyReward[i];

            if (data.coin > 0 && data.boosterAmount == 0)
            {
                lstDailyUi[i].txtDiamondValue.text = $"+{data.coin}";
            }
            else if (data.coin == 0 && data.boosterAmount > 0)
            {
                lstDailyUi[i].txtDiamondValue.text = $"+{data.boosterAmount}";
            }
            else
            {
                lstDailyUi[i].txtDiamondValue.text =
                    $"+{data.coin}";
            }
        }
    }
    public void ShowPassDate(int datePass)
    {
        for (int i = 0; i < datePass; i++)
        {
            // lstDailyUi[i].tfmDiamond.localScale = Vector3.zero;
            lstDailyUi[i].tfmTick.localScale = Vector3.one;
        }
    }

    public void ActiveCloseBtn(bool isActive)
    {
        gobjClose.SetActive(isActive);
    }

    public void ActiveClaimBtn(bool isActive)
    {
        gobjDotRed.SetActive(isActive);
        gobjClaim.SetActive(isActive);
        gobjTimeBanner.gameObject.SetActive(!isActive);
        PlayAnimButtonDailyReward(isActive);
    }

    public void PlayAnimButtonDailyReward(bool isCanClaim)
    {
        animator.SetBool("CanClaim", isCanClaim);
    }

    public void DoAnimPassDate(int datePass)
    {
        Debug.Log($"CheckPassDate {datePass}");
        lstDailyUi[datePass].tfmTick.DOScale(1, 0.3f);
        if (datePass != 6)
        {
            lstDailyUi[datePass].imgBackground.sprite = sprPassDate;
        }
        else
        {
            lstDailyUi[datePass].imgBackground.sprite = sprPassDate7;
        }
    }

    public void ActiveTickPassDate(int datePass)
    {
        lstDailyUi[datePass].tfmTick.localScale = Vector3.one;
        if (datePass != 6)
        {
            lstDailyUi[datePass].imgBackground.sprite = sprPassDate;
        }
        else
        {
            lstDailyUi[datePass].imgBackground.sprite = sprPassDate7;
        }
    }

    public void ActiveCurrentDate(int datePass, bool active)
    {
        if (datePass == 0)
        {
            ResetAllUI();
            for (int i = 1; i < lstDailyUi.Count; i++)
            {
                lstDailyUi[i].tfmTick.localScale = Vector3.zero;
                lstDailyUi[i].gobjAura.SetActive(false);
                lstDailyUi[i].gobjOutline.SetActive(false);
            }
        }
        
        lstDailyUi[datePass].gobjAura.SetActive(active);
        lstDailyUi[datePass].gobjOutline.SetActive(active);
        if (datePass != 6)
        {
            lstDailyUi[datePass].imgBackground.sprite = sprCurrentDate;
        }
        else
        {
            lstDailyUi[datePass].imgBackground.sprite = sprCurrentDate7;
        }
    }
    public void ResetAllUI()
    {
        for (int i = 0; i < lstDailyUi.Count; i++)
        {
            lstDailyUi[i].tfmTick.localScale = Vector3.zero;
            lstDailyUi[i].gobjAura.SetActive(false);
            lstDailyUi[i].gobjOutline.SetActive(false);
            if (i != 6)
                lstDailyUi[i].imgBackground.sprite = sprPassDate;
            else
                lstDailyUi[i].imgBackground.sprite = sprPassDate7;
        }
    }

    public void SetTimeRemain(TimeSpan ts)
    {
        var _tsDay = ts.Days * 24;
        string timeStr = $"{(ts.Hours + _tsDay):00}:{ts.Minutes:00}:{ts.Seconds:00}";

        EventDispatcher.Push(EventId.OnUpdateRemainTime, timeStr);
    }
    #endregion
}

[Serializable]
public class DailyUI
{
    public Image imgBackground;
    public GameObject gobjAura;
    public GameObject gobjOutline;
    public Transform tfmDiamond;
    public Transform tfmTick;
    public Text txtDiamondValue;
    
}
[Serializable]
public class DailyRewardData
{
    public int coin;       
    public int boosterId;      
    public int boosterAmount; 
}