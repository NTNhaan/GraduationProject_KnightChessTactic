using Audio;
using UnityEngine;
using Data; 
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class ScoreController : Singleton<ScoreController>
    {
        // private const string HIGH_SCORE_KEY = "HighScore";
        public event UnityAction<int> OnScoreChanged;
        public event UnityAction<int> OnBestScoreChanged;
        public event UnityAction<int> OnHealthChanged;
        public int Score {get; set;}
        public int HighScore {get; private set;}
        public int HealthPlayer {get; private set;}
        
        public int GetHighScore() => HighScore;
        public int GetHealthPlayer() => HealthPlayer;
        private void OnEnable()
        {
            // TimeManager.Instance.OnTimeMilestone += HandleTimeMilestone;
            EventDispatcher.Register(EventId.OnTimeBonus, HandleTimeMilestone);
        }

        private void OnDisable()
        {
            // TimeManager.Instance.OnTimeMilestone -= HandleTimeMilestone;
            EventDispatcher.RemoveCallback(EventId.OnTimeBonus, HandleTimeMilestone);
        }
        private void Start()
        {
            Score = 0;
            HealthPlayer = 1;
            HighScore = 0;
            
            HighScore = DBController.Instance.BEST_SCORE;
            OnScoreChanged?.Invoke(Score);
            OnBestScoreChanged?.Invoke(HighScore);
            OnHealthChanged?.Invoke(HealthPlayer);
        }
        public void AddPoints(int points)
        {
            Score += points;
            if (Score > HighScore)
            {
                HighScore = Score;
                DBController.Instance.BEST_SCORE = Score;
                EventDispatcher.Push(EventId.OnBestScoreChange);
                OnBestScoreChanged?.Invoke(HighScore);
            }
            
            OnScoreChanged?.Invoke(Score);
        }
        
        public void TakeDamage(int damage)
        {
            HealthPlayer -= damage;
            OnHealthChanged?.Invoke(HealthPlayer);
        }

        public void HandleTimeMilestone(object data = null)
        {
            AddPoints(GameConfig.SCORE_PER_MINUTE);
        }
    }
}