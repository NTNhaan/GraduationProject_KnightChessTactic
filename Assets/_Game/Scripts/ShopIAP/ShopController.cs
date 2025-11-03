using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Data;

public class ShopController : MonoBehaviour
{
    [Header("References")]
    public DataSOController dataSoController;
    public Transform contentParent;
    public ItemShop prefab;
    public Image backgroundImage;
    public SwipeSnapScroll snapScroll;
    public Text textCoin;

    private readonly List<ItemShop> themeItems = new();
    private int currentIndex;

    private void Start()
    {
        DBController.Instance.InitDefaultTheme(dataSoController.dataSOBG.themes.ToList());

        InitShop();
    
        textCoin.text = DBController.Instance.COIN.ToString();
    
        UpdateBackground(DBController.Instance.SELECTED_THEME);
    }

    private void Update()
    {
        int newIndex = GetCurrentFocusedIndex();
        if (newIndex != currentIndex)
        {
            currentIndex = newIndex;
            UpdateThemeHighlight();
        }
    }

    private void InitShop()
    {
        foreach (var model in dataSoController.dataSOBG.themes)
        {
            bool isOwned = DBController.Instance.IsThemeOwned(model.id);
            bool isSelected = DBController.Instance.SELECTED_THEME == model.id;
        
            ShopItemState state = ShopItemState.Locked;
            if (isSelected) state = ShopItemState.Selected;
            else if (isOwned) state = ShopItemState.Unlocked;
        
            var item = Instantiate(prefab, contentParent);
            item.Setup(this, model.id, model.previewImage, model.price, state);
            themeItems.Add(item);
        }
        UpdateThemeHighlight();
    }

    private void UpdateThemeHighlight()
    {
        for (int i = 0; i < themeItems.Count; i++)
            themeItems[i].SetHighlight(i == currentIndex);
    }

    private int GetCurrentFocusedIndex()
    {
        float scrollValue = snapScroll.GetScrollbarValue();
        int index = Mathf.RoundToInt(scrollValue * (themeItems.Count - 1));
        return Mathf.Clamp(index, 0, themeItems.Count - 1);
    }

    public void TryBuyTheme(string id, int price)
    {
        if (DBController.Instance.IsThemeOwned(id))
            return;

        if (!CoinController.Instance.SpendCoin(price))
        {
            Debug.Log("Not enough coins!");
            return;
        }

        DBController.Instance.AddOwnedTheme(id);
        UpdateItemUI(id, ShopItemState.Unlocked);
        
        textCoin.text = DBController.Instance.COIN.ToString();
    }

    public void SelectTheme(string id)
    {
        if (!DBController.Instance.IsThemeOwned(id))
        {
            Debug.LogWarning($"[ShopController] Theme {id} chưa được mua, không thể chọn!");
            return;
        }

        // Set theme mới được chọn
        DBController.Instance.SetSelectedTheme(id);
        // Cập nhật toàn bộ UI
        foreach (var item in themeItems)
        {
            var newState = GetThemeState(item.Id);
            item.UpdateUIState(newState);
        }

        UpdateBackground(id);
        EventDispatcher.Push(EventId.OnThemeChanged, id);
    }

    private void UpdateItemUI(string id, ShopItemState state)
    {
        foreach (var item in themeItems)
        {
            if (item.Id == id)
                item.UpdateUIState(state);
        }
    }

    public void UpdateBackground(string id)
    {
        var model = dataSoController.GetModelByID(id);
        if (model != null && backgroundImage != null)
        {
            backgroundImage.sprite = model.backgroundImage;
        }
        else
        {
            Debug.LogWarning($"[ShopController] Không tìm thấy background cho id: {id}");
        }
    }

    public ShopItemState GetThemeState(string id)
    {
        if (DBController.Instance.SELECTED_THEME == id)
            return ShopItemState.Selected;
        if (DBController.Instance.IsThemeOwned(id))
            return ShopItemState.Unlocked;
        return ShopItemState.Locked;
    }
}
