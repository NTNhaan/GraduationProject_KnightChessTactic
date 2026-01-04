using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIAnimationStep
{
    public string stepName;
    public List<UIAnimator> animators;

    public async UniTask Play()
    {
        var tasks = new List<UniTask>();
        foreach (var anim in animators)
            tasks.Add(anim.PlayShow());

        await UniTask.WhenAll(tasks);
    }
}