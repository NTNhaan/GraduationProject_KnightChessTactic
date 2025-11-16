using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public static class BannerAnimationHelper
{
    // Banner lao vào + bounce 2 lần + dính lại
    public static async UniTask PlayDoubleImpact(
        RectTransform left,
        RectTransform right,
        float hitPos = 0f,
        float inTime = 0.45f,
        float bounce1 = 60f,
        float bounce2 = 20f,
        float bounceTime1 = 0.12f,
        float bounceTime2 = 0.10f,
        float snapTime = 0.10f)
    {
        // STEP 1: Move vào hitPos
        await UniTask.WhenAll(
            left.DOAnchorPosX(hitPos, inTime).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask(),
            right.DOAnchorPosX(hitPos, inTime).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask()
        );

        // STEP 2: Bounce 1 (bật mạnh xa nhất)
        await UniTask.WhenAll(
            left.DOAnchorPosX(hitPos - bounce1, bounceTime1).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask(),
            right.DOAnchorPosX(hitPos + bounce1, bounceTime1).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask()
        );

        // STEP 3: Trở lại gần hitPos
        await UniTask.WhenAll(
            left.DOAnchorPosX(hitPos + 10f, bounceTime1).SetEase(Ease.InQuad).AsyncWaitForCompletion().AsUniTask(),
            right.DOAnchorPosX(hitPos - 10f, bounceTime1).SetEase(Ease.InQuad).AsyncWaitForCompletion().AsUniTask()
        );

        // STEP 4: Bounce 2 nhỏ
        await UniTask.WhenAll(
            left.DOAnchorPosX(hitPos - bounce2, bounceTime2).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask(),
            right.DOAnchorPosX(hitPos + bounce2, bounceTime2).SetEase(Ease.OutQuad).AsyncWaitForCompletion().AsUniTask()
        );

        // STEP 5: Snap lại vào hitPos
        await UniTask.WhenAll(
            left.DOAnchorPosX(hitPos, snapTime).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask(),
            right.DOAnchorPosX(hitPos, snapTime).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask()
        );
    }
}
