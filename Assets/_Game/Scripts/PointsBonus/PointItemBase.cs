using Audio;
using DefaultNamespace;
using UnityEngine;
using TMPro;
using DG.Tweening;

public abstract class PointItemBase : MonoBehaviour
{
    [Header("Base Point Settings")]
    [SerializeField] protected int pointValue;         
    [SerializeField] protected float disappearTime = 5f; 

    [SerializeField] private TMP_FontAsset textMeshPro;
    [Header("TextEffect")]
    // [SerializeField] private TMP_FontAsset fontTextPro;
    // [SerializeField] private GameObject pointTextPrefab;
    // [SerializeField] private Transform uiParent;
    
    private float spawnTime;

    protected virtual void Start()
    {
        // spawnTime = Time.time;
        spawnTime = TimeManager.Instance.ElapsedTime;
        DOTween.Init();
    }

    protected virtual void Update()
    {
        if (TimeManager.Instance.ElapsedTime - spawnTime >= disappearTime)
        {
            OnDisappear();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"CheckColioderEnter 2");
        if (other.CompareTag("Player"))
        {
            OnCollected();
        }
    }

    protected virtual void OnCollected()
    {
        Debug.Log($"{name} collected! +{pointValue} points!");
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PoinBonus);
        ScoreController.Instance.AddPoints(pointValue);
        ShowPointValueText();
        Destroy(gameObject);
    }

    protected virtual void OnDisappear()
    {
        Debug.Log($"{name} disappeared!");
        Destroy(gameObject);
    }

    void ShowPointValueText()
    {
        var go = new GameObject("TextObstacle");
        go.transform.position = transform.position + Vector3.up * 0.5f;
        var text = go.AddComponent<TMPro.TextMeshPro>();
        text.text = $"+{pointValue}";
        text.fontSize = 3f;
        text.font = textMeshPro;
        text.color = Color.white;
        text.alignment = TMPro.TextAlignmentOptions.Center;
        text.sortingOrder = 100;
        text.gameObject.SetActive(true);
        text.outlineWidth = 0.2f;
        text.outlineColor = Color.black;
        go.AddComponent<PointValueTextEffect>();
    }
    // protected void ShowPointValueText(int value)
    // {
    //     if (pointTextPrefab == null)
    //     {
    //         Debug.LogError("⚠️ Missing pointTextPrefab!");
    //         return;
    //     }
    //     var go = Instantiate(pointTextPrefab, transform.position, Quaternion.identity, uiParent);
    //
    //     var tmp = go.GetComponent<TextMeshPro>();
    //     if (tmp == null)
    //     {
    //         Debug.LogError("⚠️ Prefab không có TextMeshPro (3D)!");
    //         return;
    //     }
    //
    //     tmp.text = "+" + value;
    //     tmp.font = fontTextPro;
    //
    //     var effect = go.GetComponent<PointValueTextEffect>();
    //     if (effect != null)
    //         effect.Play(() => { /* callback sau khi bay xong */ });
    // }
}