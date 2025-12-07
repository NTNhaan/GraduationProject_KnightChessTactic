using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig
{
    public static GameMode GameMode = 0;
    public static readonly int ROTATE_COIN = 50;
    public static readonly int END_GAME_REWARD = 500;
    public static int ROTATE_COUNT = 0;
    public static int PAUSE_TIME = 0;
    public static readonly int COIN_PLUS = 50;
    public static readonly int COIN_BOOSTER = 300;
    public static readonly int COIN_REVIVE = 200;
    public static readonly int DAILY_COIN = 250;
    public static readonly int EXP_PLUS = 100;
    public static readonly int EXP_PLUS_GUIDE = 200;
    public static readonly int TIME_BOOSTER = 10000;
    public static readonly float SPEED_REDUCE = .1f;
    public static readonly float SPEED_ORIGINAL = 2.5f;
    
    public static readonly int SCORE_PER_MINUTE = 100;
    public static readonly float TIME_BONUS = 60f; // 1'
    public static readonly float REVERSE_TIME = 150f; // 2'30s
    public static readonly float TIME_SPAWN_BONUS = 5f;
    public static readonly float TIME_USE_BOOSTER = 5f;
    
    [Header("SpinRW")]
    public static readonly int SPIN_ENERGY = 10;
    public static readonly int MAX_ENERGY = 100;
    [Header("Energy Config")]
    public static readonly int RecoverEnergy = 5;
    public static readonly int SpinBreak = 5;
    public static readonly int ACCUMULATE = 10;
    public static readonly int MIN_EXP_ACCUMULATE = 50;
    public static readonly int MAX_EXP_ACCUMULATE = 100;
    public static readonly int RW_ENERGY = 5;
    public static readonly int DAILY_RESET_HOUR = 24;
    public static readonly List<DailyRewardData> lstDailyReward = new()
    {
        new DailyRewardData
        {
            rewards = new List<RewardItem>()
            {
                new RewardItem { type = RewardType.Coin, amount = 100 }
            }
        },
        new DailyRewardData
        {
            rewards = new List<RewardItem>()
            {
                new RewardItem { type = RewardType.Coin, amount = 200 }
            }
        },
        new DailyRewardData
        {
            rewards = new List<RewardItem>()
            {
                new RewardItem { type = RewardType.Energy, amount = 50 }
            }
        },
        new DailyRewardData
        {
            rewards = new List<RewardItem>()
            {
                new RewardItem { type = RewardType.BoosterHammer, amount = 3 }
            }
        },
        new DailyRewardData
        {
            rewards = new List<RewardItem>()
            {
                new RewardItem { type = RewardType.Exp, amount = 500 }
            }
        },
        new DailyRewardData
        {
            rewards = new List<RewardItem>()
            {
                new RewardItem { type = RewardType.Coin, amount = 500 }
            }
        },
        new DailyRewardData
        {
            rewards = new List<RewardItem>()
            {
                new RewardItem { type = RewardType.Coin, amount = 1000 },
                new RewardItem { type = RewardType.BoosterHammer, amount = 1 },
                new RewardItem { type = RewardType.BoosterSwap, amount = 1 },
                new RewardItem { type = RewardType.BoosterBroom, amount = 1 }
            }
        }
    };
}

public enum GameMode
{
    Level = 0,
    Endless = 1,
    PvE = 2,
}