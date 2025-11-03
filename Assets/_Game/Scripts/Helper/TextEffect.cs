using UnityEngine;
using UnityEngine.UI;

public class TextEffect : MonoBehaviour
{
    [SerializeField] private Text textComponent;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float zoomScale = 0.9f;

    private Coroutine loopCoroutine;

    private void OnEnable()
    {
        StartLoopEffect();
    }

    private void OnDisable()
    {
        StopLoopEffect();
    }

    public void StartLoopEffect()
    {
        if (loopCoroutine == null)
            loopCoroutine = StartCoroutine(LoopZoomCoroutine());
    }

    public void StopLoopEffect()
    {
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
            textComponent.transform.localScale = Vector3.one; // reset scale
        }
    }

    private System.Collections.IEnumerator LoopZoomCoroutine()
    {
        Vector3 originalScale = textComponent.transform.localScale;
        Vector3 targetScale = originalScale * zoomScale;
        float halfDuration = duration / 2f;

        while (true)
        {
            // Zoom in
            float timer = 0f;
            while (timer < halfDuration)
            {
                timer += Time.deltaTime;
                float t = timer / halfDuration;
                textComponent.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            // Zoom out
            timer = 0f;
            while (timer < halfDuration)
            {
                timer += Time.deltaTime;
                float t = timer / halfDuration;
                textComponent.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }
        }
    }
}