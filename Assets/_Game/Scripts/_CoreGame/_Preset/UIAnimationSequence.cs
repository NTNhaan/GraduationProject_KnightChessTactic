using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationSequence : MonoBehaviour
{
    [SerializeField] private List<UIAnimationStep> steps;

    public async UniTask Play()
    {
        foreach (var step in steps)
        {
            await step.Play(); // ⬅️ CHỜ step trước xong
        }
    }
}