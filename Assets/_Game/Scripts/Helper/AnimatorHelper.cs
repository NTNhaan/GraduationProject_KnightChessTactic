using System;
using System.Collections;
using Audio;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using UnityEngine.Events;

public class AnimatorHelper : Singleton<AnimatorHelper>
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform tfmParentDia;
    public GameObject diamonFly;
    public GameObject hammerFly;
    public GameObject swapFly;
    public GameObject prefabSprite;
    private GameObject clone;
    private int directAnim = 1;
    
    public async UniTask WaitForStateComplete(Animator animator, string stateName, int layer = 0)
    {
        Debug.Log($"[AnimatorHelper] State Name: {stateName}");
        int hash = Animator.StringToHash(stateName);
        
        await UniTask.Yield();
        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(layer).shortNameHash == hash);
        await UniTask.WaitUntil(() =>
        {
            var s = animator.GetCurrentAnimatorStateInfo(layer);
            return s.normalizedTime >= 1f && !animator.IsInTransition(layer);
        });
        Debug.Log($"[AnimatorHelper] Done State Name: {stateName}");
    }

    public void ObjectFly(Vector3 firstPos, int objectAmount, Transform lastPos, Transform parent,
        UnityAction onCompleted = null)
    {
        Debug.Log($"CheckObjectFly: {firstPos} - {objectAmount} - {lastPos} - {parent}");
        Vector3 moveToPosition = lastPos.position;
        float delay = 0;
        float delta = Mathf.Clamp(1.0f / objectAmount, 0.1f, 0.3f);

        int completedCount = 0;

        for (int i = 0; i < objectAmount; i++)
        {
            int index = i;
            DelayFunction(delay, () =>
            {
                float basePitch = 0.9f;
                float pitchStep = 0.05f;
                float pitch = basePitch + index * pitchStep;
                pitch = Mathf.Clamp(pitch, 0.9f, 1.4f);
                float volumeScale = Mathf.Lerp(0.8f, 1f, (float)index / objectAmount);
                AudioController.Instance.PlayEffectPooled(
                    Sound.Name.Sound_CoinSpawn,
                    volumeScale,
                    pitch,
                    pitch
                );
                
                var cloneObj = Instantiate(prefab, parent);
                cloneObj.transform.position = firstPos;
                cloneObj.transform.localScale = Vector3.one * 0.5f;

                var curveHelper = cloneObj.GetComponent<CurveMove>();
                curveHelper.CurveMoveVer1(firstPos, moveToPosition, 1f, () =>
                {
                    AudioController.Instance.PlayEffectPooled(
                        Sound.Name.Sound_CoinRecive,
                        1f,
                        UnityEngine.Random.Range(0.95f, 1.1f),
                        UnityEngine.Random.Range(0.95f, 1.1f)
                    );
                    
                    Destroy(cloneObj);

                    completedCount++;
                    if (completedCount >= objectAmount)
                    {
                        onCompleted?.Invoke();
                    }
                });

                cloneObj.transform.DOScale(UnityEngine.Random.Range(1f, 1.2f), 0.45f);
            });
            delay += 0.06f;
            // delay += delta / 2;
        }
    }
    public void MultiObject(PrefabObjectFly prefabFly, float objectAmount, Transform lastPos)
    {
        GameObject prefabToUse;
        switch (prefabFly)
        {
            case PrefabObjectFly.Coin:
                prefabToUse = diamonFly;
                break;
            case PrefabObjectFly.Hammer:
                prefabToUse = hammerFly;
                break;
            case PrefabObjectFly.Swap:
                prefabToUse = swapFly;
                break;
            default:
                prefabToUse = diamonFly;
                break;
        }
        
        Vector3 moveToPosition = lastPos.position;
        float delay = 0;
        float delta = Mathf.Clamp(1.0f / objectAmount, 0.01f, 0.3f);

        for (int i = 0; i < objectAmount; i++)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                var cloneObj = Instantiate(prefabToUse, tfmParentDia);
                cloneObj.transform.position = tfmParentDia.transform.position;
                cloneObj.transform.localScale = Vector3.one * 0.5f;
                var curveHelper = cloneObj.gameObject.GetComponent<CurveMove>();
                curveHelper.CurveMoveVer2(tfmParentDia.transform.position, moveToPosition, 0.5f, directAnim,
                    () => { Destroy(cloneObj); });

                cloneObj.transform.DOScale(UnityEngine.Random.Range(1f, 1.2f), 0.45f);
            });
            delay += delta / 2;
        }
    }
    public async UniTask WaitForComplePartical(ParticleSystem particle, UnityAction onComplete = null)
    {
        if (particle == null) return;
        await UniTask.WaitUntil(() => particle.isPlaying || particle.IsAlive(true));
        onComplete?.Invoke();
    }
    
    public void DelayFunction(float delayTime, Action action) {
        StartCoroutine(DelayFunctionIEumerator(action, delayTime));
    }
    IEnumerator DelayFunctionIEumerator(Action action, float waitTime) {
        yield return new WaitForSeconds(waitTime);
        action?.Invoke();
    }

    public void Stop() {
        StopAllCoroutines();
    }

    private void OnDisable() {
        StopAllCoroutines();
    }
}

public enum PrefabObjectFly
{
    Coin,
    Exp,
    Energy,
    Hammer,
    Swap,
    Broom,
}