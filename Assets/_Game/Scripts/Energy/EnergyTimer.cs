using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using Data;
using UnityEngine;
using UnityEngine.Events;

namespace Energy
{
    public class EnergyTimer : Singleton<EnergyTimer>
    {
        private long totalTimeTicks = TimeSpan.FromMinutes(1).Ticks;
        private long currentTimeTicks;
        private EnergyController energyController;
        private bool isCounting = false;
        private long startTime = 0;
        private long offsetTime = 0;

        public UnityAction<TimeSpan> ActionUpdateTimeUI;
        public long CurrentTimeTicks { get => currentTimeTicks; }
        
        public async void Init(EnergyController energyController)
        {
            this.energyController = energyController;
            await LoadAppOpenTime();
            StartCountdown().Forget();
        }

        private async UniTask LoadAppOpenTime()
        {
            await UniTask.WaitUntil(() => NasaTimer.Instance.IsGettedTime);

            totalTimeTicks = TimeSpan.FromMinutes(GameConfig.RecoverEnergy).Ticks;

            long currentMs = await NasaTimer.Instance.GetCurrentTime();
            DateTime now = DateTimeOffset.FromUnixTimeMilliseconds(currentMs).UtcDateTime;

            long lastTicks = DBController.Instance.TIME_OFFLINE;

            // ❗ Lần đầu vào game
            if (lastTicks == 0)
            {
                DBController.Instance.TIME_OFFLINE = now.Ticks;
                currentTimeTicks = totalTimeTicks;
                ActionUpdateTimeUI?.Invoke(TimeSpan.FromTicks(currentTimeTicks));
                return;
            }

            long ticksPassed = now.Ticks - lastTicks;
            if (ticksPassed < 0) ticksPassed = 0; // chống lỗi giờ hệ thống

            // thời gian còn lại ở lần trước
            currentTimeTicks = DBController.Instance.TIME_RETURN_ENERGY;

            // ====== TÍNH HỒI ENERGY OFFLINE ======
            while (ticksPassed >= totalTimeTicks && DBController.Instance.ENERGY < GameConfig.MAX_ENERGY)
            {
                EnergyController.Instance.AddEnergy(1);
                ticksPassed -= totalTimeTicks;
            }

            // ====== TÍNH LẠI THỜI GIAN COUNTDOWN ======
            currentTimeTicks = totalTimeTicks - ticksPassed;
            if (currentTimeTicks < 0) currentTimeTicks = 0;

            DBController.Instance.TIME_RETURN_ENERGY = currentTimeTicks;
            DBController.Instance.TIME_OFFLINE = now.Ticks;

            ActionUpdateTimeUI?.Invoke(TimeSpan.FromTicks(currentTimeTicks));
        }

        // [ContextMenu("AA")]
        // private async UniTask LoadAppOpenTime()
        // {
        //     await UniTask.WaitUntil(() => NasaTimer.Instance.IsGettedTime);
        //     
        //     totalTimeTicks = TimeSpan.FromMinutes(GameConfig.RecoverEnergy).Ticks;
        //     
        //     var currentTime = await NasaTimer.Instance.GetCurrentTime();
        //     DateTime currentDateTime = DateTimeOffset.FromUnixTimeMilliseconds(currentTime).UtcDateTime;
        //
        //     long savedTimeTicks = DBController.Instance.TIME_OFFLINE;
        //     // Debug.Log($"currentDateTime: {currentDateTime}");
        //     // Debug.Log($"currentTime: {currentTime}");
        //     // Debug.Log($"savedTimeTicks: {DateTime.FromBinary(savedTimeTicks)}");
        //     currentTimeTicks = DBController.Instance.TIME_RETURN_ENERGY;
        //
        //     if (currentTimeTicks != totalTimeTicks || DBController.Instance.ENERGY < GameConfig.MAX_ENERGY)
        //     {
        //         var ticksPassed = Math.Abs(currentDateTime.Ticks - savedTimeTicks);
        //
        //         Debug.Log($"ticksPassed: {ticksPassed} : {currentDateTime.Ticks} : {savedTimeTicks}");
        //         if (ticksPassed <= currentTimeTicks)
        //         {
        //             currentTimeTicks -= ticksPassed;
        //         }
        //         else
        //         {
        //
        //             ticksPassed -= currentTimeTicks;
        //             int secondsPassed = (int)TimeSpan.FromTicks(ticksPassed).TotalSeconds;
        //             Debug.Log($"secondsPassed time: {secondsPassed}");
        //             EnergyController.Instance.AddEnergy(1);
        //
        //             ReturnEnergyOffline(secondsPassed);
        //
        //         }
        //     }
        //
        //     if (currentTimeTicks < 0)
        //     {
        //         currentTimeTicks = totalTimeTicks;
        //     }
        //
        //     DBController.Instance.TIME_RETURN_ENERGY = currentTimeTicks;
        //
        //     TimeSpan currentTimeSpan = TimeSpan.FromTicks(currentTimeTicks);
        //     ActionUpdateTimeUI?.Invoke(currentTimeSpan);
        //
        //     DBController.Instance.TIME_OFFLINE = currentDateTime.Ticks;
        // }

        private void ReturnEnergyOffline(int secondsPassed)
        {
            var curEnergy = DBController.Instance.ENERGY;
            var amountEnergy = secondsPassed / (int)(GameConfig.RecoverEnergy * 60);

            var energyReturn = curEnergy + amountEnergy;
            energyReturn = Mathf.Clamp(energyReturn, 0, GameConfig.MAX_ENERGY);
            energyController.UpdateEnergy(energyReturn);

            if (DBController.Instance.ENERGY < GameConfig.MAX_ENERGY)
            {

                var timeOff = secondsPassed % (GameConfig.RecoverEnergy * 60);
                currentTimeTicks = TimeSpan.FromSeconds(timeOff).Ticks;
            }

        }

        public async UniTaskVoid StartCountdown()
        {
            if (currentTimeTicks <= 0)
            {
                currentTimeTicks = totalTimeTicks;

            }

            if (isCounting) return;
            while (currentTimeTicks > 0 && DBController.Instance.ENERGY < GameConfig.MAX_ENERGY)
            {
                isCounting = true;
                await UniTask.Delay(1000);
                currentTimeTicks -= TimeSpan.TicksPerSecond;

                if (currentTimeTicks <= 0)
                {
                    currentTimeTicks = totalTimeTicks;
                    EnergyController.Instance.AddEnergy(1);

                }

                DBController.Instance.TIME_RETURN_ENERGY = currentTimeTicks;


                TimeSpan currentTimeSpan = TimeSpan.FromTicks(currentTimeTicks);
                ActionUpdateTimeUI?.Invoke(currentTimeSpan);
                SaveAppOpenTime().Forget();
            }
            isCounting = false;
        }


        private async UniTaskVoid SaveAppOpenTime()
        {
            await UniTask.WaitUntil(() => NasaTimer.Instance.IsGettedTime);

            var currentTime = await NasaTimer.Instance.GetCurrentTime();
            DateTime currentDateTime = DateTimeOffset.FromUnixTimeMilliseconds(currentTime).UtcDateTime;
            DBController.Instance.TIME_OFFLINE = currentDateTime.Ticks;
        }

        private async void OnDisable()
        {
            SaveAppOpenTime().Forget();
        }
    }
}



