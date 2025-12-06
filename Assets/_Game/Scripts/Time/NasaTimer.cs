using Cysharp.Threading.Tasks;
using PS.NetworkTime;
using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class NasaTimer : Singleton<NasaTimer>
{
    public bool IsGettedTime = false;
    public long startTime = 0;
    public long offset = 0;
    public long currentTime = 0;
    public DateTime currentDateTime;
    
    void Start()
    {

        GetTime();

    }
    void OnRealTimeCallBack(TimeNetData data, string error)
    {
        Debug.Log("Get time");
        if (error != null)
        {
            Debug.Log($"OnRealTimeCallBack {error}");

            print($"error: {error}");
            GetTime();
            return;
        }
        offset = (int)(Time.realtimeSinceStartup * 1000);
        var dt = RealTimeNet.ConvertTimeStringToDateTime(data.dateTime);
        startTime = new DateTimeOffset(dt).ToUnixTimeMilliseconds();
        IsGettedTime = true;
        Debug.Log("GettedTime1");
    }
    public async UniTask GetTime()
    {
        Debug.Log("Start get time");
        RealTimeNet realTimeNet = new RealTimeNet();
        realTimeNet.RealTime.OnCompleted = OnRealTimeCallBack;
        realTimeNet.RealTime.GetNASATime();
    }
    public async UniTask<long> GetCurrentTime()
    {
        await UniTask.WaitUntil(() => IsGettedTime);
        return currentTime;
    }
    public async UniTask<string> GetCurrentTimeString()
    {
        await UniTask.WaitUntil(() => IsGettedTime);
        return DateTimeOffset.FromUnixTimeMilliseconds(currentTime).UtcDateTime.ToString();
    }

    private void Update()
    {
        if (!IsGettedTime)
            return;
        currentTime = startTime + (int)(Time.realtimeSinceStartup * 1000) - offset;
        currentDateTime = DateTimeOffset.FromUnixTimeMilliseconds(currentTime).UtcDateTime;
    }
}