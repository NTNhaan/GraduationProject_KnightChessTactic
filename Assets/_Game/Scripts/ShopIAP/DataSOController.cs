using System.Linq;
using Data;
using UnityEngine;

public class DataSOController : Singleton<DataSOController>
{
    public DataSOBG dataSOBG;

    public Sprite GetPreviewByID(string id)
    {
        var theme = dataSOBG.themes.FirstOrDefault(t => t.id == id);
        return theme?.previewImage;
    }

    public Sprite GetBackgroundByID(string id)
    {
        var theme = dataSOBG.themes.FirstOrDefault(t => t.id == id);
        return theme?.backgroundImage;
    }

    public int GetPriceByID(string id)
    {
        var theme = dataSOBG.themes.FirstOrDefault(t => t.id == id);
        return theme?.price ?? 0;
    }

    public BGModel GetModelByID(string id)
    {
        return dataSOBG.themes.FirstOrDefault(t => t.id == id);
    }
}