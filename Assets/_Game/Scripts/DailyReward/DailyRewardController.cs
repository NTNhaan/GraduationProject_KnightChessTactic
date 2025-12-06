using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
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
        // _dailyRewardUI.InitRewardUI();
        // _dailyRewardUI.ActiveClaimBtn(false);
        // DateTime lastClaim = new DateTime(_daily.dateTimeLastTimeClaimRewardTick);
        // DateTime nextReset = GetNextResetTime();
        //
        // bool isExpired = DateTime.Now >= nextReset;
        // bool canClaim = DateTime.Now >= lastClaim && DateTime.Now < nextReset;
        //
        // if (isExpired)
        // {
        //     _daily.datePass = 0;
        //     _db.DAILY_REWARD = _daily;
        //     _isCountDown = false;
        //     _dailyRewardUI.SetTimeRemain(TimeSpan.Zero);
        //     _dailyRewardUI.ResetAllUI();
        //     _dailyRewardUI.ActiveClaimBtn(true);
        //     _dailyRewardUI.ActiveCurrentDate(0, true);
        //     return;
        // }
        // if (canClaim && _daily.datePass == 0)
        // {
        //     _dailyRewardUI.ResetAllUI();
        //     _dailyRewardUI.ActiveClaimBtn(true);
        //     _dailyRewardUI.ActiveCurrentDate(0, true);
        //     return;
        // }
        // if (canClaim)
        // {
        //     _dailyRewardUI.ActiveClaimBtn(true);
        //     _dailyRewardUI.ActiveCurrentDate(_daily.datePass, true);
        // }
        // else
        // {
        //     SetTimeRemain();
        // }
        //
        // _dailyRewardUI.ShowPassDate(_daily.datePass);
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

        if (reward.coin > 0)
        {
            _db.COIN += reward.coin;
        }

        if (reward.boosterId == 1)
        {
            _db.BOOSTER_HAMMER += reward.boosterAmount;
        }
        else if (reward.boosterId == 2)
        {
            _db.BOOSTER_SWAP += reward.boosterAmount;
        }
        else if (reward.boosterId == 3)
        {
            _db.BOOSTER_BROOM += reward.boosterAmount;
        }
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
        Debug.Log($"CheckReward: {reward.boosterId} {reward.coin} {reward.boosterAmount}");
        // _gameHelper.ScreenController.MainScreen.ShowDiamond();  // Update Text and Score Text
        _dailyRewardUI.ActiveClaimBtn(false);

        // GameHelper.Instance.SoundController.PlaySound(SoundName.ClaimReward);
        if (reward.coin > 0)
        {
            AnimatorHelper.Instance.MultiObject(
                PrefabObjectFly.Coin,
                Mathf.Min(10, reward.coin / 50), 
                UITopController.Instance.CoinUI.tfmCoinText
            );
        }
        if (reward.boosterId > 0 && reward.boosterAmount > 0)
        {
            if (reward.boosterId == 1)
            {
                AnimatorHelper.Instance.MultiObject(
                    PrefabObjectFly.Hammer,
                    reward.boosterAmount,
                    UITopController.Instance.CoinUI.tfmCoinText
                );
            }

            if (reward.boosterId == 2) 
            {
                AnimatorHelper.Instance.MultiObject(
                    PrefabObjectFly.Swap,
                    reward.boosterAmount,
                    UITopController.Instance.CoinUI.tfmCoinText
                );
            }
            if (reward.boosterId == 3) 
            {
                AnimatorHelper.Instance.MultiObject(
                    PrefabObjectFly.Broom,
                    reward.boosterAmount,
                    UITopController.Instance.CoinUI.tfmCoinText
                );
            }
        }
    }

    public void ClaimDoubleReward()
    {
        var reward = GameConfig.lstDailyReward[_daily.datePass - 1];

        if (reward.boosterId == 0)
        {
            // x2 diamond
            _db.COIN += reward.coin * 2;
        }
        else
        {
            // x2 booster
            _db.ADD_BOOSTER(reward.boosterId, reward.boosterAmount * 2);
        }

        ClaimAndAnim();
        // _gameHelper.ScreenController.MainScreen.ShowDiamond();
        _dailyRewardUI.ActiveClaimBtn(false);
        // ClaimAndAnim();
        // _db.DIAMOND += _diaReward;
        // _gameHelper.ScreenController.MainScreen.ShowDiamond();
        // _gameHelper.GiftController.RandomNormalGiftAndUpdateUI();
        // _dailyRewardUI.ActiveClaimBtn(false);
        // _gameHelper.FakeObjectFly.MultiObject(20,
        //     _gameHelper.ScreenController.MainScreen.tfmDiaText);
        
        // _gameHelper.WatchAdHelper.ShowRewardAd(() =>
        // {
        //     ClaimAndAnim();
        //     _db.DIAMOND += _diaReward;
        //     _gameHelper.ScreenController.MainScreen.ShowDiamond();
        //     _gameHelper.GiftController.RandomNormalGiftAndUpdateUI();
        //     _dailyRewardUI.ActiveClaimBtn(false);
        //     _gameHelper.FakeObjectFly.MultiObject(20,
        //         _gameHelper.ScreenController.MainScreen.tfmDiaText);
        // });
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
        // DateTime _dt = DateTime.Now;
        // DateTime _dtEndOfDay = ConvertTickToDateTime(_daily.dateTimeLastTimeClaimRewardTick); // 22h
        // _timeRemain = _dtEndOfDay - _dt;
        // Debug.Log($"SetTimeRemain {_dtEndOfDay} - {_dt} = {_timeRemain}");
        // _dailyRewardUI.SetTimeRemain(_timeRemain);
        // _isCountDown = true;
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