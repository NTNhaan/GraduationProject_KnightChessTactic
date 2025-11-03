using Data;
using UnityEngine;
using UnityEngine.UI;

public class ScreenOverlay : Singleton<ScreenOverlay>
{
    private Material mat;
    [SerializeField] private SpriteRenderer sr;

    private void Awake()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            mat = Instantiate(sr.sharedMaterial);
            sr.material = mat;
        }
    }
    
    public void ShowBlackOverlay(float alpha = 0.75f)
    {
        Debug.Log($"CheckRadius: 0 | mat={(mat == null ? "NULL" : mat.name)} | sr={(sr == null ? "NULL" : sr.name)}");
        Debug.Log($"CheckRadius: 1");
        if (mat == null)
        {
            Debug.LogError("Material is NULL!");
            return;
        }

        mat.SetColor("_Color", new Color(0, 0, 0, alpha));
        Debug.Log($"CheckRadius: 2");

        mat.SetFloat("_HoleRadius", 0f);
        Debug.Log($"CheckRadius: 3");

        if (sr != null)
            sr.enabled = true;
        Debug.Log($"CheckRadius: 4");
    }
    
    public void ShowWithHole(Vector2 screenPos, float radius = 0.2f, float smooth = 20f)
    {
        mat.SetColor("_Color", new Color(0, 0, 0, 0.75f));

        // Convert Screen Position sang UV
        Vector2 uv = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        mat.SetVector("_HoleCenter", new Vector4(uv.x, uv.y, 0, 0));

        mat.SetFloat("_HoleRadius", radius);
        mat.SetFloat("_Smooth", smooth);

        sr.enabled = true;
    }
    
    public void HideOverlay()
    {
        sr.enabled = false;
    }
}