using System;

[Serializable]
public class LeaderBoardData
{
    public int rank;
    public string name;
    public int score;
    public string time;
    
    public LeaderBoardData(int rank, string name, int score, string time)
    {
        this.rank = rank;
        this.name = name;
        this.score = score;
        this.time = time;
    }
}