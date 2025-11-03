using System;
using Data;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private float elapsedTime = 0f;
    private bool isRunning = false;

    public event Action OnTimeChanged;
    public event Action<int> OnTimeMilestone;

    public float ElapsedTime => elapsedTime;

    private float nextActionTime = 60f;

    private void OnEnable()
    {
        EventDispatcher.Register(EventId.OnPlayerDead, OnGameOver);
    }
    private void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.OnPlayerDead, OnGameOver);
    }
    private void Update()
    {
        if (!isRunning 
            || InGameData.GAME_STATE == GameState.GameOver 
            || InGameData.GAME_STATE == GameState.PauseGame
            || InGameData.GAME_STATE == GameState.Loading
            || InGameData.GAME_STATE == GameState.GiveUp)
            return;

        elapsedTime += Time.deltaTime;
        OnTimeChanged?.Invoke();
        
        if (elapsedTime >= nextActionTime)
        {
            int minutesPassed = Mathf.FloorToInt(elapsedTime / 60);
            OnTimeMilestone?.Invoke(minutesPassed);
            EventDispatcher.Push(EventId.OnTimeBonus);
            nextActionTime += GameConfig.TIME_BONUS;
        }
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        nextActionTime = GameConfig.TIME_BONUS;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    public void OnGameOver(object data = null)
    {
        int currentTime = Mathf.FloorToInt(elapsedTime);
        DBController.Instance.LAST_PLAY_TIME = currentTime;
        EventDispatcher.Push(EventId.OnSaveTime);
        Debug.Log($"BestPlayTime: {DBController.Instance.BEST_PLAY_TIME} CurrentTime: {DBController.Instance.LAST_PLAY_TIME}");
        if (currentTime >= DBController.Instance.BEST_PLAY_TIME)
        {
            DBController.Instance.BEST_PLAY_TIME = currentTime;
        }
    }
}
