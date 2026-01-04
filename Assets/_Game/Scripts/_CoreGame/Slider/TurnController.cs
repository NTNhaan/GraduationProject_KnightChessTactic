using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class TurnController : Singleton<TurnController>
{
    private bool isSwapping = false;
    public static event Action OnTranslationEnd;

    [Header("Turn Indicator")]
    [SerializeField] private TurnIndicator turnIndicator;

    public bool IsSwapping
    {
        get { return isSwapping; }
    }

    protected override void CustomAwake()
    {
        if (turnIndicator == null)
        {
            turnIndicator = FindFirstObjectByType<TurnIndicator>();
        }
    }

    public void StartSwap()
    {
        isSwapping = true;
    }

    /// <summary>
    /// Called when turn change animation completes
    /// </summary>
    public void OnTurnChangeComplete()
    {
        isSwapping = false;
        TimeController.Instance.SwapRole();
        TimeController.Instance.ResumeTimeSlider(); // Resume slider after animation
    }

    /// <summary>
    /// Start turn change with new flip animation effect
    /// </summary>
    public void StartTurnChange()
    {
        // Không start turn change nếu đang trong training mode
        if (GamePlayController.Instance != null && GamePlayController.Instance.IsTrainingMode)
        {
            return;
        }

        if (isSwapping)
        {
            Debug.LogWarning("[TurnController] Already swapping, skipping...");
            return;
        }

        StartSwap();

        // Get next role
        Role currentRole = TimeController.Instance.role;
        Role nextRole = currentRole == Role.Player ? Role.Demon : Role.Player;

        // Show turn change animation
        if (turnIndicator != null)
        {
            turnIndicator.ShowTurnChange(nextRole, OnTurnChangeComplete);
        }
        else
        {
            Debug.LogWarning("[TurnController] TurnIndicator not found! Calling OnTurnChangeComplete immediately");
            // If no indicator, just swap immediately
            OnTurnChangeComplete();
        }
    }

    // Legacy methods - kept for compatibility but will use new animation
    public void TranslationHero()
    {
        StartTurnChange();
    }

    public void TranslationDemon()
    {
        StartTurnChange();
    }

    // Called from animation events (if still using old animations)
    public void OnAnimationEnd()
    {
        OnTurnChangeComplete();
    }
}
