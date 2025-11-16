using UnityEngine;
using Audio;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Setting;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Data;
public class GamePlayScreen : ScreenBase
{
    void Start()
    {
        SettingCtrl.Instance.InitSetting();
    }
    #region Override Methods
    public override void ShowScreen(UnityAction onComplete)
    {
        // LoadUI();
        base.ShowScreen(onComplete);
    }

    public override void HideScreen(UnityAction onComplete)
    {
        base.HideScreen(onComplete);
    }

    public override void LoadCoinUI()
    {
        base.LoadCoinUI();
    }

    // public void UpdateCoinUI()
    // {
    //     txtCoin.text = $"{DBController.Instance.COIN}";
    // }
    #endregion
}
