using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Data
{
    [Serializable]
    public class Level
    {
        public int level;
        public int min_exp;
        public int max_exp;

        public Level(int level, int min_exp, int max_exp)
        {
            this.level = level;
            this.min_exp = min_exp;
            this.max_exp = max_exp;
        }
    }
    [Serializable]
    public class LevelDataConfig
    {
        public List<Level> levelData = new List<Level>();
    }
    [Serializable]
    public class UserProfile
    {
        // public string display_name;
        // public int avatar_id;
        public int coin;
        public int score;
        public int highScore;
        public int level;
        public int exp;
        public int rewardEndScreen;
        public int free_prop_time;
        public int coin_Plus_Per_Level;
    }
    [Serializable]
    public class GameSettingModel
    {
        // public MusicType musicType;
        // public BgType bgType;
        public bool sound;
        public bool vibration;
        public bool notif;
    }
    public enum MusicType
    {
        CLASSIC = 0,
        RELAXING = 1,
        OFF = 2
    }

    [Serializable]
    public class SkinUI
    {
        public Sprite skinImg;
        public GameObject hightlight;
    }
    public enum CharacterType
    {
        DOG = 0,
        CAT = 1
    }
    
    public enum SkinType
    {
        CUTE = 0,
        SILLY = 1
    }
    public enum GameState
    {
        None,
        Tutorial,
        GiveUp,
        GameOver,
        Counting,
        PauseGame,
        PlayingGame,
        LevelUp,
        Loading,
        LoadingDone,
        MainMenu,
        SelectSkin,
        UseBooster,
        ProgressDead
    }
    public enum TutorialType
    {
        None,
        ShowLine,
        WaitSpawnPoint,
        Jumping,
        PointsBonus,
        AvoidObstacle,
        TryJump,
        BreakTime,
        HowToPlay,
        EffectMask,
        Completed,
    }
    public enum ClockPattern
    {
        NormalDirection,
        ReverseBoth,            // 1) Cả hai đảo ngược
        HourClockwise_MinuteCCW, // 2) Giờ thuận, phút ngược
        HourCCW_MinuteClockwise, // 3) Giờ ngược, phút thuận
    }
    [Serializable]
    public class NumOfUseBooster
    {
        public int broomNum;
        public int hammerNum;
        public int swapNum;
    }
}