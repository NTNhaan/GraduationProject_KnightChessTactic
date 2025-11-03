using UnityEngine;
using UnityEngine.Events;

public class ShopIAPScreen : ScreenBase
{
    #region Override Methods
    public override void ShowScreen(UnityAction onComplete)
    {
        base.ShowScreen(onComplete);
    }

    public override void HideScreen(UnityAction onComplete)
    {
        base.HideScreen(onComplete);
    }

    public override void LoadUI()
    {
        base.LoadUI();
    }

    // public void UpdateCoinUI()
    // {
    //     txtCoin.text = $"{DBController.Instance.COIN}";
    // }
    #endregion
}