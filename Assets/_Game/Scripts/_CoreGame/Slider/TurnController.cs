using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class TurnController : Singleton<TurnController>
{
    private bool isSwapping = false;
    public static event Action OnTranslationEnd;
    public bool IsSwapping
    {
        get { return isSwapping; }
    }
    public void StartSwap()
    {
        isSwapping = true;
    }
    public void OnAnimationEnd()
    {
        isSwapping = false;
        TimeController.Instance.SwapRole();
        TimeController.Instance.ResetAnimation();
    }
    public void TranslationHero()
    {
        LevelLoader.Instance.TranslationPlayerAnim();
    }
    public void TranslationDemon()
    {
        LevelLoader.Instance.TranslationDemonAnim();
    }
}
