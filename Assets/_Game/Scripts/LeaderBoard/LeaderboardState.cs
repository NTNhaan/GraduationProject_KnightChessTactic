using System;
using System.Collections.Generic;

[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public int level;
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> Level_World;
    public List<LeaderboardEntry> Level_Vietnam;
    public List<LeaderboardEntry> Endless_World;
    public List<LeaderboardEntry> Endless_Vietnam;
}

public enum LeaderboardMode { Level, Endless }
public enum LeaderboardCountry { World, Vietnam }

[Serializable]
public struct LeaderboardState
{
    public LeaderboardMode mode;
    public LeaderboardCountry country;

    public LeaderboardState(LeaderboardMode m, LeaderboardCountry c)
    {
        mode = m;
        country = c;
    }
}