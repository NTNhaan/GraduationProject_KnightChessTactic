using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using UnityEngine;

public class ExpBarController : Singleton<ExpBarController>
{
    [SerializeField] private ExpBarView expBarView;
    [SerializeField] ExpLevelData expLevelData;
    private DBController dbCtrl;
    private ExpData currentData;
    bool isAnimatingExpBar = false;

#if UNITY_EDITOR
    [ContextMenu("Init")]
#endif
    public void Init()
    {
        dbCtrl = DBController.Instance;
        currentData = GetExpDataClampLevel();
        expBarView.Init(currentData.lvl, dbCtrl.CURRENT_EXP - currentData.minExp, currentData.maxExp - currentData.minExp);
        Debug.Log($"updateExpBar {currentData.lvl} - {dbCtrl.CURRENT_EXP} - {currentData.minExp} - {currentData.maxExp}");
    }
    public void UpdateExpPercent(int exp)
    {
        Debug.Log("UpdateExpBar");
        AddExp(exp);
    }
    public void UpdateExpPopup(int exp)
    {
        dbCtrl.CURRENT_EXP += exp;
        float amount = (dbCtrl.CURRENT_EXP - this.currentData.minExp) * 1.0f /
                       (currentData.maxExp - currentData.minExp);
        StartCoroutine(DoIncreaseExp());
    }
    ExpData GetExpDataClampLevel()
    {
        int lvl = dbCtrl.EXP_LVL;
        if (lvl > expLevelData.expDataArray.Length)
        {
            lvl = expLevelData.expDataArray.Length;
        }
        var expData = expLevelData.expDataArray[lvl - 1];
        return expData;
    }
    public void AddExp(int exp)
    {
        Debug.Log($"ADD EXP=============<> {exp}");
        dbCtrl.CURRENT_EXP += exp;
        float amount = (dbCtrl.CURRENT_EXP - this.currentData.minExp) * 1.0f /
                       (currentData.maxExp - currentData.minExp);
        if (amount >= 1)
        {
            dbCtrl.EXP_LVL++;
            if (isAnimatingExpBar)
            {
                return;
            }

            isAnimatingExpBar = true;
        }
        StartCoroutine(DoIncreaseExp());
    }

#if UNITY_EDITOR
    [ContextMenu("Add Exp 500")]
    void AutoAdd500Exp()
    {
        int exp = (currentData.maxExp - currentData.minExp) / 2;
        AddExp(exp + 500);
    }
#endif

    IEnumerator DoIncreaseExp()
    {
        Debug.Log($"[CheckEXP]: AddEXP {currentData.lvl} {dbCtrl.EXP_LVL}");
        if (currentData.lvl < dbCtrl.EXP_LVL)
        {
            expBarView.SetExp(currentData.maxExp - currentData.minExp, currentData.maxExp - currentData.minExp);
            expBarView.DoFillAmount(1, 0.3f, Ease.Linear, () =>
            {
                // do show win popup
                // PopupController.Instance.ClickShowLevelUpPopUp();
                expBarView.DoFillAmount(0, 0.2f, Ease.Linear, () => isAnimatingExpBar = false);
            });

            yield return null;
        }
        yield return new WaitUntil(() => !isAnimatingExpBar);

        currentData = GetExpDataClampLevel();
        expBarView.SetExp(dbCtrl.CURRENT_EXP - currentData.minExp, currentData.maxExp - currentData.minExp);
        float amount = (dbCtrl.CURRENT_EXP - this.currentData.minExp) * 1.0f /
                       (currentData.maxExp - currentData.minExp);
        expBarView.DoFillAmount(amount, 0.3f);
        expBarView.SetLvl(currentData.lvl);
    }
}