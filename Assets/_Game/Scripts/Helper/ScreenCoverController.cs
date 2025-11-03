using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCoverController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer coverSprite;
    private Material mat;

    void Awake()
    {
        mat = coverSprite.material;
    }

    // Show full black cover
    public void ShowFullBlack(float fade)
    {
        mat.SetFloat("_ShowMode", 0f);
        mat.SetFloat("_Fade", fade);
    }

    // Show black cover with hole around target
    public void ShowWithHold(Transform target, float fade, float radius)
    {
        mat.SetFloat("_ShowMode", 1f);
        mat.SetFloat("_Fade", fade);
        mat.SetFloat("_HoldRadius", radius);

        // Convert world position to UV
        Vector3 screenPos = Camera.main.WorldToViewportPoint(target.position);
        mat.SetVector("_HoldCenter", new Vector4(screenPos.x, screenPos.y, 0, 0));
    }
}
