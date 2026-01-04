using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIAnimator : MonoBehaviour
{
    [SerializeField] private UIAnimationPreset showPreset;
    [SerializeField] private UIAnimationPreset hidePreset;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public UniTask PlayShow()
    {
        Sequence seq = DOTween.Sequence();

        if (showPreset.useScale)
        {
            rect.localScale = showPreset.fromScale;
            seq.Join(
                rect.DOScale(showPreset.toScale, showPreset.duration)
                    .SetEase(showPreset.ease)
            );
        }

        if (showPreset.useMove)
        {
            rect.anchoredPosition = showPreset.fromAnchoredPos;
            seq.Join(
                rect.DOAnchorPos(showPreset.toAnchoredPos, showPreset.duration)
                    .SetEase(showPreset.ease)
            );
        }

        return seq.AsyncWaitForCompletion().AsUniTask();
    }

    public UniTask PlayHide()
    {
        if (hidePreset == null)
            return UniTask.CompletedTask;

        Sequence seq = DOTween.Sequence();

        if (hidePreset.useScale)
        {
            seq.Join(
                rect.DOScale(hidePreset.toScale, hidePreset.duration)
                    .SetEase(hidePreset.ease)
            );
        }

        if (hidePreset.useMove)
        {
            seq.Join(
                rect.DOAnchorPos(hidePreset.toAnchoredPos, hidePreset.duration)
                    .SetEase(hidePreset.ease)
            );
        }

        return seq.AsyncWaitForCompletion().AsUniTask();
    }
}