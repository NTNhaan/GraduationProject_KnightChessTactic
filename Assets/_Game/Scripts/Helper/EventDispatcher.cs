using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EventDispatcher
{
    private static Dictionary<EventId, UnityAction<object>> listenner = new Dictionary<EventId,UnityAction<object>>();

    public static void Register(EventId id, UnityAction<object> callback)
    {
        if (HasExistEvent(id)) // Check event exist
        {
            listenner[id] += callback;
        }
        else
        {
            listenner.Add(id, callback); // add new event if event don't exist
        }
    }
    public static void Push(EventId id, object data = null)
    {
        if (!HasExistEvent(id))
        {
            Debug.LogError($"Can not push. The event {id} not exist.");
            return;
        }
        
        listenner[id]?.Invoke(data);
    }
    public static bool HasExistEvent(EventId id)
    {
        return listenner.ContainsKey(id);
    }
    public static void RemoveCallback(EventId id, UnityAction<object> callback)
    {
        if (!HasExistEvent(id))
        {
            Debug.LogError($"Can not remove callback. The event {id} not exist.");
            return;
        }
        listenner[id] -= callback;

        if (listenner[id] == null)
        {
            RemoveEvent(id);
        }
    }
    public static void RemoveEvent(EventId id)
    {
        if (!HasExistEvent(id))
        {
            Debug.LogError($"Can not remove event. The event {id} not exist.");
            return;
        }
        listenner.Remove(id);
    }
    public static void RemoveAll()
    {
        listenner.Clear();
    }
}

public enum EventId
{
    OnScoreChange = 0,
    OnBestScoreChange = 1,
    OnPlayerDead = 2,
    OnPlayerRevive = 3,
    OnPlayerJump = 4,
    OnGamePlayScreen = 5,
    OnMainScreen = 6,
    OnTutorialScreen = 7,
    OnSaveTime = 8,
    OnTimeBonus = 9,
    OnCoinChanged = 10,
    OnClockPatternChanged = 11,
    OnThemeChanged = 12,
    OnTutorialShow = 13,
    OnTutorialHide = 14,
    OnHideLineGuide = 15,
    OnSoundClick = 16,
    
    OnGameStateChanged = 17,
    OnCheatChangeSpeed = 18,
}
