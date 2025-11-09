using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Data;

public class LoadingFade : Singleton<LoadingFade>
{
    [SerializeField] private float timeClose = 0.8f;
    [SerializeField] private float timeOpen = 0.5f;
    [SerializeField] private Ease easeOpen = Ease.OutQuad;
    [SerializeField] private Ease easeClose = Ease.OutQuad;
    [SerializeField] private Image imgBackground;

    [SerializeField] private Image topBanner;
    [SerializeField] private Image midBanner;
    [SerializeField] private Text bottomBanner;

    // [SerializeField] private List<Sprite> midSprites;
    public async UniTask ShowLoadingFade(int midSpriteIndex = -1)
    {
        InGameData.GAME_STATE = GameState.Loading;
        imgBackground.gameObject.SetActive(true);
        topBanner.gameObject.SetActive(true);
        midBanner.gameObject.SetActive(true);
        bottomBanner.gameObject.SetActive(true);

        // if (midSpriteIndex >= 0 && midSpriteIndex < midSprites.Count)
        // {
        //     midBanner.sprite = midSprites[midSpriteIndex];
        // }
        // else
        // {
        //     int rand = Random.Range(0, midSprites.Count);
        //     midBanner.sprite = midSprites[rand];
        // }

        imgBackground.DOFade(1, 1f).From(0).SetEase(Ease.OutQuad);
        topBanner.DOFade(1, 1f).From(0).SetEase(Ease.OutQuad);
        midBanner.DOFade(1, 1f).From(0).SetEase(Ease.OutQuad);
        bottomBanner.DOFade(1, 1f).From(0).SetEase(Ease.OutQuad);
        await Task.Delay(500);
    }
    public async UniTask HideLoadingFade()
    {
        var t1 = topBanner.DOFade(0, 0.5f).From(1).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask();
        var t2 = midBanner.DOFade(0, 0.5f).From(1).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask();
        var t3 = bottomBanner.DOFade(0, 0.5f).From(1).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(t1, t2, t3);

        await imgBackground.DOFade(0, 1f).From(1).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

        InGameData.GAME_STATE = GameState.LoadingDone;

        imgBackground.gameObject.SetActive(false);
        topBanner.gameObject.SetActive(false);
        midBanner.gameObject.SetActive(false);
        bottomBanner.gameObject.SetActive(false);
    }

}