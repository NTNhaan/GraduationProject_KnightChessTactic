using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Data;
using TMPro;
using UnityEngine.Serialization;

public class LoadingFade : Singleton<LoadingFade>
{
    [SerializeField] private float timeClose = 0.8f;
    [SerializeField] private float timeOpen = 0.8f;
    [SerializeField] private Ease easeOpen = Ease.OutQuad;
    [SerializeField] private Ease easeClose = Ease.OutQuad;

    [Header("Doors")]
    [SerializeField] private DoorMover leftDoor;
    [SerializeField] private DoorMover rightDoor;
    [Header("UI")]
    [SerializeField] private TextMeshPro txtBottomBanner;

    private float bounceDistance = 40f;
    private int doorHitCount = 0;
    private Vector2 leftDoorStartPos;
    private Vector2 rightDoorStartPos;

    private void CustomAwake()
    {
        txtBottomBanner.gameObject.SetActive(false);
        leftDoor.gameObject.SetActive(false);
        rightDoor.gameObject.SetActive(false);
    }
    public async UniTask ShowLoadingFade(int midSpriteIndex = -1)
    {
        InGameData.GAME_STATE = GameState.Loading;
        EventDispatcher.Push(EventId.OnGameStateChanged); // Disable buttons ngay lập tức
        doorHitCount = 0;

        leftDoor.ResetDoor();
        rightDoor.ResetDoor();

        // Lưu vị trí ban đầu
        leftDoorStartPos = leftDoor.transform.localPosition;
        rightDoorStartPos = rightDoor.transform.localPosition;

        leftDoor.gameObject.SetActive(true);
        rightDoor.gameObject.SetActive(true);
        txtBottomBanner.gameObject.SetActive(true);

        // Set Rigidbody2D thành Kinematic để tránh conflict với DOTween
        leftDoor.SetKinematic(true);
        rightDoor.SetKinematic(true);

        // Disable collider để tránh đẩy player khi move
        leftDoor.SetColliderEnabled(false);
        rightDoor.SetColliderEnabled(false);

        // Đảm bảo doors đã stop trước khi dùng DOTween
        leftDoor.Stop();
        rightDoor.Stop();

        // Lấy vị trí ban đầu
        float leftStartX = leftDoorStartPos.x;
        float rightStartX = rightDoorStartPos.x;

        // Tính vị trí giữa (nơi 2 doors sẽ gặp nhau)
        float centerX = (leftStartX + rightStartX) / 2f;

        // Move doors vào giữa bằng DOTween thay vì Rigidbody2D
        float moveDuration = 0.8f; // Thời gian move vào giữa

        await UniTask.WhenAll(
            leftDoor.transform.DOLocalMoveX(centerX, moveDuration)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion()
                .AsUniTask(),
            rightDoor.transform.DOLocalMoveX(centerX, moveDuration)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion()
                .AsUniTask()
        );

        await BounceDoors();

        txtBottomBanner.DOFade(1, 1f).From(0).SetEase(Ease.OutQuad);
        DOTween.To(() => 0, x =>
        {
            txtBottomBanner.text = "Loading" + new string('.', x % 4);
        }, 3, 1f).SetLoops(-1).SetEase(Ease.Linear);
        await Task.Delay(1000);
    }
    public async UniTask HideLoadingFade()
    {
        txtBottomBanner.DOFade(0, 0.3f);

        // Đảm bảo doors đã stop và kinematic
        leftDoor.Stop();
        rightDoor.Stop();
        leftDoor.SetKinematic(true);
        rightDoor.SetKinematic(true);

        // Disable collider để tránh đẩy player khi move out
        leftDoor.SetColliderEnabled(false);
        rightDoor.SetColliderEnabled(false);

        // Lấy vị trí ban đầu đã lưu
        float leftStartX = leftDoorStartPos.x;
        float rightStartX = rightDoorStartPos.x;

        // Move doors ra ngoài bằng DOTween
        float moveOutDuration = 0.7f;

        await UniTask.WhenAll(
            leftDoor.transform.DOLocalMoveX(leftStartX, moveOutDuration)
                .SetEase(Ease.InQuad)
                .AsyncWaitForCompletion()
                .AsUniTask(),
            rightDoor.transform.DOLocalMoveX(rightStartX, moveOutDuration)
                .SetEase(Ease.InQuad)
                .AsyncWaitForCompletion()
                .AsUniTask()
        );

        // Set lại Dynamic sau khi xong (nếu cần)
        leftDoor.SetKinematic(false);
        rightDoor.SetKinematic(false);

        leftDoor.Stop();
        rightDoor.Stop();

        leftDoor.gameObject.SetActive(false);
        rightDoor.gameObject.SetActive(false);
        txtBottomBanner.gameObject.SetActive(false);

        InGameData.GAME_STATE = GameState.LoadingDone;
        EventDispatcher.Push(EventId.OnGameStateChanged);
    }
    private void OnDoorHit()
    {
        doorHitCount++;
    }
    private async UniTask BounceDoors()
    {
        float bounce = 0.1f;   // nhỏ hơn để đỡ gắt
        float bounceTime = 0.18f;
        float snapTime = 0.22f;

        float leftX = leftDoor.transform.localPosition.x;
        float rightX = rightDoor.transform.localPosition.x;

        // bật nhẹ ra (mềm hơn)
        await UniTask.WhenAll(
            leftDoor.transform.DOLocalMoveX(leftX - bounce, bounceTime)
                .SetEase(Ease.OutSine)
                .AsyncWaitForCompletion()
                .AsUniTask(),

            rightDoor.transform.DOLocalMoveX(rightX + bounce, bounceTime)
                .SetEase(Ease.OutSine)
                .AsyncWaitForCompletion()
                .AsUniTask()
        );

        // sát lại chậm và mềm
        await UniTask.WhenAll(
            leftDoor.transform.DOLocalMoveX(leftX, snapTime)
                .SetEase(Ease.OutSine)
                .AsyncWaitForCompletion()
                .AsUniTask(),

            rightDoor.transform.DOLocalMoveX(rightX, snapTime)
                .SetEase(Ease.OutSine)
                .AsyncWaitForCompletion()
                .AsUniTask()
        );
    }

}