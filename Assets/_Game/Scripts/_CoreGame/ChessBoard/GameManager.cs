using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Dictionary<ItemPieces.ItemType, System.Action<GamePieces>> itemBehaviors;
    [SerializeField] private EnemyCharacter enemy;
    [SerializeField] private HeroCharater player;
    [SerializeField] private TimeController timeswap;

    // Score and combo system
    private int currentScore = 0;
    private float comboMultiplier = 1f;
    private float comboTimer = 0f;
    private const float COMBO_DURATION = 2f;
    private const float MAX_COMBO = 4f;
    public int RemainingMoves { get; private set; }

    public float GetComboMultiplier()
    {
        return comboMultiplier;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void AddScore(int points)
    {
        currentScore += Mathf.RoundToInt(points * comboMultiplier);
        comboMultiplier = Mathf.Min(comboMultiplier + 0.5f, MAX_COMBO);
        comboTimer = COMBO_DURATION;
    }

    void Awake()
    {
        // OPTIMIZED: Use FindFirstObjectByType instead of deprecated FindObjectOfType
        if (timeswap == null)
            timeswap = FindFirstObjectByType<TimeController>();

        if (enemy == null)
            enemy = FindFirstObjectByType<EnemyCharacter>();

        if (player == null)
            player = FindFirstObjectByType<HeroCharater>();

        comboMultiplier = 1f;
        currentScore = 0;
    }

    // OPTIMIZED: Removed Update() loop - using coroutine instead for better performance
    private Coroutine comboCoroutine;

    void Start()
    {
        // Start combo timer coroutine
        InitializeItemBehaviors();
        comboCoroutine = StartCoroutine(ComboTimerCoroutine());
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

    void OnDestroy()
    {
        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
        }
    }

    void InitializeItemBehaviors()
    {
        // Kiểm tra xem các tham chiếu đã được khởi tạo chưa
        if (enemy == null || player == null || timeswap == null)
        {
            Debug.LogError("GameManager: Các tham chiếu quan trọng chưa được khởi tạo!");
            return;
        }

        itemBehaviors = new Dictionary<ItemPieces.ItemType, System.Action<GamePieces>>
        {
            {ItemPieces.ItemType.Sword, (GamePieces piece) => {
                if(timeswap.role == Role.Player)
                {
                    player.PerformAttack(enemy);
                }
                else
                {
                    enemy.PerformAttack(player);
                }
            } },
            {ItemPieces.ItemType.Apple, (GamePieces piece) => {
                if(timeswap.role == Role.Player)
                {
                    player.RestoreHealth(5);
                }
                else
                {
                    enemy.RestoreHealth(5);
                }
            } },
            {ItemPieces.ItemType.Heart, (GamePieces piece) => {
                if(timeswap.role == Role.Player)
                {
                    player.RestoreHealth(player.maxHealth);
                }
                else
                {
                    enemy.RestoreHealth(enemy.maxHealth);
                }
            } },
            {ItemPieces.ItemType.AppleGreen, (GamePieces piece) => {
                if(timeswap.role == Role.Player)
                {
                    enemy.ApplyBurnEffect();
                }
                else
                {
                    player.ApplyBurnEffect();
                }
            } },
            // {ItemPieces.ItemType.Beer, (GamePieces piece) => {
            //     Debug.Log("Beer item activated");
            //     if(timeswap.role == TimeBar.Role.Player)
            //     {
            //         Debug.Log("Applying speed up to enemy");
            //         enemy.ApplySpeedUpEffect();
            //     }
            //     else
            //     {
            //         Debug.Log("Applying speed up to player");
            //         player.ApplySpeedUpEffect();
            //     }
            // } }
        };
    }

    public void HandleItemBehaviour(GamePieces piece)
    {
        // Kiểm tra null trước khi sử dụng
        if (piece == null || piece.ItemComponent == null)
        {
            Debug.LogError("GameManager: GamePieces hoặc ItemComponent là null!");
            return;
        }

        // OPTIMIZED: Removed duplicate switch statement - Dictionary already handles this
        if (itemBehaviors != null && itemBehaviors.ContainsKey(piece.ItemComponent.Item))
        {
            itemBehaviors[piece.ItemComponent.Item].Invoke(piece);
        }
        else
        {
            Debug.LogWarning($"No item behavior found for {piece.ItemComponent.Item}");
        }
    }
}
