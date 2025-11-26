using System;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TabItem : MonoBehaviour
{
    [SerializeField] private TabType tab;
    public TabType Tab => tab;

    [SerializeField] private Button button;
    [SerializeField] private Transform icon;
    [SerializeField] private Image bgSelected;
    [SerializeField] private Text label;

    [Header("Animation")]
    [SerializeField] private float activeScale = 1.2f;
    [SerializeField] private float moveUpOffset = 70f;
    [SerializeField] private float animDuration = 0.2f;

    private Vector3 iconDefaultLocalPos;
    private Tween iconTween;

    private void Awake()
    {
        if (icon != null)
            iconDefaultLocalPos = icon.localPosition;
    }

    public void Init(Action<TabType> callback)
    {
        button.onClick.AddListener(() => callback.Invoke(tab));
    }

    public void SetActiveVisual(bool active)
    {
        iconTween?.Kill();

        float targetScale = active ? activeScale : 1f;
        Vector3 targetPos = iconDefaultLocalPos + (active ? Vector3.up * moveUpOffset : Vector3.zero);

        iconTween = DOTween.Sequence()
            .Join(icon.DOScale(targetScale, animDuration).SetEase(Ease.OutBack))
            .Join(icon.DOLocalMove(targetPos, animDuration).SetEase(Ease.OutQuad));

        bgSelected.gameObject.SetActive(active);
        label.gameObject.SetActive(active);
    }
}