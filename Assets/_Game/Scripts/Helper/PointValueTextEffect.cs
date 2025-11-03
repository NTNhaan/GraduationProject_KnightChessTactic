using System;
using DG.Tweening;
using UnityEngine;

public class PointValueTextEffect : MonoBehaviour
{
    void Start()
    {
        transform.DOMove(transform.position + Vector3.up * 1f, 0.8f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => Destroy(gameObject));
    }
    // public void Play(Action onComplete)
    // {
    //     Debug.Log("PlayEffectText");
    //     transform.DOMove(transform.position + Vector3.up * 5f, 0.6f)
    //         .SetEase(Ease.OutQuad)
    //         .OnComplete(() =>
    //         {
    //             onComplete?.Invoke();
    //             Destroy(gameObject);
    //         });
    // }
}