using Popup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Audio;
using Data;
using Spin;
using UnityEngine.Serialization;

public class SpinRewardPopup : PopUpBase
{
    [Header("Popup UI")]
    [SerializeField] private Text txtAmountEnergySpin;
    [SerializeField] private Button btnSpin;
    [SerializeField] private Button btnExit;
    [SerializeField] private Image imgFillAccumulate;
    [SerializeField] private Text txtAccumulate;
    [SerializeField] private ScreenGetRW screenGetRW;
    
    private void OnEnable()
    {
        EventDispatcher.Register(EventId.OnSpinReward, OnSpinReward);
        EventDispatcher.Register(EventId.OnAccumulateReward, OnAccumulateReward);
        EventDispatcher.Register(EventId.OnSpinStateChanged, OnSpinStateChanged);
        EventDispatcher.Register(EventId.OnAccumulateChanged, OnAccumulateChanged);

        if (txtAmountEnergySpin != null)
        {
            txtAmountEnergySpin.text = $"-{GameConfig.SPIN_ENERGY}";
        }
    }

    private void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.OnSpinReward, OnSpinReward);
        EventDispatcher.RemoveCallback(EventId.OnAccumulateReward, OnAccumulateReward);
        EventDispatcher.RemoveCallback(EventId.OnSpinStateChanged, OnSpinStateChanged);
        EventDispatcher.RemoveCallback(EventId.OnAccumulateChanged, OnAccumulateChanged);
    }
    
    #region Overrides Func
    public override void ShowPopUp(float posY, float duration, UnityAction onComplete = null)
    {
        ShowCover(0.5f, () =>
        {
            base.ShowPopUp(posY, duration, onComplete);
        });
    }
    public override void HidePopUp(float posY, float duration, UnityAction onComplete = null)
    {
        base.HidePopUp(posY, duration, () =>
        {
            onComplete?.Invoke();
            // tfmPopup.gameObject.SetActive(false);
            HideCover();
        });
    }

    public override void ShowCover(float duration = 0.5f, UnityAction onComplete = null)
    {
        base.ShowCover(duration, onComplete);
    }


    public override void HideCover(UnityAction onComplete = null)
    {
        base.HideCover(onComplete);
    }
    #endregion
    
    #region SpinRewardPopup
    [ContextMenu("Show SpinReward Popup")]
    public async UniTask ShowSpinRewardPopUp()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        // InGameData.GAME_STATE = GameState.PauseGame;
        // EventDispatcher.Push(EventId.OnGameStateChanged);
        // EventManager.PasueGame();
        TabController.Instance.HideBottomTab();
        UITopController.Instance.HideTab();
        SpinController.Instance.InitSpin();
        ShowPopUp(0f, 0.3f, () =>
        {
            btnSpin.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            btnExit.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        });
    }
    
    [ContextMenu("Hide SpinReward Popup")]
    public void HideSpinRewardPopUp()
    {
        AudioController.Instance.PlayEffect(Sound.Name.Sound_PopupOpen);
        // InGameData.GAME_STATE = GameState.PauseGame;
        // EventDispatcher.Push(EventId.OnGameStateChanged);
        // EventManager.PasueGame();
        btnSpin.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        btnExit.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        HidePopUp(2500f, 0.3f, () =>
        {
            TabController.Instance.ShowBottomTab();
            UITopController.Instance.ShowTab();
        });
    }
    public void OnSpinButtonClick()
    {
        SpinController.Instance.TrySpin();
    }
    public void OnExitButtonClick()
    {
        HideSpinRewardPopUp();
    }
    #endregion
    
    
    private void OnSpinReward(object data)
    {
        Debug.Log(">>> OnSpinReward EVENT RECEIVED");

        var reward = data as ModelDataSpin;
        if (reward == null) return;

        screenGetRW.OnShowScreen(reward.value, reward.sprReward);
    }

    private void OnAccumulateReward(object data)
    {
        int exp = (int)data;
        screenGetRW.OnShowScreen(exp);
    }

    private void OnSpinStateChanged(object isSpinningObj)
    {
        bool isSpinning = (bool)isSpinningObj;

        btnSpin.interactable = !isSpinning;
        btnExit.interactable = !isSpinning;
    }

    private void OnAccumulateChanged(object obj)
    {
        Vector3 data = (Vector3)obj;

        int cur = (int)data.x;
        int max = (int)data.y;
        float fill = data.z;

        txtAccumulate.text = $"{cur}/{max}";
        imgFillAccumulate.DOFillAmount(fill, 0.5f);
    }
}