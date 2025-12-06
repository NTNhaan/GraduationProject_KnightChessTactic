using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spin
{
    public class DataRewardSpin : MonoBehaviour
    {
        public List<ModelDataSpin> Data = new List<ModelDataSpin>();



        private void Start()
        {
            SetUpReward();
        }

        private void SetUpReward()
        {
            for (int i = 0; i < Data.Count; i++)
            {
                Debug.Log($"Setup Reward Index {i} : {Data[i].rewardType}");
                switch (Data[i].rewardType)
                {
                    case SpinRewardType.None:
                        Debug.LogError($"Reward {i} type none");
                        break;
                    case SpinRewardType.Energy:
                        Data[i].rewardSpin = new EnergyReward(Data[i].value);
                        break;
                    case SpinRewardType.Coin:
                        Data[i].rewardSpin = new CoinReward(Data[i].value);
                        break;
                    case SpinRewardType.Exp:
                        Data[i].rewardSpin = new ExpReward(Data[i].value);

                        break;
                    default:
                        break;
                }
            }
        }

        public ModelDataSpin GetDataSpinByIndex(int index)
        {
            return Data[index];
        }
    }

    [Serializable]
    public class ModelDataSpin
    {
        public IRewardSpinHandler rewardSpin;
        public SpinRewardType rewardType;
        public int value;
        public int percent;
        public Sprite sprReward;
    }

    public enum SpinRewardType
    {
        None = 0,
        Energy = 1,
        Coin = 2,
        Exp = 3,
    }
}