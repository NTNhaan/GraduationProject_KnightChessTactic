using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName ="LevelData",menuName ="SO/LevelSO/LevelDataSO")]
public class LevelData : ScriptableObject
{
    public List<LevelInfo> levels;
    [Serializable]
    public class LevelInfo
    {
        public int level;
        public int min_exp;
        public int max_exp;
        public int speedRotate;
        public List<ObstacleInfo> obstacleInfos;
    }
    [Serializable]
    public class ObstacleInfo
    {
        public GameObject obstacle;
        public List<DirectionType> direction;
        public int spawnRate;
        public float fallSpeed;
    }
}
