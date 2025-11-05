using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Data;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using  UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using Object = System.Object;

public class GamePlayUI : MonoBehaviour
{
    [Header("Coin")]
    [SerializeField] private Transform coinBanner;
    [SerializeField] private Text coinText;
    
    [Header("Score Menu")]
    [SerializeField] private float highScore;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text bestScoreText;

    [SerializeField] private Text TimerText;
    public Text PanelScoreText;
    public Text HighScoreText;

    [SerializeField] private Image imgBG;
    void Start()
    {
        ScoreController.Instance.OnScoreChanged += UpdateScoreText;
        ScoreController.Instance.OnBestScoreChanged += UpdateBestScoreText;
        ScoreController.Instance.OnHealthChanged += UpdateHealthPlayer;
        EventDispatcher.Register(EventId.OnCoinChanged, UpdateCoinText);
        EventDispatcher.Register(EventId.OnPlayerDead, EnableCoinBanner);
        EventDispatcher.Register(EventId.OnThemeChanged, OnThemeChanged);
        UpdateCoinText();
        string selectedTheme = DBController.Instance.SELECTED_THEME;
        Debug.Log($"CheckStartGame {selectedTheme}");
        UpdateBackground(selectedTheme);
    }
    void OnDestroy()
    {
        ScoreController.Instance.OnScoreChanged -= UpdateScoreText;
        ScoreController.Instance.OnBestScoreChanged -= UpdateBestScoreText;
        ScoreController.Instance.OnHealthChanged -= UpdateHealthPlayer;
        EventDispatcher.RemoveCallback(EventId.OnCoinChanged, UpdateCoinText);
        EventDispatcher.RemoveCallback(EventId.OnPlayerDead, EnableCoinBanner);
        EventDispatcher.RemoveCallback(EventId.OnThemeChanged, OnThemeChanged);
    }

    private void OnThemeChanged(object data = null)
    {
        if (data is string themeId)
        {
            UpdateBackground(themeId);
        }
        else
        {
            Debug.LogWarning("[GamePlayUI] Không nhận được themeId hợp lệ từ event.");
        }
    }
    
    private void UpdateBackground(string themeId)
    {
        var model = DataSOController.Instance.GetModelByID(themeId);
        if (model != null && model.backgroundImage != null)
        {
            if(imgBG ==null) return;
            imgBG.sprite = model.backgroundImage;
            Debug.Log($"[GamePlayUI] Background changed to {themeId}");
        }
        else
        {
            Debug.LogWarning($"[GamePlayUI] Không tìm thấy model hoặc sprite cho themeId = {themeId}");
        }
    }
    private void Update()
    {
        bestScoreText.text = ScoreController.Instance.GetHighScore().ToString();
        scoreText.text = ScoreController.Instance.Score.ToString();
        // HealthPlayer.text = scoreManager.GetHealthPlayer().ToString();
        TimerText.text = TimeManager.Instance.GetFormattedTime();
        
    }
    void UpdateCoinText(object data = null)
    {
        Debug.Log("CheckkCoin: " + DBController.Instance.COIN);
        coinText.text = DBController.Instance.COIN.ToString("N0");
    }
    void UpdateScoreText(int score)
    {
        scoreText.text = ScoreController.Instance.Score.ToString();
    }
    void UpdateBestScoreText(int score)
    {
        bestScoreText.text = ScoreController.Instance.HighScore.ToString();
    }
    void UpdatePanelScoreText(int score)
    {
        PanelScoreText.text = "Score: " +  ScoreController.Instance.Score;
    }
    void UpdateHealthPlayer(int health)
    {
        // HealthPlayer.text = ScoreController.Instance.HealthPlayer.ToString();
    }

    void EnableCoinBanner(Object data = null)
    {
        coinBanner.DOScale(0f, .3f).SetEase(Ease.OutBack);
    }
}
