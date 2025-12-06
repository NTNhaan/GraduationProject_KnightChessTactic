using System.Collections.Generic;
using Data;
using UnityEngine;

public class LeaderboardController : Singleton<LeaderboardController>
{
    public LeaderboardData data;
    private LeaderboardState currentState;

    [SerializeField] private Transform content;
    [SerializeField] private LeaderboardItem itemPrefab;
    [SerializeField] private TextAsset jsonFile;
    
    protected override void CustomAwake()
    {
        Debug.Log($"CheckLoadDataa 1");
        LoadData();
        Debug.Log($"CheckLoadDataa 2");
    }

    private void LoadData()
    {
        if (jsonFile == null)
        {
            Debug.LogError("[LEADERBOARD] JSON file not assigned!");
            return;
        }

        Debug.Log("JSON TEXT = " + jsonFile.text);
        data = JsonUtility.FromJson<LeaderboardData>(jsonFile.text);

        Debug.Log("World Count = " + data.Level_World?.Count);
        
        data.Level_World ??= new List<LeaderboardEntry>();
        data.Level_Vietnam ??= new List<LeaderboardEntry>();
        data.Endless_World ??= new List<LeaderboardEntry>();
        data.Endless_Vietnam ??= new List<LeaderboardEntry>();
    }

    private void Start()
    {
        currentState = new LeaderboardState(LeaderboardMode.Level, LeaderboardCountry.World);
        LoadLeaderboard();
    }

    public void SetMode(LeaderboardMode mode)
    {
        currentState.mode = mode;
        LoadLeaderboard();
    }

    public void SetCountry(LeaderboardCountry country)
    {
        currentState.country = country;
        LoadLeaderboard();
    }

    private void LoadLeaderboard()
    {
        // Xóa items cũ
        foreach (Transform c in content)
            Destroy(c.gameObject);

        // Chọn danh sách phù hợp
        List<LeaderboardEntry> list = GetDataByState();

        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("[LEADERBOARD] No data for selected tab!");
            return;
        }

        // Spawn UI items
        int index = 0;
        foreach (var entry in list)
        {
            var item = Instantiate(itemPrefab, content);
            item.SetData(entry, index);
            index++;
        }
    }

    private List<LeaderboardEntry> GetDataByState()
    {
        if (currentState.mode == LeaderboardMode.Level)
        {
            return currentState.country == LeaderboardCountry.World
                ? data.Level_World
                : data.Level_Vietnam;
        }
        else // Endless
        {
            return currentState.country == LeaderboardCountry.World
                ? data.Endless_World
                : data.Endless_Vietnam;
        }
    }
}
