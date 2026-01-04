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
    [SerializeField] private Grid gameGrid;

    [Header("Training Mode")]
    [SerializeField] private bool isTrainingMode = false;

    public bool IsTrainingMode
    {
        get { return isTrainingMode; }
        set { isTrainingMode = value; }
    }

    // Track time slow effect
    private bool hasTimeSlowEffect = false;

    [Header("Combo System")]
    private float comboMultiplier = 1f;
    private float comboTimer = 0f;
    private const float COMBO_DURATION = 2f;
    private const float MAX_COMBO = 4f;
    private Coroutine comboCoroutine;

    [Header("Item Behaviors")]
    private Dictionary<ItemPieces.ItemType, System.Action<GamePieces>> itemBehaviors;

    // Track role để detect turn change
    private Role lastRole = Role.Player;

    void OnEnable()
    {
        EventManager.OnAddPoints += HandleAddPoints;
        EventManager.OnHPchanged += HandleHealthChanged;
        EventDispatcher.Register(EventId.OnGameStateChanged, OnGameStateChanged);
        // Lắng nghe khi turn thay đổi để xử lý Shield
        // Kiểm tra null trước khi truy cập Instance
        if (TimeController.Instance != null)
        {
            TimeController.Instance.role = Role.Player; // Đảm bảo role được khởi tạo
        }
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
    protected override void CustomAwake()
    {
        if (timeController == null)
            timeController = FindFirstObjectByType<TimeController>();

        if (enemy == null)
            enemy = FindFirstObjectByType<EnemyCharacter>();

        if (player == null)
            player = FindFirstObjectByType<HeroCharater>();

        if (gameGrid == null)
            gameGrid = FindFirstObjectByType<Grid>();

        comboMultiplier = 1f;
    }

    async UniTask Start()
    {
        Application.targetFrameRate = 60;
        InitializeItemBehaviors();
        comboCoroutine = StartCoroutine(ComboTimerCoroutine());

        // Khởi tạo lastRole
        if (timeController != null)
        {
            lastRole = timeController.role;
        }

        await UniTask.WaitUntil(() => InGameData.GAME_STATE == GameState.LoadingDone);
        EventDispatcher.Push(EventId.OnGamePlayScreen);

        ChangeState(GameState.PlayingGame);
    }

    void Update()
    {
        // Kiểm tra khi turn thay đổi để xử lý Shield và Time
        if (timeController != null && player != null && enemy != null)
        {
            Role currentRole = timeController.role;

            // Khi turn enemy xong (chuyển từ Demon về Player), tắt shield nếu có và áp dụng time slow
            if (lastRole == Role.Demon && currentRole == Role.Player)
            {
                // Tắt shield của player nếu có
                if (player.HasShield)
                {
                    player.DeactivateShield();
                }

                // Áp dụng time slow effect nếu có
                if (hasTimeSlowEffect)
                {
                    // Làm chậm time slider (giảm tốc độ giảm của slider)
                    timeController.SetTimeScale(0.5f); // Chậm lại 50%
                    Debug.Log("[GamePlayController] Time slow effect applied - time scale set to 0.5");
                    hasTimeSlowEffect = false; // Reset sau khi áp dụng
                }
            }
            // Khi turn player xong (chuyển từ Player về Demon), tắt shield nếu có
            else if (lastRole == Role.Player && currentRole == Role.Demon)
            {
                // Tắt shield của enemy nếu có
                if (enemy.HasShield)
                {
                    enemy.DeactivateShield();
                }

                // Reset time scale về bình thường khi đến lượt enemy
                timeController.ResetTimeScale();
            }

            lastRole = currentRole;
        }
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
                    player.RestoreHealth(2);
                }
                else
                {
                    enemy.RestoreHealth(2);
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
            {ItemPieces.ItemType.Fire, (GamePieces piece) => {
                if(timeController.role == Role.Player)
                {
                    enemy.ApplyBurnEffect();
                }
                else
                {
                    player.ApplyBurnEffect();
                }
            } },
            {ItemPieces.ItemType.Coin, (GamePieces piece) =>
            {
                CoinController.Instance.AddCoin(3);
            } },
            {ItemPieces.ItemType.Energy, (GamePieces piece) =>
            {
                EnergyController.Instance.AddEnergy(3);
            } },
            {ItemPieces.ItemType.Shield, (GamePieces piece) => {
                // Shield: đỡ turn tiếp theo, khi turn enemy xong thì đổi lại idle
                if(timeController.role == Role.Player)
                {
                    player.ActivateShield();
                }
                else
                {
                    enemy.ActivateShield();
                }
            } },
            {ItemPieces.ItemType.Armor, (GamePieces piece) => {
                // Armor: đỡ cho đến khi bị attack
                if(timeController.role == Role.Player)
                {
                    player.ActivateArmor();
                }
                else
                {
                    enemy.ActivateArmor();
                }
            } },
            {ItemPieces.ItemType.Time, (GamePieces piece) => {
                // Time: nếu player ăn được time thì tới lượt player tiếp theo time sẽ chậm lại
                if(timeController.role == Role.Player)
                {
                    hasTimeSlowEffect = true; // Đánh dấu để áp dụng khi đến lượt player tiếp theo
                    Debug.Log("[GamePlayController] Time item collected - will slow time on next player turn");
                }
            } },
            {ItemPieces.ItemType.Boom, (GamePieces piece) => {
                // Boom: clear hết hàng hoặc cột mà piece có
                if (gameGrid == null)
                {
                    gameGrid = FindFirstObjectByType<Grid>();
                }

                if (gameGrid != null && piece != null)
                {
                    // Lấy match để xác định ngang hay dọc
                    // Sử dụng GetMatch từ Grid để lấy match của piece này
                    List<GamePieces> match = gameGrid.GetMatch(piece, piece.X, piece.Y);

                    if (match != null && match.Count >= 3)
                    {
                        // Xác định match là ngang hay dọc
                        bool isHorizontal = gameGrid.IsMatchHorizontal(match);

                        if (isHorizontal)
                        {
                            // Clear hàng (tất cả pieces trong hàng đó)
                            gameGrid.ClearRow(piece.Y);
                        }
                        else
                        {
                            // Clear cột (tất cả pieces trong cột đó)
                            gameGrid.ClearColumn(piece.X);
                        }
                    }
                    else
                    {
                        // Nếu không tìm thấy match (có thể đã bị clear), 
                        // mặc định clear hàng hoặc cột dựa trên vị trí piece
                        // Ưu tiên clear hàng trước
                        gameGrid.ClearRow(piece.Y);
                    }
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
