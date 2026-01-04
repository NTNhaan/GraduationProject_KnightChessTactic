using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : Singleton<TimeController>
{
    public Slider TimeSliderDemon;
    public Slider TimeSliderHero;
    public float MaxTime = 100;
    public float timeScale = 1f;   // tg cho speedupState
    public float baseSpeed = 1f;
    public float currentSpeed;
    public Role role;
    public Animator animator;


    private bool isPaused = false;
    private bool hasPlayedWarning = false;
    private const float WARNING_THRESHOLD = 30f;
    private bool isGameStarted = false;
    // public float maxTimeScale = 3f; // Giới hạn tốc độ tối đa
    // public float minTimeScale = 0.5f;
    protected override void CustomAwake()
    {
        Debug.Log("TimeController Awake: " + this);
    }

    public void Start()
    {
        TimeSliderHero.value = MaxTime;
        TimeSliderDemon.value = MaxTime;
        Debug.Log("TimeController Start: " + this);
        currentSpeed = baseSpeed;
        role = Role.Player;
        // Đăng ký lắng nghe sự kiện board đã fill xong
        Grid.OnBoardFilled += StartGame;
    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi object bị destroy
        Grid.OnBoardFilled -= StartGame;
    }

    private void StartGame()
    {
        isGameStarted = true;
    }

    public void SwapRole()
    {
        // Không swap role nếu đang trong training mode
        if (GamePlayController.Instance != null && GamePlayController.Instance.IsTrainingMode)
        {
            return;
        }

        if (role == Role.Player)
        {
            isPaused = false;
            role = Role.Demon;
            TimeSliderHero.value = MaxTime;
            hasPlayedWarning = false;
        }
        else if (role == Role.Demon)
        {
            isPaused = false;
            role = Role.Player;
            TimeSliderDemon.value = MaxTime;
            hasPlayedWarning = false;
        }
    }

    public void ResetAnimation()
    {
        animator.ResetTrigger("StartTurn");
        animator.ResetTrigger("StartTurnBack");
    }

    public void PlayAnimation(string nametrigger)
    {
        animator.SetTrigger(nametrigger);
    }

    public void Update()
    {
        currentSpeed = baseSpeed * timeScale;
        // Chỉ cập nhật thời gian khi game đã bắt đầu
        if (!isGameStarted) return;

        bool SwapOnBoard = TurnController.Instance.IsSwapping;

        // Pause slider khi game bị pause hoặc đang swap
        if (SwapOnBoard || isPaused || InGameData.GAME_STATE == GameState.PauseGame)
        {
            return; // Don't update slider during swap animation or pause
        }

        if (role == Role.Player)
        {
            // Sử dụng timeScale để điều khiển tốc độ giảm của slider
            TimeSliderHero.value -= Time.deltaTime * 10 * timeScale;

            // Kiểm tra và phát âm thanh cảnh báo
            if (TimeSliderHero.value <= WARNING_THRESHOLD && !hasPlayedWarning)
            {
                hasPlayedWarning = true;
            }

            if (TimeSliderHero.value <= 0)
            {
                // Không start turn change nếu đang trong training mode
                if (GamePlayController.Instance == null || !GamePlayController.Instance.IsTrainingMode)
                {
                    // Start turn change with new animation effect
                    TurnController.Instance.StartTurnChange();
                }
            }
        }
        else if (role == Role.Demon)
        {
            // Sử dụng timeScale để điều khiển tốc độ giảm của slider
            TimeSliderDemon.value -= Time.deltaTime * 10 * timeScale;

            // Kiểm tra và phát âm thanh cảnh báo
            if (TimeSliderDemon.value <= WARNING_THRESHOLD && !hasPlayedWarning)
            {
                hasPlayedWarning = true;
            }

            if (TimeSliderDemon.value <= 0)
            {
                // Không start turn change nếu đang trong training mode
                if (GamePlayController.Instance == null || !GamePlayController.Instance.IsTrainingMode)
                {
                    // Start turn change with new animation effect
                    TurnController.Instance.StartTurnChange();
                }
            }
        }
    }

    /// <summary>
    /// Resume time slider after turn change animation completes
    /// </summary>
    public void ResumeTimeSlider()
    {
        isPaused = false;
        // Reset slider to max for the new role
        if (role == Role.Player)
        {
            TimeSliderHero.value = MaxTime;
        }
        else
        {
            TimeSliderDemon.value = MaxTime;
        }
        hasPlayedWarning = false;
    }
    public void SetTimeScale(float newScale)
    {
        timeScale = Mathf.Clamp(newScale, 0.5f, 3f); // Giới hạn tốc độ từ 0.5x đến 3x
        currentSpeed = baseSpeed * timeScale;
        Debug.Log($"Time scale set to: {timeScale}");
    }

    public void ResetTimeScale()
    {
        timeScale = 1f;
        currentSpeed = baseSpeed;
        Debug.Log("Time scale reset to normal");
    }
    public void Pause()
    {
        isPaused = true;
        TurnController.Instance.StartSwap();
    }
}

public enum Role
{
    Player,
    Demon
}