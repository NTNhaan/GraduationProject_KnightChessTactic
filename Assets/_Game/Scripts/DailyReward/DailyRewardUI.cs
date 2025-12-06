// using System;
// using System.Collections;
// using System.Collections.Generic;
// using DG.Tweening;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.Serialization;
// using UnityEngine.UI;
//
// public class DailyRewardUI : PopupUIBase
// {
//     [SerializeField] private List<DailyUI> lstDailyUi;
//     [SerializeField] private GameObject gobjClaim;
//     [SerializeField] private GameObject gobjClaimX2;
//     [SerializeField] private GameObject gobjClose;
//     [SerializeField] private Text txtTimeRemain;
//     [SerializeField] private GameObject gobjTimeBanner;
//     [SerializeField] GameObject gobjNextReward;
//     [SerializeField] GameObject gobjDotRed;
//     [SerializeField] Animator animator; 
//     
//     [Header("SPRITE FIELD")]
//     [SerializeField] private Sprite sprCurrentDate;
//     [SerializeField] private Sprite sprPassDate;
//     [SerializeField] private Sprite sprCurrentDate7;
//     [SerializeField] private Sprite sprPassDate7;
//     
//     public void InitRewardUI()
//     {
//         for (int i = 0; i < lstDailyUi.Count; i++)
//         {
//             // lstDailyUi[i].txtDiamondValue.text = $"+{GameConfig.lstDiaReward[i]}";
//             var data = GameConfig.lstDailyReward[i];
//
//             if (data.diamond > 0 && data.boosterAmount == 0)
//             {
//                 lstDailyUi[i].txtDiamondValue.text = $"+{data.diamond}";
//             }
//             else if (data.diamond == 0 && data.boosterAmount > 0)
//             {
//                 lstDailyUi[i].txtDiamondValue.text = $"+{data.boosterAmount}";
//             }
//             else
//             {
//                 lstDailyUi[i].txtDiamondValue.text =
//                     $"+{data.diamond}";
//             }
//         }
//     }
//     public void ShowPassDate(int datePass)
//     {
//         for (int i = 0; i < datePass; i++)
//         {
//             // lstDailyUi[i].tfmDiamond.localScale = Vector3.zero;
//             lstDailyUi[i].tfmTick.localScale = Vector3.one;
//         }
//     }
//
//     public void ActiveCloseBtn(bool isActive)
//     {
//         gobjClose.SetActive(isActive);
//     }
//
//     public void ActiveClaimBtn(bool isActive)
//     {
//         gobjDotRed.SetActive(isActive);
//         gobjClaim.SetActive(isActive);
//         gobjTimeBanner.gameObject.SetActive(!isActive);
//         PlayAnimButtonDailyReward(isActive);
//     }
//
//     public void PlayAnimButtonDailyReward(bool isCanClaim)
//     {
//         animator.SetBool("CanClaim", isCanClaim);
//     }
//
//     public void DoAnimPassDate(int datePass)
//     {
//         Debug.Log($"CheckPassDate {datePass}");
//         lstDailyUi[datePass].tfmTick.DOScale(1, 0.3f);
//         if (datePass != 6)
//         {
//             lstDailyUi[datePass].imgBackground.sprite = sprPassDate;
//         }
//         else
//         {
//             lstDailyUi[datePass].imgBackground.sprite = sprPassDate7;
//         }
//     }
//
//     public void ActiveTickPassDate(int datePass)
//     {
//         lstDailyUi[datePass].tfmTick.localScale = Vector3.one;
//         if (datePass != 6)
//         {
//             lstDailyUi[datePass].imgBackground.sprite = sprPassDate;
//         }
//         else
//         {
//             lstDailyUi[datePass].imgBackground.sprite = sprPassDate7;
//         }
//     }
//
//     public void ActiveCurrentDate(int datePass, bool active)
//     {
//         if (datePass == 0)
//         {
//             ResetAllUI();
//             for (int i = 1; i < lstDailyUi.Count; i++)
//             {
//                 lstDailyUi[i].tfmTick.localScale = Vector3.zero;
//                 lstDailyUi[i].gobjAura.SetActive(false);
//                 lstDailyUi[i].gobjOutline.SetActive(false);
//             }
//         }
//         
//         lstDailyUi[datePass].gobjAura.SetActive(active);
//         lstDailyUi[datePass].gobjOutline.SetActive(active);
//         if (datePass != 6)
//         {
//             lstDailyUi[datePass].imgBackground.sprite = sprCurrentDate;
//         }
//         else
//         {
//             lstDailyUi[datePass].imgBackground.sprite = sprCurrentDate7;
//         }
//     }
//     public void ResetAllUI()
//     {
//         for (int i = 0; i < lstDailyUi.Count; i++)
//         {
//             lstDailyUi[i].tfmTick.localScale = Vector3.zero;
//             lstDailyUi[i].gobjAura.SetActive(false);
//             lstDailyUi[i].gobjOutline.SetActive(false);
//             if (i != 6)
//                 lstDailyUi[i].imgBackground.sprite = sprPassDate;
//             else
//                 lstDailyUi[i].imgBackground.sprite = sprPassDate7;
//         }
//     }
//
//     public void SetTimeRemain(TimeSpan ts)
//     {
//         var _tsDay = ts.Days * 24;
//         string timeStr = $"{(ts.Hours + _tsDay):00}:{ts.Minutes:00}:{ts.Seconds:00}";
//
//         EventDispatcher.Push(EventId.OnUpdateRemainTime, timeStr);
//     }
// }
//
// [Serializable]
// public class DailyUI
// {
//     public Image imgBackground;
//     public GameObject gobjAura;
//     public GameObject gobjOutline;
//     public Transform tfmDiamond;
//     public Transform tfmTick;
//     public Text txtDiamondValue;
//     
// }
// [Serializable]
// public class DailyRewardData
// {
//     public int diamond;       
//     public int boosterId;      
//     public int boosterAmount; 
// }