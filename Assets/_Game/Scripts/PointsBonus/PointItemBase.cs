using Audio;
using DefaultNamespace;
using UnityEngine;
using TMPro;
using DG.Tweening;

public abstract class PointItemBase : MonoBehaviour
{
    [Header("Base Point Settings")] 
    [SerializeField] protected SpriteRenderer itemSprite;
    [SerializeField] protected SpriteRenderer shadowSprite;
    [SerializeField] protected int pointValue;
    [SerializeField] protected float disappearTime = 5f;
    [SerializeField] private TMP_Text floatingText;

    private float spawnTime;

    protected virtual void Start()
    {
        spawnTime = TimeManager.Instance.ElapsedTime;
    }

    protected virtual void Update()
    {
        if (TimeManager.Instance.ElapsedTime - spawnTime >= disappearTime)
            OnDisappear();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioController.Instance.PlayEffect(Sound.Name.Sound_PoinBonus);
            ScoreController.Instance.AddPoints(pointValue);
            itemSprite.enabled = false;
            shadowSprite.enabled = false;
            OnCollected();
        }
    }

    protected virtual void OnCollected()
    {
        ShowPointValueText();
        Destroy(gameObject, 1f); 
    }

    protected virtual void OnDisappear()
    {
        Destroy(gameObject);
    }

    void ShowPointValueText()
    {
        if (floatingText == null)
        {
            Debug.LogWarning($"[{name}] FloatingText reference missing!");
            return;
        }

        floatingText.text = $"+{pointValue}";
        floatingText.alpha = 1f;
        floatingText.gameObject.SetActive(true);

        floatingText.transform.DOKill();
        floatingText.transform.DOMoveY(floatingText.transform.position.y + 1f, 0.8f)
            .SetEase(Ease.OutQuad);
        floatingText.DOFade(0f, 0.8f)
            .SetEase(Ease.Linear)
            .OnComplete(() => floatingText.gameObject.SetActive(false));
    }
}