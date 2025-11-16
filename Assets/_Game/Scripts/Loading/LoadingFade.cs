using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Data;
using UnityEngine.Serialization;

public class LoadingFade : Singleton<LoadingFade>
{
    [SerializeField] private float timeClose = 0.8f;
    [SerializeField] private float timeOpen = 0.5f;
    [SerializeField] private Ease easeOpen = Ease.OutQuad;
    [SerializeField] private Ease easeClose = Ease.OutQuad;

    [SerializeField] private RectTransform imgLeftBanner;
    [SerializeField] private RectTransform imgRightBanner;
    [SerializeField] private Text txtBottomBanner;

    // [SerializeField] private List<Sprite> midSprites;
    
    private float bounceDistance  = 40f; 
    
    public async UniTask ShowLoadingFade(int midSpriteIndex = -1)
    {
        InGameData.GAME_STATE = GameState.Loading;
        imgLeftBanner.gameObject.SetActive(true);
        imgRightBanner.gameObject.SetActive(true);
        txtBottomBanner.gameObject.SetActive(true);

        // if (midSpriteIndex >= 0 && midSpriteIndex < midSprites.Count)
        // {
        //     midBanner.sprite = midSprites[midSpriteIndex];
        // }
        // else
        // {
        //     int rand = Random.Range(0, midSprites.Count);
        //     midBanner.sprite = midSprites[rand];
        // }

        // var tLeftIn = imgLeftBanner.DOAnchorPosX(0f, 0.45f)
        //     .SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask();
        // var tRightIn = imgRightBanner.DOAnchorPosX(0f, 0.45f)
        //     .SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask();
        // await UniTask.WhenAll(tLeftIn, tRightIn);
        //
        //
        // var tLeftBounce = imgLeftBanner.DOAnchorPosX(0f - bounceDistance, 0.12f)
        //     .SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask();
        // var tRightBounce = imgRightBanner.DOAnchorPosX(0f + bounceDistance, 0.12f)
        //     .SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask();
        // await UniTask.WhenAll(tLeftBounce, tRightBounce);
        //
        // var tLeftSnap = imgLeftBanner.DOAnchorPosX(0f, 0.12f)
        //     .SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        // var tRightSnap = imgRightBanner.DOAnchorPosX(0f, 0.12f)
        //     .SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask();
        // await UniTask.WhenAll(tLeftSnap, tRightSnap);
        
        await BannerAnimationHelper.PlayDoubleImpact(
            imgLeftBanner,
            imgRightBanner,
            hitPos: 0f
        );
        txtBottomBanner.DOFade(1, 1f).From(0).SetEase(Ease.OutQuad);
        await Task.Delay(500);
    }
    public async UniTask HideLoadingFade()
    {
        var tLeft = imgLeftBanner.DOAnchorPosX(-1000, 0.5f).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask();
        var tRight = imgRightBanner.DOAnchorPosX(1000, 0.5f).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask();
        var t3 = txtBottomBanner.DOFade(0, 0.5f).From(1).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(tLeft, tRight, t3);

        // await imgBackground.DOFade(0, 1f).From(1).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

        InGameData.GAME_STATE = GameState.LoadingDone;

        imgLeftBanner.gameObject.SetActive(false);
        imgRightBanner.gameObject.SetActive(false);
        txtBottomBanner.gameObject.SetActive(false);
    }

}