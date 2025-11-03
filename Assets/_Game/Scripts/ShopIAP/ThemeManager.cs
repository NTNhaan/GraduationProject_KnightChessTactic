using System.Linq;
using Data;
using UnityEngine;

public class ThemeManager : Singleton<ThemeManager>
{
    [SerializeField] private DataSOBG dataSO;
    public BGModel CurrentTheme => GetThemeById(DBController.Instance.SELECTED_THEME);

    public BGModel GetThemeById(string id)
    {
        if (dataSO == null || dataSO.themes == null) return null;
        return dataSO.themes.FirstOrDefault(t => t.id == id);
    }
    public BGModel GetCurrentTheme()
    {
        string selectedId = DBController.Instance.SELECTED_THEME;
        return DataSOController.Instance.GetModelByID(selectedId);
    }
    
    public void SelectTheme(string id)
    {
        var theme = GetThemeById(id);
        if (theme == null)
        {
            Debug.LogWarning($"Không tìm thấy theme có ID = {id}");
            return;
        }

        DBController.Instance.SELECTED_THEME = id;
    }
}