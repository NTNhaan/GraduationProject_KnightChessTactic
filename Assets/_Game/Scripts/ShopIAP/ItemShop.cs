using Data;
using UnityEngine;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour
{
    [Header("UI References")]
    public Image previewImage;
    public GameObject btnBuy;
    public GameObject btnSelect;
    public GameObject imgCheck;
    public Text txtPrice;
    public GameObject frameHighlight;

    private string themeId;
    private int price;
    private ShopController shopController;
    private ShopItemState currentState;

    public string Id => themeId;

    public void Setup(ShopController controller, string id, Sprite preview, int price, ShopItemState state)
    {
        this.shopController = controller;
        this.themeId = id;
        this.price = price;
        this.currentState = state;
    
        previewImage.sprite = preview;
        txtPrice.text = price.ToString();
    
        UpdateUIState(state);
        frameHighlight.SetActive(false);
    }
    public void OnBuyClick()
    {
        Debug.Log("OnBuyClick");
        if (DBController.Instance.COIN < price)
        {
            PopupController.Instance.SetTextNotify("You don't have enough coins to buy this item");
            PopupController.Instance.ShowNotifyPopUp();
            return;
        }

        PopupController.Instance.ShowConfirmPopUp(
            onYes: () => shopController.TryBuyTheme(themeId, price)
        );
    }

    public void OnSelectClick()
    {
        shopController.SelectTheme(themeId);
        ThemeManager.Instance.SelectTheme(themeId);
    }

    public void SetHighlight(bool active)
    {
        frameHighlight.SetActive(active);
    }

    public void UpdateUIState(ShopItemState state)
    {
        currentState = state;

        btnBuy.SetActive(false);
        btnSelect.SetActive(false);
        imgCheck.SetActive(false);

        var model = DataSOController.Instance.GetModelByID(themeId);

        switch (state)
        {
            case ShopItemState.Locked:
                btnBuy.SetActive(true);
                previewImage.sprite = model?.lockImage;
                break;

            case ShopItemState.Unlocked:
                btnSelect.SetActive(true);
                previewImage.sprite = model?.previewImage;
                break;

            case ShopItemState.Selected:
                imgCheck.SetActive(true);
                previewImage.sprite = model?.previewImage;
                break;
        }
    }
    public void RefreshStateFromDB()
    {
        var newState = shopController.GetThemeState(themeId);
        UpdateUIState(newState);
    }
}