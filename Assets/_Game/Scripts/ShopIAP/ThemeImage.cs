using UnityEngine;
using UnityEngine.UI;
using Data;

[RequireComponent(typeof(Image))]
public class ThemeImage : MonoBehaviour
{
    public enum ThemeImageType { Background, Clock, Front }
    public ThemeImageType imageType;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        ApplyTheme();
    }

    public void ApplyTheme()
    {
        var theme = ThemeManager.Instance.CurrentTheme;
        if (theme == null)
        {
            Debug.LogWarning("ThemeImage: Không tìm thấy theme hiện tại!");
            return;
        }

        switch (imageType)
        {
            case ThemeImageType.Background:
                image.sprite = theme.background;
                break;
            case ThemeImageType.Clock:
                image.sprite = theme.clock;
                break;
            case ThemeImageType.Front:
                image.sprite = theme.front;
                break;
        }
    }
}