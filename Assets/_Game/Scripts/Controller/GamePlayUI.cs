using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Data;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using Object = System.Object;

/// <summary>
/// Manages all UI elements in gameplay screen
/// Handles score, coin, timer, background updates
/// </summary>
public class GamePlayUI : MonoBehaviour
{
    [Header("Coin UI")]
    [SerializeField] private Transform coinBanner;
    [SerializeField] private Text coinText;

    [Header("Score UI")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text bestScoreText;
    [SerializeField] public Text panelScoreText;
    [SerializeField] public Text highScoreText;

    [Header("Timer UI")]
    [SerializeField] private Text timerText;

    [Header("Background")]
    [SerializeField] private Image imgBG;

    private void Start()
    {
        // Subscribe to events
        ScoreController.Instance.OnScoreChanged += UpdateScoreText;
        ScoreController.Instance.OnBestScoreChanged += UpdateBestScoreText;
        ScoreController.Instance.OnHealthChanged += UpdateHealthPlayer;

        EventDispatcher.Register(EventId.OnCoinChanged, UpdateCoinText);
        EventDispatcher.Register(EventId.OnPlayerDead, EnableCoinBanner);
        EventDispatcher.Register(EventId.OnThemeChanged, OnThemeChanged);

        // Initialize UI
        UpdateCoinText();
        string selectedTheme = DBController.Instance.SELECTED_THEME;
        Debug.Log($"[GamePlayUI] Selected theme: {selectedTheme}");
        UpdateBackground(selectedTheme);
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (ScoreController.Instance != null)
        {
            ScoreController.Instance.OnScoreChanged -= UpdateScoreText;
            ScoreController.Instance.OnBestScoreChanged -= UpdateBestScoreText;
            ScoreController.Instance.OnHealthChanged -= UpdateHealthPlayer;
        }

        EventDispatcher.RemoveCallback(EventId.OnCoinChanged, UpdateCoinText);
        EventDispatcher.RemoveCallback(EventId.OnPlayerDead, EnableCoinBanner);
        EventDispatcher.RemoveCallback(EventId.OnThemeChanged, OnThemeChanged);
    }

    private void Update()
    {
        // Update UI every frame (lightweight operations)
        if (bestScoreText != null && ScoreController.Instance != null)
        {
            bestScoreText.text = ScoreController.Instance.GetHighScore().ToString();
        }

        if (scoreText != null && ScoreController.Instance != null)
        {
            scoreText.text = ScoreController.Instance.Score.ToString();
        }

        if (timerText != null && TimeManager.Instance != null)
        {
            timerText.text = TimeManager.Instance.GetFormattedTime();
        }
    }

    #region Score Updates

    private void UpdateScoreText(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = ScoreController.Instance.Score.ToString();
        }

        if (panelScoreText != null)
        {
            panelScoreText.text = "Score: " + ScoreController.Instance.Score;
        }
    }

    private void UpdateBestScoreText(int score)
    {
        if (bestScoreText != null)
        {
            bestScoreText.text = ScoreController.Instance.HighScore.ToString();
        }

        if (highScoreText != null)
        {
            highScoreText.text = ScoreController.Instance.HighScore.ToString();
        }
    }

    private void UpdateHealthPlayer(int health)
    {
        // Health UI update can be added here if needed
        // Currently handled by Character.cs
    }

    #endregion

    #region Coin Updates

    private void UpdateCoinText(object data = null)
    {
        if (coinText != null && DBController.Instance != null)
        {
            coinText.text = DBController.Instance.COIN.ToString("N0");
        }
    }

    private void EnableCoinBanner(Object data = null)
    {
        if (coinBanner != null)
        {
            coinBanner.DOScale(0f, 0.3f).SetEase(Ease.OutBack);
        }
    }

    #endregion

    #region Background & Theme

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
        if (imgBG == null) return;

        var model = DataSOController.Instance.GetModelByID(themeId);
        if (model != null && model.backgroundImage != null)
        {
            imgBG.sprite = model.backgroundImage;
            Debug.Log($"[GamePlayUI] Background changed to {themeId}");
        }
        else
        {
            Debug.LogWarning($"[GamePlayUI] Không tìm thấy model hoặc sprite cho themeId = {themeId}");
        }
    }

    #endregion
}
