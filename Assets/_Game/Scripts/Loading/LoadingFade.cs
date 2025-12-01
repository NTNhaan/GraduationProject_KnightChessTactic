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
    
    private float bounceDistance  = 40f; 
    private int doorHitCount = 0;
    
    private void CustomAwake()
    {
        txtBottomBanner.gameObject.SetActive(false);
        leftDoor.gameObject.SetActive(false);
        rightDoor.gameObject.SetActive(false);
    }
    public async UniTask ShowLoadingFade(int midSpriteIndex = -1)
    {
        InGameData.GAME_STATE = GameState.Loading;
        doorHitCount = 0;

        leftDoor.ResetDoor();
        rightDoor.ResetDoor();

        leftDoor.OnHitOtherDoor = OnDoorHit;
        rightDoor.OnHitOtherDoor = OnDoorHit;

        leftDoor.gameObject.SetActive(true);
        rightDoor.gameObject.SetActive(true);
        txtBottomBanner.gameObject.SetActive(true);

        leftDoor.StartMoveIn();
        rightDoor.StartMoveIn();

        // chỉ cần 1 cửa báo chạm
        await UniTask.WaitUntil(() => doorHitCount >= 1);

        // STOP cả 2
        leftDoor.Stop();
        rightDoor.Stop();

        await BounceDoors();
        
        txtBottomBanner.DOFade(1, 1f).From(0).SetEase(Ease.OutQuad);
        DOTween.To(()=>0, x=>{
            txtBottomBanner.text = "Loading" + new string('.', x % 4);
        }, 3, 1f).SetLoops(-1).SetEase(Ease.Linear);
        await Task.Delay(1000);
    }
    public async UniTask HideLoadingFade()
    {
        txtBottomBanner.DOFade(0, 0.3f);

        leftDoor.StartMoveOut();
        rightDoor.StartMoveOut();

        // Đợi cho cửa ra khỏi màn hình
        await UniTask.Delay(700);

        leftDoor.Stop();
        rightDoor.Stop();

        leftDoor.gameObject.SetActive(false);
        rightDoor.gameObject.SetActive(false);
        txtBottomBanner.gameObject.SetActive(false);

        InGameData.GAME_STATE = GameState.LoadingDone;
    }
    private void OnDoorHit()
    {
        doorHitCount++;
    }
    private async UniTask BounceDoors()
    {
        float bounce = 0.1f;   // nhỏ hơn để đỡ gắt
        float bounceTime = 0.18f;
        float snapTime   = 0.22f;

        float leftX  = leftDoor.transform.localPosition.x;
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