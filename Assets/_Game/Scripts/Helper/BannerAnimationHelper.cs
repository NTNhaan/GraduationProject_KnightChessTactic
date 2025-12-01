using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public static class BannerAnimationHelper
{
    // Banner lao vào + bounce 2 lần + dính lại
    public static async UniTask PlayDoubleImpact(
        RectTransform left,
        RectTransform right,
        float leftHitPos,
        float rightHitPos,
        float inTime = 0.45f,
        float bounce1 = 60f,
        float bounce2 = 20f,
        float bounceTime1 = 0.12f,
        float bounceTime2 = 0.10f,
        float snapTime = 0.10f)
    {
        // STEP 1: Move vào hitPos
        await UniTask.WhenAll(
            left.DOAnchorPosX(leftHitPos, inTime).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask(),
            right.DOAnchorPosX(rightHitPos, inTime).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask()
        );

        // STEP 2: Bounce mạnh
        await UniTask.WhenAll(
            left.DOAnchorPosX(leftHitPos - bounce1, bounceTime1).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask(),
            right.DOAnchorPosX(rightHitPos + bounce1, bounceTime1).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask()
        );

        // STEP 3
        await UniTask.WhenAll(
            left.DOAnchorPosX(leftHitPos + 10f, bounceTime1).SetEase(Ease.InQuad).AsyncWaitForCompletion().AsUniTask(),
            right.DOAnchorPosX(rightHitPos - 10f, bounceTime1).SetEase(Ease.InQuad).AsyncWaitForCompletion().AsUniTask()
        );

        // STEP 4: Bounce nhỏ
        await UniTask.WhenAll(
            left.DOAnchorPosX(leftHitPos - bounce2, bounceTime2).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask(),
            right.DOAnchorPosX(rightHitPos + bounce2, bounceTime2).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask()
        );

        // STEP 5: Snap lại
        await UniTask.WhenAll(
            left.DOAnchorPosX(leftHitPos, snapTime).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask(),
            right.DOAnchorPosX(rightHitPos, snapTime).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask()
        );
    }
}
