using UnityEngine;
using UnityEngine.UI;

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

    public void Start()
    {
        animator = GetComponent<Animator>();
        health = maxHealth;

        // Khởi tạo state mặc định
        if (currentState == null)
        {
            ChangeState(new IdleState());
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
