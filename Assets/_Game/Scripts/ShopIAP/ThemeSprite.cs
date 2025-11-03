using UnityEngine;
public class ThemeSprite : MonoBehaviour
{
    public enum Type { Background, Clock, Front }
    [SerializeField] private Type spriteType;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ApplyTheme();
    }

    public void ApplyTheme()
    {
        var theme = ThemeManager.Instance.GetCurrentTheme();
        if (theme == null) return;

        switch (spriteType)
        {
            case Type.Background:
                spriteRenderer.sprite = theme.background;
                break;
            case Type.Clock:
                spriteRenderer.sprite = theme.clock;
                break;
            case Type.Front:
                spriteRenderer.sprite = theme.front;
                break;
        }
    }
}