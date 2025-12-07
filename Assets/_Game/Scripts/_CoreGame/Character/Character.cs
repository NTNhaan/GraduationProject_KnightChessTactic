using UnityEngine;
using UnityEngine.UI;
using Data;

public abstract class Character : MonoBehaviour
{
    public float attack;
    public float health;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image fontHealthBar;
    public Image backHealthBar;
    [HideInInspector]
    public float lerpTimer;

    protected Animator animator;
    protected ICharacterState currentState;

    // Shield và Armor tracking
    private bool hasShield = false;
    private bool hasArmor = false;

    public bool HasShield
    {
        get { return hasShield; }
        set { hasShield = value; }
    }

    public bool HasArmor
    {
        get { return hasArmor; }
        set { hasArmor = value; }
    }

    public bool IsBlocking
    {
        get { return currentState is BlockState || hasShield || hasArmor; }
    }

    public void Start()
    {
        animator = GetComponent<Animator>();
        health = maxHealth;

        // Khởi tạo state mặc định
        if (currentState == null)
        {
            ChangeState(new IdleState());
        }

        // Lắng nghe sự kiện game state change để pause/resume
        EventDispatcher.Register(EventId.OnGameStateChanged, OnGameStateChanged);
    }

    private void OnDestroy()
    {
        EventDispatcher.RemoveCallback(EventId.OnGameStateChanged, OnGameStateChanged);
    }

    private void OnGameStateChanged(object data = null)
    {
        if (animator == null) return;

        // Pause animator khi game bị pause
        if (InGameData.GAME_STATE == GameState.PauseGame)
        {
            animator.speed = 0f; // Pause animation
        }
        else
        {
            animator.speed = 1f; // Resume animation
        }
    }

    // OPTIMIZED: Reduce Update frequency for health UI
    private float healthUIUpdateInterval = 0.05f; // Update every 0.05s instead of every frame
    private float healthUIUpdateTimer = 0f;

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update(this);
        }

        // OPTIMIZED: Update health UI less frequently
        healthUIUpdateTimer += Time.deltaTime;
        if (healthUIUpdateTimer >= healthUIUpdateInterval)
        {
            UpdateHealthUI();
            healthUIUpdateTimer = 0f;
        }
    }

    public void ChangeState(ICharacterState newState)
    {
        if (currentState != null)
            currentState.Exit(this);

        currentState = newState;
        currentState.Enter(this);
    }

    // OPTIMIZED: Cache health fraction calculation and reduce redundant operations
    private float lastHealthFraction = -1f;

    public void UpdateHealthUI()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        float hFraction = health / maxHealth;

        // OPTIMIZED: Skip update if health hasn't changed significantly
        if (Mathf.Abs(hFraction - lastHealthFraction) < 0.001f && lerpTimer == 0f)
            return;

        lastHealthFraction = hFraction;

        // OPTIMIZED: Cache component references
        if (fontHealthBar == null || backHealthBar == null) return;

        float fillFont = fontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;

        if (fillBack > hFraction)
        {
            fontHealthBar.fillAmount = hFraction;
            // backHealthBar.color = Color.red;
            lerpTimer += healthUIUpdateInterval; // Use update interval instead of deltaTime
            float percentComplete = Mathf.Pow(lerpTimer / chipSpeed, 2);
            backHealthBar.fillAmount = Mathf.Lerp(fillBack, hFraction, percentComplete);
        }
        else
        {
            backHealthBar.fillAmount = hFraction;
            // backHealthBar.color = Color.green;
            lerpTimer += healthUIUpdateInterval;
            float percentComplete = Mathf.Pow(lerpTimer / chipSpeed, 2);
            fontHealthBar.fillAmount = Mathf.Lerp(fillFont, hFraction, percentComplete);
        }
    }

    public virtual void PerformAttack(Character target)
    {
        ChangeState(new AttackState(target));
    }

    public virtual void ReceiveDamage(float damage)
    {
        // Kiểm tra nếu đang block (shield hoặc armor)
        if (IsBlocking)
        {
            // Block 100% damage - không nhận sát thương và không trừ máu
            // Nếu có armor, tắt armor sau khi bị attack
            if (hasArmor)
            {
                hasArmor = false;
                // Nếu không có shield, đổi về idle
                if (!hasShield)
                {
                    ChangeState(new IdleState());
                }
            }
            // Nếu chỉ có shield, shield vẫn giữ nguyên (sẽ tắt khi turn enemy xong)
            return; // Không nhận damage, không trừ máu
        }

        // Nếu không block, nhận damage bình thường
        health -= damage;
        lerpTimer = 0f;

        if (health <= 0)
        {
            ChangeState(new DeadState());
        }
        else
        {
            // Không thay đổi state nếu đang trong trạng thái thiêu đốt
            if (!(currentState is BurnState))
            {
                ChangeState(new HurtState());
            }
        }
    }
    public void ApplyBurnEffect()
    {
        if (!(currentState is BurnState))
        {
            ChangeState(new BurnState());
        }
    }
    // public void ApplySpeedUpEffect()
    // {
    //     Debug.Log($"Applying speed up effect to {gameObject.name}");
    //     // Chỉ áp dụng hiệu ứng tăng tốc nếu không đang trong trạng thái đó
    //     if (!(currentState is SpeedUpState))
    //     {
    //         ChangeState(new SpeedUpState());
    //     }
    //     else
    //     {
    //         Debug.Log($"{gameObject.name} is already in speed up state");
    //     }
    // }
    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

    /// <summary>
    /// Kích hoạt shield - đỡ turn tiếp theo
    /// </summary>
    public void ActivateShield()
    {
        hasShield = true;
        ChangeState(new BlockState());
    }

    /// <summary>
    /// Tắt shield - được gọi khi turn enemy xong
    /// </summary>
    public void DeactivateShield()
    {
        hasShield = false;
        // Nếu không có armor, đổi về idle
        if (!hasArmor)
        {
            ChangeState(new IdleState());
        }
    }

    /// <summary>
    /// Kích hoạt armor - đỡ cho đến khi bị attack
    /// </summary>
    public void ActivateArmor()
    {
        hasArmor = true;
        ChangeState(new BlockState());
    }
}


// public abstract class Character : MonoBehaviour
// {
//     public float attack;
//     public float health;
//     [HideInInspector] public float lerpTimer;
//     public float maxHealth = 100f;
//     public float chipSpeed = 2f;
//     public Image fontHealthBar;
//     public Image backHealthBar;
//     // function for state
//     public abstract void Attack(Character target);
//     public abstract void TakeHit(float damage);
//     public abstract void Dead();
//     public abstract void RestoreHealth(float mount);
//     public void Start()
//     {
//         health = maxHealth;
//     }
//     public void Update()
//     {
//         health = Mathf.Clamp(health, 0, maxHealth);
//         float fillFont = fontHealthBar.fillAmount;
//         float fillBack = backHealthBar.fillAmount;
//         float hFraction = health / maxHealth;
//         if (fillBack > hFraction)
//         {
//             fontHealthBar.fillAmount = hFraction;
//             backHealthBar.color = Color.red;
//             lerpTimer += Time.deltaTime;
//             float percentComplete = lerpTimer / chipSpeed;
//             percentComplete = percentComplete * percentComplete;
//             backHealthBar.fillAmount = Mathf.Lerp(fillBack, hFraction, percentComplete);
//         }
//         else
//         {
//             backHealthBar.fillAmount = hFraction;
//             backHealthBar.color = Color.green;
//             lerpTimer += Time.deltaTime;
//             float percentComplete = lerpTimer / chipSpeed;
//             percentComplete = percentComplete * percentComplete;
//             fontHealthBar.fillAmount = Mathf.Lerp(fillFont, hFraction, percentComplete);
//         }
//     }
// }
