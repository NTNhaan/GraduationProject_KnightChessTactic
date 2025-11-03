using System;
using System.Threading;
using Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Data;
using UnityEngine.UI;

public class BoosterController : Singleton<BoosterController>
{
    [SerializeField] private Image boosterBtn;
    [SerializeField] private Sprite boosterActive;
    [SerializeField] private Sprite boosterDefault;
    [SerializeField] private Sprite boosterDontUse;
    private bool isBoosterActive = false;
    private bool isBoosterPaused = false;

    private float boosterDuration = 5f;
    private float remainingTime = 0f;
    private float boosterStartTime;

    private CancellationTokenSource boosterCTS;

    private void Start()
    {
        boosterDuration = GameConfig.TIME_USE_BOOSTER;
        var coinUser = DBController.Instance.COIN;
        if (coinUser < GameConfig.COIN_BOOSTER)
        {
            boosterBtn.sprite = boosterDontUse;
        }
        else
        {
            boosterBtn.sprite = boosterDefault;
        }
    }

    public void OnClickUseBooster()
    {
        if (isBoosterActive) return;
        if (DBController.Instance.GUIDE_BOOSTER == 0)
        {
            DBController.Instance.GUIDE_BOOSTER = 1;
            UseTimeStopBooster(boosterDuration);
        }
        else
        {
            if (DBController.Instance.COIN < GameConfig.COIN_BOOSTER)
            {
                PopupController.Instance.SetTextNotify("You don't have enough coins to use booster");
                PopupController.Instance.ShowNotifyPopUp();
            }
            else
            {
                CoinController.Instance.SpendCoin(GameConfig.COIN_BOOSTER);
                UseTimeStopBooster(boosterDuration);
            }
        }
    }
    public async void UseTimeStopBooster(float duration)
    {
        if (isBoosterActive) return;
        isBoosterActive = true;
        boosterBtn.sprite = boosterActive;
        remainingTime = duration;
        boosterStartTime = TimeManager.Instance.ElapsedTime;
        boosterCTS = new CancellationTokenSource();

        InGameData.GAME_STATE = GameState.UseBooster;
        // ClockController.Instance.PauseRotate();
        AudioController.Instance.PlayEffect(Sound.Name.Sound_Clock_Stuck);

        Debug.Log($"[Booster] Time Stop activated! Duration = {duration:F2}s");

        // try
        // {
        //     await ClockController.Instance.ShakeDuringFreeze(remainingTime);
        // }
        // catch (OperationCanceledException)
        // {
        //     Debug.Log("[Booster] Booster paused or cancelled.");
        //     return;
        // }

        FinishBooster();
    }
    public void PauseBooster()
    {
        if (!isBoosterActive || isBoosterPaused) return;

        float elapsed = TimeManager.Instance.ElapsedTime - boosterStartTime;
        remainingTime = Mathf.Max(0, remainingTime - elapsed);
        isBoosterPaused = true;

        Debug.Log($"[Booster] Paused — Remaining time: {remainingTime:F2}s");

        boosterCTS?.Cancel();
        // ClockController.Instance.PauseRotate();
        AudioController.Instance.StopEffect(Sound.Name.Sound_Clock_Stuck);
    }

    public void ContinueBooster()
    {
        if (!isBoosterPaused || remainingTime <= 0f) return;

        Debug.Log($"[Booster] Continue booster ({remainingTime:F2}s left)");
        UseTimeStopBooster(remainingTime);
    }

    public void CancelBooster()
    {
        if (!isBoosterActive) return;

        Debug.Log("[Booster] Cancelled");
        boosterCTS?.Cancel();
        ResetBooster();
    }
    private void FinishBooster()
    {
        Debug.Log("[Booster] Finished");
        ResetBooster();
    }

    private void ResetBooster()
    {
        isBoosterActive = false;
        isBoosterPaused = false;
        remainingTime = 0f;

        boosterBtn.sprite = boosterDefault;
        InGameData.GAME_STATE = GameState.PlayingGame;

        // ClockController.Instance.ResumeRotate();
        AudioController.Instance.StopEffect(Sound.Name.Sound_Clock_Stuck);
    }
}