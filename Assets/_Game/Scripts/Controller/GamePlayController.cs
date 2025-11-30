using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DefaultNamespace;
using UnityEngine.SceneManagement;
using Data;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Serialization;

/// <summary>
/// Main gameplay controller - manages game state, item behaviors, combo system, and score
/// Replaces the old GameManager
/// </summary>
public class GamePlayController : Singleton<GamePlayController>
{
    [Header("Game References")]
    [SerializeField] private EnemyCharacter enemy;
    [SerializeField] private HeroCharater player;
    [SerializeField] private TimeController timeController;

    [Header("Combo System")]
    private float comboMultiplier = 1f;
    private float comboTimer = 0f;
    private const float COMBO_DURATION = 2f;
    private const float MAX_COMBO = 4f;
    private Coroutine comboCoroutine;

    [Header("Item Behaviors")]
    private Dictionary<ItemPieces.ItemType, System.Action<GamePieces>> itemBehaviors;

    void OnEnable()
    {
        EventManager.OnAddPoints += HandleAddPoints;
        EventManager.OnHPchanged += HandleHealthChanged;
        EventDispatcher.Register(EventId.OnGameStateChanged, OnGameStateChanged);
    }

    void OnDisable()
    {
        EventManager.OnAddPoints -= HandleAddPoints;
        EventManager.OnHPchanged -= HandleHealthChanged;
        EventDispatcher.RemoveCallback(EventId.OnGameStateChanged, OnGameStateChanged);
    }
    void OnDestroy()
    {
        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
        }
    }
    void CustomAwake()
    {
        if (timeController == null)
            timeController = FindFirstObjectByType<TimeController>();

        if (enemy == null)
            enemy = FindFirstObjectByType<EnemyCharacter>();

        if (player == null)
            player = FindFirstObjectByType<HeroCharater>();
        comboMultiplier = 1f;
    }

    async UniTask Start()
    {
        Application.targetFrameRate = 60;
        InitializeItemBehaviors();
        comboCoroutine = StartCoroutine(ComboTimerCoroutine());

        await UniTask.WaitUntil(() => InGameData.GAME_STATE == GameState.LoadingDone);
        EventDispatcher.Push(EventId.OnGamePlayScreen);

        ChangeState(GameState.PlayingGame);
    }

    #region Game State Management

    public void ChangeState(GameState newState)
    {
        InGameData.GAME_STATE = newState;
        switch (InGameData.GAME_STATE)
        {
            case GameState.PlayingGame:
                InGameData.GAME_STATE = GameState.PlayingGame;
                UITopController.Instance.ShowTab();
                break;
            case GameState.GameOver:
                // TutorialPanel.Instance.HideTutorial();
                if (DBController.Instance.TUTORIAL_COMPLETED)
                {
                    if (InGameData.GIVE_UP_COUNT != 0)
                    {
                        PopupController.Instance.ClickShowLosePopUp();
                    }
                    else
                    {
                        PopupController.Instance.ClickShowGiveUpPopUp();
                    }
                }
                else
                {
                    PopupController.Instance.ClickShowLosePopUp();
                }
                break;
        }
    }

    public void OnGameStateChanged(object data = null)
    {
        var newState = InGameData.GAME_STATE;
        switch (newState)
        {
            case GameState.PauseGame:
                if (InGameData.PRE_STATE == GameState.UseBooster)
                {
                    BoosterController.Instance.PauseBooster();
                }
                AudioController.Instance.StopEffect(Sound.Name.Sound_Clock);
                AudioController.Instance.StopEffect(Sound.Name.Sound_Clock_Stuck);
                break;
            case GameState.PlayingGame:
                if (InGameData.PRE_STATE == GameState.UseBooster)
                {
                    InGameData.GAME_STATE = GameState.UseBooster;
                    BoosterController.Instance.ContinueBooster();
                    AudioController.Instance.PlayEffect(Sound.Name.Sound_Clock_Stuck);
                }
                else
                {
                    AudioController.Instance.PlayEffect(Sound.Name.Sound_Clock);
                }
                break;
            case GameState.GameOver:
                BoosterController.Instance.CancelBooster();
                AudioController.Instance.StopEffect(Sound.Name.Sound_Clock);
                AudioController.Instance.StopEffect(Sound.Name.Sound_Clock_Stuck);
                break;
            case GameState.UseBooster:
                AudioController.Instance.PlayEffect(Sound.Name.Sound_Clock_Stuck);
                break;
            case GameState.GiveUp:
                AudioController.Instance.StopEffect(Sound.Name.Sound_Clock);
                AudioController.Instance.StopEffect(Sound.Name.Sound_Clock_Stuck);
                break;
        }
    }

    #endregion

    #region Health Management

    public void HandleHealthChanged(int damage)
    {
        ScoreController.Instance.TakeDamage(damage);

        if (ScoreController.Instance.GetHealthPlayer() <= 0)
        {
            ChangeState(GameState.GameOver);
        }
    }

    #endregion

    #region Score & Combo System

    public float GetComboMultiplier()
    {
        return comboMultiplier;
    }

    private void HandleAddPoints(int points)
    {
        // Apply combo multiplier to score
        int finalScore = Mathf.RoundToInt(points * comboMultiplier);
        ScoreController.Instance.AddPoints(finalScore);

        // Increase combo multiplier
        comboMultiplier = Mathf.Min(comboMultiplier + 0.5f, MAX_COMBO);
        comboTimer = COMBO_DURATION;
    }

    private IEnumerator ComboTimerCoroutine()
    {
        while (true)
        {
            if (comboTimer > 0)
            {
                comboTimer -= 0.1f; // Update every 0.1s instead of every frame
                if (comboTimer <= 0)
                {
                    comboMultiplier = 1f;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    #endregion

    #region Item Behaviors

    void InitializeItemBehaviors()
    {
        // Validate references
        if (enemy == null || player == null || timeController == null)
        {
            Debug.LogError("[GamePlayController] Các tham chiếu quan trọng chưa được khởi tạo!");
            return;
        }

        itemBehaviors = new Dictionary<ItemPieces.ItemType, System.Action<GamePieces>>
        {
            {ItemPieces.ItemType.Sword, (GamePieces piece) => {
                if(timeController.role == Role.Player)
                {
                    player.PerformAttack(enemy);
                }
                else
                {
                    enemy.PerformAttack(player);
                }
            } },
            {ItemPieces.ItemType.Apple, (GamePieces piece) => {
                if(timeController.role == Role.Player)
                {
                    player.RestoreHealth(5);
                }
                else
                {
                    enemy.RestoreHealth(5);
                }
            } },
            {ItemPieces.ItemType.Heart, (GamePieces piece) => {
                if(timeController.role == Role.Player)
                {
                    player.RestoreHealth(player.maxHealth);
                }
                else
                {
                    enemy.RestoreHealth(enemy.maxHealth);
                }
            } },
            {ItemPieces.ItemType.AppleGreen, (GamePieces piece) => {
                if(timeController.role == Role.Player)
                {
                    enemy.ApplyBurnEffect();
                }
                else
                {
                    player.ApplyBurnEffect();
                }
            } },
        };
    }

    /// <summary>
    /// Handle item behavior when a piece is matched
    /// Called from Grid.cs when clearing matches
    /// </summary>
    public void HandleItemBehaviour(GamePieces piece)
    {
        // Validate piece
        if (piece == null || piece.ItemComponent == null)
        {
            Debug.LogError("[GamePlayController] GamePieces hoặc ItemComponent là null!");
            return;
        }

        // Execute item behavior
        if (itemBehaviors != null && itemBehaviors.ContainsKey(piece.ItemComponent.Item))
        {
            itemBehaviors[piece.ItemComponent.Item].Invoke(piece);
        }
        else
        {
            Debug.LogWarning($"[GamePlayController] No item behavior found for {piece.ItemComponent.Item}");
        }
    }

    #endregion

    #region UI Methods

    public void OnClickShowPausePopup()
    {
        PopupController.Instance.ClickShowPausePopUp();
    }

    public void OnClickShowHowToPlayPopup()
    {
        PopupController.Instance.ClickShowHowToPlayPopUp();
    }

    #endregion
}
