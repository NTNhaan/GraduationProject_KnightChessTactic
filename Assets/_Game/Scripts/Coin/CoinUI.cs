using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using Audio;
using Data;

public class CoinUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Text txtCoinFlyPrefab;
    [SerializeField] private RectTransform flyParent;
    [SerializeField] private Text txtCoinBar;

    public Transform tfmCoinText => txtCoinBar.transform;
    private Vector3 targetPos;
    public void OnEnable()
    {
        EventDispatcher.Register(EventId.OnCoinChanged, UpdateCoinUI);
    }
    public void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.OnCoinChanged, UpdateCoinUI);
        CoinController.OnCoinChanged -= AnimateCoinNumber;
    }
    private void Start()
    {
        targetPos = txtCoinBar.transform.position;

        CoinController.OnCoinChanged += AnimateCoinNumber;
        txtCoinBar.text = CoinController.Instance.CurrentCoin.ToString();
    }
    public void UpdateCoinUI(object data = null)
    {
        txtCoinBar.text = CoinController.Instance.CurrentCoin.ToString("n0");
    }
    
    private void AnimateCoinNumber(int from, int to)
    {
        AnimateMoneyText(from, to, 0.4f).Forget();
    }

    public async UniTaskVoid AnimateMoneyText(int from, int to, float duration)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            await UniTask.Yield();
            int curr = (int)Mathf.Lerp(from, to, t);
            txtCoinBar.text = curr.ToString();
        }
        txtCoinBar.text = to.ToString();
    }
    
    public async UniTaskVoid PlayCoinFlyEffect(Vector3 spawnPos, int amount, float animTime, float interval)
    {
        // AudioController.Instance.PlayEffect(Sound.Name.Sound_Coin);

        List<UniTask> tasks = new List<UniTask>();

        for (int i = 0; i < amount; i++)
        {
            var fx = Instantiate(txtCoinFlyPrefab, flyParent);
            fx.text = "+1";
            fx.transform.position = spawnPos;

            await UniTask.Delay((int)(interval * 1000));

            var t = fx.transform
                .DOMove(targetPos, animTime)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => Destroy(fx.gameObject))
                .ToUniTask();

            tasks.Add(t);
        }

        await UniTask.WhenAll(tasks);
    }
}
