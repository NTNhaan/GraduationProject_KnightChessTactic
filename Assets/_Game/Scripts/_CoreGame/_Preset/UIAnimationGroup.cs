using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationGroup : MonoBehaviour
{
    [SerializeField] private List<UIAnimator> animators;

    public async UniTask PlayShowAll()
    {
        var tasks = new List<UniTask>();
        foreach (var anim in animators)
            tasks.Add(anim.PlayShow());

        await UniTask.WhenAll(tasks);
    }

    public async UniTask PlayHideAll()
    {
        var tasks = new List<UniTask>();
        foreach (var anim in animators)
            tasks.Add(anim.PlayHide());

        await UniTask.WhenAll(tasks);
    }
}