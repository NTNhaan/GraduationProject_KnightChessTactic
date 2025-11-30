using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Handles turn indicator UI with flip and fly animation effect
/// TurnSwap: Container for flip effect
/// TurnImage: Image that changes sprite
/// </summary>
public class TurnIndicator : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform turnSwap; // Container for flip effect
    [SerializeField] private Image turnImage; // Image that changes sprite
    [SerializeField] private Sprite playerTurnSprite;
    [SerializeField] private Sprite enemyTurnSprite;

    [Header("Animation Settings")]
    [SerializeField] private float flyUpDistance = 50f; // Bay lên một chút
    [SerializeField] private float flipDuration = 0.5f;
    [SerializeField] private float flyDuration = 0.3f;
    [SerializeField] private Ease flyEase = Ease.OutQuad;
    [SerializeField] private Ease flipEase = Ease.InOutQuad;

    private Vector3 originalPosition;
    private bool isAnimating = false;

    private void Awake()
    {
        // Find TurnSwap if not assigned
        if (turnSwap == null)
        {
            turnSwap = GetComponent<RectTransform>();
        }

        // Find TurnImage if not assigned
        if (turnImage == null)
        {
            turnImage = GetComponentInChildren<Image>();
        }

        if (turnSwap != null)
        {
            originalPosition = turnSwap.anchoredPosition;
        }
        else
        {
            Debug.LogError("[TurnIndicator] TurnSwap RectTransform not found!");
        }

        if (turnImage == null)
        {
            Debug.LogError("[TurnIndicator] TurnImage not found!");
        }
    }

    /// <summary>
    /// Show turn change animation: fly up, flip, change sprite, fly back down
    /// </summary>
    public void ShowTurnChange(Role newRole, System.Action onComplete = null)
    {
        if (isAnimating)
        {
            Debug.LogWarning("[TurnIndicator] Animation already in progress, skipping...");
            return;
        }

        StartCoroutine(TurnChangeAnimation(newRole, onComplete));
    }

    private IEnumerator TurnChangeAnimation(Role newRole, System.Action onComplete)
    {
        isAnimating = true;

        if (turnSwap == null)
        {
            Debug.LogError("[TurnIndicator] TurnSwap is null!");
            isAnimating = false;
            onComplete?.Invoke();
            yield break;
        }

        // Step 1: Fly up một chút
        Vector3 upPosition = originalPosition + Vector3.up * flyUpDistance;
        Tween flyUpTween = turnSwap.DOAnchorPos(upPosition, flyDuration)
            .SetEase(flyEase);

        yield return flyUpTween.WaitForCompletion();

        // Step 2: Flip TurnSwap (scale X to 0)
        Vector3 originalScale = turnSwap.localScale;
        Tween flipTween = turnSwap.DOScaleX(0f, flipDuration / 2f)
            .SetEase(flipEase);

        yield return flipTween.WaitForCompletion();

        // Step 3: Change sprite của TurnImage while scale X is 0
        ChangeTurnSprite(newRole);

        // Step 4: Flip back TurnSwap (scale X to 1)
        Tween flipBackTween = turnSwap.DOScaleX(originalScale.x, flipDuration / 2f)
            .SetEase(flipEase);

        yield return flipBackTween.WaitForCompletion();

        // Step 5: Fly back về vị trí ban đầu
        Tween flyBackTween = turnSwap.DOAnchorPos(originalPosition, flyDuration)
            .SetEase(flyEase);

        yield return flyBackTween.WaitForCompletion();

        isAnimating = false;

        // Callback when animation completes
        onComplete?.Invoke();
    }

    private void ChangeTurnSprite(Role role)
    {
        if (turnImage == null)
        {
            Debug.LogError("[TurnIndicator] Turn image is null!");
            return;
        }

        switch (role)
        {
            case Role.Player:
                if (playerTurnSprite != null)
                {
                    turnImage.sprite = playerTurnSprite;
                    Debug.Log("[TurnIndicator] Changed to Player Turn sprite");
                }
                break;
            case Role.Demon:
                if (enemyTurnSprite != null)
                {
                    turnImage.sprite = enemyTurnSprite;
                    Debug.Log("[TurnIndicator] Changed to Enemy Turn sprite");
                }
                break;
        }
    }

    /// <summary>
    /// Set initial turn sprite without animation
    /// </summary>
    public void SetInitialTurn(Role role)
    {
        ChangeTurnSprite(role);
    }
}

