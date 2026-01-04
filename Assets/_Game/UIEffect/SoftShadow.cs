using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[AddComponentMenu("UI/Effects/Soft Shadow")]
public class SoftShadow : BaseMeshEffect
{
    [Header("Shadow")]
    [SerializeField] private Color shadowColor = new Color(0, 0, 0, 0.25f);
    [SerializeField] private Vector2 shadowOffset = new Vector2(8f, -8f);

    [Header("Blur")]
    [Range(1, 8)]
    [SerializeField] private int blurIterations = 3;

    [Range(0f, 1f)]
    [SerializeField] private float blurStep = 0.4f;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        List<UIVertex> verts = new List<UIVertex>();
        vh.GetUIVertexStream(verts);

        int originalCount = verts.Count;

        for (int i = 0; i < blurIterations; i++)
        {
            float factor = 1f - (i * blurStep);
            Vector2 offset = shadowOffset * factor;

            ApplyShadow(
                verts,
                shadowColor,
                originalCount * i,
                originalCount * (i + 1),
                offset.x,
                offset.y,
                factor
            );
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(verts);
    }

    private void ApplyShadow(
        List<UIVertex> verts,
        Color color,
        int start,
        int end,
        float x,
        float y,
        float alphaFactor
    )
    {
        for (int i = start; i < end; i++)
        {
            UIVertex vt = verts[i];
            Vector3 pos = vt.position;
            pos.x += x;
            pos.y += y;
            vt.position = pos;

            Color c = color;
            c.a *= alphaFactor;
            vt.color = c;

            verts.Add(vt);
        }
    }
}