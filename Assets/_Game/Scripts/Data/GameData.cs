using UnityEngine;
using System.Collections.Generic;
namespace Data
{
    public class GameData : Singleton<GameData>
    {
        [SerializeField] private LevelData levelDataSo;

        public List<LevelData.LevelInfo> Levels => levelDataSo != null ? levelDataSo.levels : null;
        public LevelData LevelDataSo
        {
            get { return levelDataSo; }
            set { levelDataSo = value; }
        }

        private void Awake()
        {
            Debug.Log($"[ObstacleSpawner] Start called Awake");
            Debug.Log($"[GameData] Awake - levelDataSO = {Levels}");
        }

        public bool IsNoAds() => false;
    }
}