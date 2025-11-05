using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PointValueTextEffect : MonoBehaviour
{
    void Start()
    {
        transform.DOMove(transform.position + Vector3.up * 1f, 0.8f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => Destroy(gameObject));
    }
}