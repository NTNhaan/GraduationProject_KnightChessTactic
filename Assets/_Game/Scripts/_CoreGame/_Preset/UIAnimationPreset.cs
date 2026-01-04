using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Animation Preset")]
public class UIAnimationPreset : ScriptableObject
{
    public float duration = 0.3f;
    public Ease ease = Ease.OutBack;

    [Header("Scale")]
    public bool useScale = true;
    public Vector3 fromScale = Vector3.zero;
    public Vector3 toScale = Vector3.one;

    [Header("Move (AnchoredPosition)")]
    public bool useMove = false;
    public Vector2 fromAnchoredPos;
    public Vector2 toAnchoredPos;
}