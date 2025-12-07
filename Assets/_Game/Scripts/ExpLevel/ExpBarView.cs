using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExpBarView : MonoBehaviour
{
    [SerializeField] private Image imgExp;
    [SerializeField] private Text txtLvl;
    [SerializeField] private Text txtExp;
    
    public Transform tfmExpText => txtExp.transform;
    public void Init(int lvl, int exp, int maxExp)
    {
        Debug.Log($"CheckLevelmap {lvl} - {exp} - {maxExp}");
        float amount = exp * 1.0f / maxExp;
        SetFillAmount(amount);
        SetExp(exp, maxExp);
        SetLvl(lvl);
    }

    public void SetFillAmount(float amount)
    {
        imgExp.fillAmount = amount;
    }

    public void DoFillAmount(float amount, float duration, Ease ease = Ease.Linear, UnityAction onComplete = null)
    {
        imgExp.DOFillAmount(amount, 0.5f).SetEase(ease).OnComplete(() => onComplete?.Invoke());
    }

    public void SetExp(int exp, int maxExp)
    {
        txtExp.text = $"{exp}/" + $"{maxExp}";
    }

    public void SetLvl(int lvl)
    {
        txtLvl.text = $"{lvl}";
    }
}