using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

public class DailyRewardController : MonoBehaviour
{
    [SerializeField] private bool _testCase;
    [SerializeField] private DailyRewardPopup _dailyRewardUI;
    private DBController _db;
    private DailyReward _daily;

    // private int _diaReward;
    private TimeSpan _timeRemain;
    [SerializeField] private bool _isCountDown = true;
    private static bool _isShowedAuto;
    
    private void Start()
    {
        _db = DBController.Instance;
        _daily = _db.DAILY_REWARD;

        if (_daily.dateTimeLastTimeClaimRewardTick == 0)
        {
            Debug.Log($"old user");
            int hourNow = DateTime.Now.Hour;
            DateTime _dtEndOfDay = (hourNow < 24 && hourNow >= 22)
                ? DateTime.Now.Date.AddDays(1).AddHours(-2)
                : DateTime.Now.Date.AddHours(-2);
            _daily.dateTimeLastTimeClaimRewardTick = _dtEndOfDay.Ticks;
            _db.DAILY_REWARD = _daily;
        }

        CheckPassDay();

        if (_testCase)
        {
            _timeRemain = TimeSpan.FromSeconds(10);
        }
    }

    private void Update()
    {
        if (_isCountDown)
        {
            var _totalSeconds = _timeRemain.TotalSeconds;

            _totalSeconds -= Time.deltaTime;
            _timeRemain = TimeSpan.FromSeconds(_totalSeconds);
            _dailyRewardUI.SetTimeRemain(_timeRemain);
            if (_timeRemain <= TimeSpan.Zero)
            {
                _dailyRewardUI.SetTimeRemain(_timeRemain = TimeSpan.Zero);
                _dailyRewardUI.ActiveClaimBtn(true);
                _isCountDown = false;
                
                int pass = _db.DAILY_REWARD.datePass;
                _dailyRewardUI.ActiveCurrentDate(_db.DAILY_REWARD.datePass, true);
            }
        }
    }

    void InitFirstUI()
    {
        for (int i = 0; i < _db.DAILY_REWARD.datePass; i++)
        {
            _dailyRewardUI.ActiveTickPassDate(i);
        }
    }

    private void CheckPassDay()
    {
        _dailyRewardUI.InitRewardUI();
        _dailyRewardUI.ActiveClaimBtn(false);

        DateTime nextClaimTime = new DateTime(_daily.dateTimeLastTimeClaimRewardTick);
        DateTime now = DateTime.Now;

        bool canClaim = now >= nextClaimTime;

        if (canClaim)
        {
            _isCountDown = false;
            _dailyRewardUI.SetTimeRemain(TimeSpan.Zero);

            if (_daily.datePass == 0)
            {
                _dailyRewardUI.ResetAllUI();
                _dailyRewardUI.ActiveCurrentDate(0, true);
            }
            else
            {
                _dailyRewardUI.ShowPassDate(_daily.datePass);
                _dailyRewardUI.ActiveCurrentDate(_daily.datePass, true);
            }

            _dailyRewardUI.ActiveClaimBtn(true);
        }
        else
        {
            _dailyRewardUI.ShowPassDate(_daily.datePass);
            SetTimeRemain();  
        }
    }

    bool CanClaim()
    {
        DateTime _now = DateTime.Now;
        return _now >= ConvertTickToDateTime(_daily.dateTimeLastTimeClaimRewardTick);
    }

    private void SaveRewardData()
    {
        _dailyRewardUI.ActiveCurrentDate(_daily.datePass, false);
        int rewardIndex = _daily.datePass;
        _daily.datePass++;
        if (_daily.datePass > 6)
        {
            _daily.datePass = 0; 
        }
        var reward = GameConfig.lstDailyReward[rewardIndex];
        ApplyReward(reward);
        DateTime nextReset = GetNextResetTime();
        _daily.dateTimeLastTimeClaimRewardTick = nextReset.Ticks;

        _db.DAILY_REWARD = _daily;

        SetTimeRemain();
    }

    private void ClaimAndAnim()
    {
        var _oldDate = _db.DAILY_REWARD.datePass;
        SaveRewardData();
        _dailyRewardUI.DoAnimPassDate(_oldDate);
    }

    public void ClaimReward()
    {
        int oldIndex = _daily.datePass;
        ClaimAndAnim();
        var reward = GameConfig.lstDailyReward[oldIndex];
        _dailyRewardUI.ActiveClaimBtn(false);
    }
    public void ApplyReward(DailyRewardData data)
    {
        foreach (var r in data.rewards)
        {
            Debug.Log($"CheckRewardType: {r.type} + {r.amount}");
            switch (r.type)
            {
                case RewardType.Coin:
                    // DBController.Instance.COIN += r.amount;
                    UITopController.Instance.ShowTab(() =>
                    {
                        AnimatorHelper.Instance.MultiObject(PrefabObjectFly.Coin, Mathf.Min(10, r.amount / 50), UITopController.Instance.CoinUI.tfmCoinText,
                            () =>
                            {
                                CoinController.Instance.AddCoin(r.amount);
                            });
                    });
                    break;

                case RewardType.Energy:
                    UITopController.Instance.ShowTab(() =>
                    {
                        AnimatorHelper.Instance.MultiObject(PrefabObjectFly.Energy, Mathf.Min(10, r.amount / 50), UITopController.Instance.EnergyUI.tfmEnergyText,
                            () =>
                            {
                                EnergyController.Instance.AddEnergy(r.amount);
                            });
                    });
                    break;

                case RewardType.Exp:
                    UITopController.Instance.ShowTab(() =>
                    {
                        AnimatorHelper.Instance.MultiObject(PrefabObjectFly.Exp, Mathf.Min(10, r.amount / 50), UITopController.Instance.ExpBarView.tfmExpText,
                            () =>
                            {
                                ExpBarController.Instance.AddExp(r.amount);
                            });
                    });
                    break;

                case RewardType.BoosterHammer:
                    UITopController.Instance.ShowTab(() =>
                    {
                        AnimatorHelper.Instance.MultiObject(PrefabObjectFly.Hammer, r.amount, UITopController.Instance.ExpBarView.tfmExpText,
                            async () =>
                            {
                                DBController.Instance.BOOSTER_HAMMER += r.amount;
                                await UniTask.Delay(2000);
                                UITopController.Instance.HideTab();
                            });
                    });
                    break;

                case RewardType.BoosterSwap:
                    UITopController.Instance.ShowTab(() =>
                    {
                        AnimatorHelper.Instance.MultiObject(PrefabObjectFly.Swap, r.amount, UITopController.Instance.ExpBarView.tfmExpText,
                            async () =>
                            {
                                DBController.Instance.BOOSTER_SWAP += r.amount;
                                
                                await UniTask.Delay(2000);
                                UITopController.Instance.HideTab();
                            });
                    });
                    break;

                case RewardType.BoosterBroom:
                    UITopController.Instance.ShowTab(() =>
                    {
                        AnimatorHelper.Instance.MultiObject(PrefabObjectFly.Broom, r.amount, UITopController.Instance.ExpBarView.tfmExpText,
                            async ()  =>
                            {
                                DBController.Instance.BOOSTER_BROOM += r.amount;

                                await UniTask.Delay(2000);
                                UITopController.Instance.HideTab();
                            });
                    });
                    break;
            }
        }
    }

    DateTime ConvertTickToDateTime(long timeTick)
    {
        try
        {
            return new DateTime(timeTick);
        }
        catch (Exception ex)
        {
            Debug.Log($"Corrupt data {ex}");
            return DateTime.Now;
        }
    }

    void SetTimeRemain()
    {
        DateTime now = DateTime.Now;
        DateTime nextClaimTime = new DateTime(_daily.dateTimeLastTimeClaimRewardTick);

        _timeRemain = nextClaimTime - now;

        if (_timeRemain < TimeSpan.Zero)
            _timeRemain = TimeSpan.Zero;

        _dailyRewardUI.SetTimeRemain(_timeRemain);
        _isCountDown = _timeRemain > TimeSpan.Zero;
    }
    public static DateTime GetNextResetTime()
    {
        DateTime todayReset = DateTime.Today.AddHours(GameConfig.DAILY_RESET_HOUR);
        DateTime now = DateTime.Now;

        return (now >= todayReset)
            ? todayReset.AddDays(1)
            : todayReset;
    }
    public static bool IsResetExpired(DateTime lastClaim)
    {
        return DateTime.Now >= GetNextResetTime();
    }
    public static bool IsAfterReset(DateTime lastClaim)
    {
        return DateTime.Now >= GetNextResetTime();
    }

    #region Cheat
    public void CheatClaimToday()
    {
        Debug.Log("CHEAT DAILY REWARD!");
        
        _isCountDown = false;
        _timeRemain = TimeSpan.Zero;
        _dailyRewardUI.SetTimeRemain(_timeRemain);
        _dailyRewardUI.ActiveClaimBtn(true);
        if (_daily.datePass >= 7)
            _daily.datePass = 0;

        ClaimReward();

        Debug.Log($"CHEAT DONE — Claimed Day: {_daily.datePass}");
    }
    
    public void CheatFullResetDay()
    {
        Debug.Log("🔥 CHEAT: FULL DAILY RESET");
        
        _daily.datePass = 0;
        
        _daily.dateTimeLastTimeClaimRewardTick = DateTime.Now.Ticks;

        DBController.Instance.DAILY_REWARD = _daily;
        
        CheckPassDay();

        Debug.Log("🔥 CHEAT DONE — Daily system fully reset.");
    }

    public void CheatNextDay()
    {
        _daily.dateTimeLastTimeClaimRewardTick -= TimeSpan.FromDays(1).Ticks;
        DBController.Instance.DAILY_REWARD = _daily;
        CheckPassDay();
    }


    #endregion
}

[Serializable]
public class DailyReward
{
    public int datePass;
    public long dateTimeLastTimeClaimRewardTick = 0;
}