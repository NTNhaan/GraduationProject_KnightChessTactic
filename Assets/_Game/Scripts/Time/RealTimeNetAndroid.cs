using UnityEngine;
using UnityEngine.Events;

namespace PS.NetworkTime
{
    public class RealTimeNetAndroid : IRealTimeNetBridge
    {
        public UnityAction<TimeNetData, string> OnCompleted { get; set; }

        public void GetTimeByPublicIp(string publicIp)
        {
            Debug.LogWarning("[RealTimeNetAndroid] Not implemented - using Unity fallback");
            // Fallback to Unity implementation
            var unityImpl = new RealTimeNetUnity();
            unityImpl.OnCompleted = OnCompleted;
            unityImpl.GetTimeByPublicIp(publicIp);
        }

        public void GetUtcTime()
        {
            Debug.LogWarning("[RealTimeNetAndroid] Not implemented - using Unity fallback");
            // Fallback to Unity implementation
            var unityImpl = new RealTimeNetUnity();
            unityImpl.OnCompleted = OnCompleted;
            unityImpl.GetUtcTime();
        }

        public void GetNASATime()
        {
            Debug.LogWarning("[RealTimeNetAndroid] Not implemented - using Unity fallback");
            // Fallback to Unity implementation
            var unityImpl = new RealTimeNetUnity();
            unityImpl.OnCompleted = OnCompleted;
            unityImpl.GetNASATime();
        }
    }
}

