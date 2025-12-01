using Data;
using UnityEngine;
public class DeviceInfo : Singleton<DeviceInfo>
{
    public float ScreenWidth { get; private set; }
    public float ScreenHeight { get; private set; }

    protected override void CustomAwake()
    {
        base.CustomAwake();

        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;

        DontDestroyOnLoad(gameObject);
    }
    public static float GetUIHalfWidth(RectTransform canvas)
    {
        return canvas.sizeDelta.x / 2f;
    }

    public static float GetUIHalfHeight(RectTransform canvas)
    {
        return canvas.sizeDelta.y / 2f;
    }
}