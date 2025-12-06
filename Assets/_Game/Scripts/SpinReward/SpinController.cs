using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Spin
{
    public class SpinController : Singleton<SpinController>
    {
        [SerializeField] private DataRewardSpin dataRewardSpin;
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private List<ItemSpin> lstItemSpin;

        private const int NUM_SEGMENT = 8;
        private int targetSegment;
        private int minRounds = 10;
        private int maxRounds = 20;
        private bool isSpinning = false;
        private bool isGetAccumulate = false;

        public event UnityAction<bool> OnSpinStateChanged;
        public event UnityAction<int,int,float> OnAccumulateChanged;
        public event UnityAction<int, Sprite> OnSpinReward;
        public event UnityAction<int> OnAccumulateReward;

        public int CurrentAccumulate => DBController.Instance.COUNT_SPINT;
        public int AccumulateTarget => GameConfig.ACCUMULATE;

        public void InitSpin()
        {
            for (int i = 0; i < lstItemSpin.Count; i++)
            {
                lstItemSpin[i].Init(
                    dataRewardSpin.Data[i].value,
                    dataRewardSpin.Data[i].sprReward
                );
            }
            NotifyAccumulateUI();
        }

        private int GetRandomPoint()
        {
            int totalPercent = 0;
            for (int i = 0; i < dataRewardSpin.Data.Count; i++)
                totalPercent += dataRewardSpin.Data[i].percent;

            int randPoint = Random.Range(0, totalPercent);
            return randPoint;
        }

        private int GetTargetSegment()
        {
            int point = GetRandomPoint();
            int cumulative = 0;

            for (int i = 0; i < dataRewardSpin.Data.Count; i++)
            {
                cumulative += dataRewardSpin.Data[i].percent;
                if (point < cumulative)
                    return i;
            }
            return 0;
        }
        public void TrySpin()
        {
            if (isSpinning) return;
            if (DBController.Instance.ENERGY < GameConfig.SPIN_ENERGY) return;

            EnergyController.Instance.AddEnergy(-GameConfig.SPIN_ENERGY);
            StartSpin();
        }

        private void StartSpin()
        {
            isSpinning = true;
            EventDispatcher.Push(EventId.OnSpinStateChanged, true);

            targetSegment = GetTargetSegment();
            Debug.Log($"🎯 SPIN TARGET = {targetSegment}");

            if (DBController.Instance.IS_FIRST_SPIN)
            {
                targetSegment = 5;
                DBController.Instance.IS_FIRST_SPIN = false;
            }

            int rounds = Random.Range(minRounds, maxRounds + 1);
            float anglePer = 360f / NUM_SEGMENT;
            float finalAngle = rounds * 360f + targetSegment * anglePer;

            wheelTransform
                .DORotate(new Vector3(0, 0, -finalAngle), 10f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => StartCoroutine(OnSpinComplete()));
        }

        private IEnumerator OnSpinComplete()
        {
            Debug.Log($"💠 SPIN COMPLETE → PROCESSING REWARD...");

            isGetAccumulate = false;

            // Lấy reward data
            var resultData = dataRewardSpin.GetDataSpinByIndex(targetSegment);

            yield return new WaitForSeconds(0.5f);

            resultData.rewardSpin.ApplyReward();

            // Gửi event trả thưởng
            EventDispatcher.Push(EventId.OnSpinReward, resultData);
        }

        public void AddAccumulateCountSpin()
        {
            if (!isGetAccumulate)
                DBController.Instance.COUNT_SPINT++;

            // accumulate FULL → trả EXP reward
            if (DBController.Instance.COUNT_SPINT >= GameConfig.ACCUMULATE)
            {
                DBController.Instance.COUNT_SPINT = 0;

                int exp = Random.Range(
                    GameConfig.MIN_EXP_ACCUMULATE,
                    GameConfig.MAX_EXP_ACCUMULATE + 1
                );

                isGetAccumulate = true;
                ExpBarController.Instance.AddExp(exp);

                EventDispatcher.Push(EventId.OnAccumulateReward, exp);
            }

            isSpinning = false;
            EventDispatcher.Push(EventId.OnSpinStateChanged, false);
        }
        // public void AddAccumulateCountSpin()
        // {
        //     if (!isGetAccumulate)
        //         DBController.Instance.COUNT_SPINT++;
        //
        //     NotifyAccumulateUI();
        //
        //     if (DBController.Instance.COUNT_SPINT >= GameConfig.ACCUMULATE)
        //     {
        //         DBController.Instance.COUNT_SPINT = 0;
        //
        //         int exp = Random.Range(
        //             GameConfig.MIN_EXP_ACCUMULATE,
        //             GameConfig.MAX_EXP_ACCUMULATE + 1
        //         );
        //
        //         isGetAccumulate = true;
        //
        //         // gửi event EXP reward
        //         EventDispatcher.Push(EventId.OnAccumulateReward, exp);
        //
        //         NotifyAccumulateUI();
        //     }
        //
        //     isSpinning = false;
        //     EventDispatcher.Push(EventId.OnSpinStateChanged, false);
        // }

        private void NotifyAccumulateUI()
        {
            int cur = DBController.Instance.COUNT_SPINT;
            int max = GameConfig.ACCUMULATE;
            float fill = max > 0 ? (float)cur / max : 0f;

            // gửi Vector3(cur, max, fill)
            EventDispatcher.Push(EventId.OnAccumulateChanged, new Vector3(cur, max, fill));
        }
    }

}
