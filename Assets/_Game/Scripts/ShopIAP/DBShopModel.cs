using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatusModel
{
    public string id;
    public ShopItemState state;
}

public enum ShopItemState
{
    Locked,
    Unlocked,
    Selected
}

[Serializable]
public class DBShopModel
{
    public List<StatusModel> items = new();

    public ShopItemState GetState(string id)
    {
        var item = items.Find(x => x.id == id);
        Debug.Log($"CheckStateItem: {item.id} : {item.state}");
        return item != null ? item.state : ShopItemState.Locked;
    }

    public void SetState(string id, ShopItemState newState)
    {
        var item = items.Find(x => x.id == id);
        if (item == null)
        {
            item = new StatusModel { id = id, state = newState };
            items.Add(item);
        }
        else
        {
            item.state = newState;
        }
    }

    public string GetSelectedID()
    {
        var item = items.Find(x => x.state == ShopItemState.Selected);
        return item?.id;
    }

    public void DeselectAll()
    {
        foreach (var item in items)
            if (item.state == ShopItemState.Selected)
                item.state = ShopItemState.Unlocked;
    }
}