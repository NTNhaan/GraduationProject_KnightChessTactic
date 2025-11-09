using System;
using System.Collections;
using System.Threading.Tasks;
using Audio;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Data;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine.UI;

public class TutorialController : Singleton<TutorialController>
{
    [SerializeField] private LineCtrl lineCtrl;
    [Header("Tutorial Panel")]
    [SerializeField] private GameObject GuidePanel;
    [SerializeField] private Text textGuide;
    [SerializeField] private SpriteRenderer imageCover;

    [Header("Guide Arrow")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject arrowShadowPrefab;
    [SerializeField] private Transform[] jumpPoints;
    [SerializeField] private Transform player;

    [Header("Hand Anim")]
    [SerializeField] private Animator animHandTouch;
    [SerializeField] private Animator animHandHold;
    [SerializeField] private SpriteRenderer transHandHold;
    [SerializeField] private Animator animHighLighIcon;

    [Header("Guide Fake")]
    [SerializeField] private GameObject guideFake;
    [SerializeField] private GameObject guideGamePlay;
    [SerializeField] private GameObject maskIcon;
    [SerializeField] private ParticleSystem confetiiPartical;

    [Header("Needles")]
    [SerializeField] private SpriteRenderer minuteHandRenderer;
    [SerializeField] private SpriteRenderer hourHandRenderer;
    [SerializeField] private SpriteRenderer centerClockRenderer;

    [SerializeField] private Sprite minuteHand;
    [SerializeField] private Sprite hourHand;
    [SerializeField] private Sprite centerClock;

    [SerializeField] private Sprite minuteHandGlow;
    [SerializeField] private Sprite hourHandGlow;
    [SerializeField] private Sprite centerClockGlow;

    private GameObject currentArrow;
    private GameObject currentArrowShadow;
    private JumpPointType currentPlayerPointType = JumpPointType.Point1;
    private TutorialType currentTutorial;
    private int jumpCount;
    private int MaxJumpCount = 5;
    private bool[] filled = new bool[4];
    private bool isRunning = false;
    private float minScale = 50f;
    private float maxScale = 70f;
    private float duration = 0.5f;
    private Tween pulseTween;
    public static bool checkShowTutorial { get; set; } = false;

    public TutorialType CurrentTutorial
    {
        get { return currentTutorial; }
        set { currentTutorial = value; }
    }
    public bool CheckShowTutorial
    {
        get { return checkShowTutorial; }
        set { checkShowTutorial = value; }
    }

    void OnEnable()
    {
        ScoreController.Instance.OnScoreChanged += HandleScoreChanged;
        EventDispatcher.Register(EventId.OnTutorialScreen, OnTutorialStart);
        EventDispatcher.Register(EventId.OnPlayerJump, OnPlayerJump);
        EventDispatcher.Register(EventId.OnTutorialShow, OnTutorialShow);
        EventDispatcher.Register(EventId.OnTutorialHide, OnTutorialHide);
    }
    void OnDisable()
    {
        ScoreController.Instance.OnScoreChanged -= HandleScoreChanged;
        EventDispatcher.RemoveCallback(EventId.OnTutorialScreen, OnTutorialStart);
        EventDispatcher.RemoveCallback(EventId.OnPlayerJump, OnPlayerJump);
        EventDispatcher.RemoveCallback(EventId.OnTutorialShow, OnTutorialShow);
        EventDispatcher.RemoveCallback(EventId.OnTutorialHide, OnTutorialHide);
    }

    private async void OnTutorialStart(object data = null)
    {
        jumpCount = 0;
        currentTutorial = TutorialType.ShowLine;
        TutorialPanel.Instance.SetTextTutorial(currentTutorial);
        await UniTask.WaitUntil(() => InGameData.GAME_STATE == GameState.Tutorial);
        checkShowTutorial = true;
        lineCtrl.TurnOnLine();
        TutorialPanel.Instance.ShowTutorial();
    }
    private void Update()
    {
        // Debug.Log($"CurrentStep: {currentTutorial}");
        if (currentTutorial == TutorialType.ShowLine && InGameData.GAME_STATE == GameState.Tutorial)
        {
            lineCtrl.StartFillLoop();
        }
        if (currentTutorial == TutorialType.TryJump && !isRunning)
        {
            Debug.Log($"CheckTryJumping 1");
            isRunning = true;
            StartCoroutine(lineCtrl.AvoidObstacleFill());
        }
        if (currentTutorial == TutorialType.EffectMask && !isRunning)
        {
            isRunning = true;
            // EffectMask();
        }

        if (currentTutorial == TutorialType.BreakTime)
        {
            if (Input.GetMouseButtonDown(0) && !isRunning)
            {
                isRunning = true;
                checkShowTutorial = false;

                currentTutorial = TutorialType.HowToPlay;
                ChangeState(currentTutorial);
            }
        }
    }
    public async UniTask ChangeState(TutorialType currentState)
    {
        Debug.Log($"ChangeState: {currentState}");
        switch (currentState)
        {
            case TutorialType.ShowLine:
                animHandTouch.transform.DOScale(0f, .2f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    animHandTouch.gameObject.SetActive(false);
                    lineCtrl.StopFillLoop();
                    lineCtrl.TurnOffLine();
                    StopAllCoroutines();
                });
                Debug.Log("ShowLine 5");
                await Task.Delay(500);

                currentTutorial = TutorialType.Jumping;
                InGameData.GAME_STATE = GameState.PlayingGame;
                SpawnArrowAtNextPoint();
                break;


            case TutorialType.Jumping:
                TutorialPanel.Instance.HideTutorial();
                checkShowTutorial = false;
                await Task.Delay(500);

                currentTutorial = TutorialType.WaitSpawnPoint;

                JumpPointType nextType = GetNextPointType(currentPlayerPointType);
                Transform nextPoint = FindNextPointByType(nextType);

                // PointSpawner.Instance.SpawnPointAt(nextPoint, () =>
                // {
                //     checkShowTutorial = true;
                //     lineCtrl.TurnOnLine();
                //     TutorialPanel.Instance.ShowTutorial(async () =>
                //     {
                //         await UniTask.Delay(1000);
                //         // LinePanel.SetActive(true);
                //         currentTutorial = TutorialType.PointsBonus;
                //     });
                // });
                break;

            case TutorialType.PointsBonus:

                currentTutorial = TutorialType.AvoidObstacle;
                ChangeClockSkin(true);
                checkShowTutorial = true;
                TutorialPanel.Instance.ShowTutorial();

                await UniTask.Delay(5000);
                lineCtrl.TurnOnLine();
                isRunning = false;
                currentTutorial = TutorialType.TryJump;

                imageCover.DOFade(0F, 0.2f);
                imageCover.gameObject.SetActive(false);
                imageCover.sortingOrder = 7;
                TutorialPanel.Instance.SetTextTutorial(currentTutorial);
                break;

            case TutorialType.BreakTime:
                checkShowTutorial = true;
                TutorialPanel.Instance.ShowTutorial();
                InGameData.GAME_STATE = GameState.PauseGame;

                await UniTask.Delay(1000);
                isRunning = false;  // delay 1s for on mouse down
                break;

            case TutorialType.HowToPlay:
                checkShowTutorial = true;
                textGuide.gameObject.SetActive(false);
                guideFake.SetActive(true);

                animHighLighIcon.gameObject.SetActive(true);
                animHighLighIcon.transform.DOScale(1f, .2f).SetEase(Ease.InBack);
                TutorialPanel.Instance.ShowTutorial();
                InGameData.GAME_STATE = GameState.PauseGame;
                isRunning = false;
                break;

            case TutorialType.Completed:
                AudioController.Instance.PlayEffect(Sound.Name.Sound_Reward);
                checkShowTutorial = true;
                TutorialPanel.Instance.ShowTutorial(async () =>
                {
                    await UniTask.Delay(1000);
                    confetiiPartical.gameObject.SetActive(true);
                    AudioController.Instance.PlayEffect(Sound.Name.Sound_Confetti);
                    confetiiPartical.Play();
                });
                InGameData.GAME_STATE = GameState.PauseGame;

                await UniTask.Delay(5000);
                TutorialPanel.Instance.HideTutorial();
                checkShowTutorial = false;

                InGameData.GAME_STATE = GameState.PauseGame;
                InGameData.IS_TUTORIAL_DONE = true;
                DBController.Instance.TUTORIAL_COMPLETED = true;
                await UniTask.Delay(1000);
                // SceneController.Instance?.ChangeScene(SceneType.GamePlayScene);
                break;
        }
    }

    private void HandleScoreChanged(int newScore)
    {
        if (newScore > 0 && currentTutorial == TutorialType.PointsBonus)
        {
            transHandHold.DOFade(0f, .3f).OnComplete(() =>
            {
                animHandHold.gameObject.SetActive(false);

                // TutorialPanel.Instance.HideTutorial();
                // checkShowTutorial = false;

                // currentTutorial = TutorialType.AvoidObstacle;
                ChangeState(currentTutorial);
                InGameData.GAME_STATE = GameState.PlayingGame;
            });
        }
    }

    #region SpawnArrow
    private void SpawnArrowAtNextPoint()
    {
        if (currentArrow != null)
        {
            currentArrow.transform.DOKill();
            Destroy(currentArrow);
        }
        if (currentArrowShadow != null)
        {
            currentArrowShadow.transform.DOKill();
            Destroy(currentArrowShadow);
        }
        JumpPointType nextType = GetNextPointType(currentPlayerPointType);

        Transform nextPoint = FindNextPointByType(nextType);
        if (nextPoint == null)
        {
            Debug.LogWarning($"Không tìm thấy JumpPoint type: {nextType}");
            return;
        }

        Vector3 spawnPos = nextPoint.position;
        if (Vector3.Distance(spawnPos, player.position) < 0.4f)
            spawnPos += Vector3.up * 1f;

        currentArrow = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);

        Vector3 shadowPos = spawnPos - Vector3.up * 0.4f;
        currentArrowShadow = Instantiate(arrowShadowPrefab, shadowPos, Quaternion.identity);
        currentArrowShadow.transform.localScale = Vector3.one * 0.2f;

        currentArrow.transform
            .DOMoveY(spawnPos.y + 0.3f, 0.6f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
        currentArrowShadow.transform
            .DOScale(Vector3.one * 0.1f, 0.6f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
    private Transform FindNextPointByType(JumpPointType type)
    {
        foreach (var point in jumpPoints)
        {
            var jp = point.GetComponent<JumpPoint>();
            if (jp != null && jp.pointType == type)
                return point;
        }
        return null;
    }
    private JumpPointType GetNextPointType(JumpPointType current)
    {
        int index = (int)current + 1;
        if (index > (int)JumpPointType.Point4)
        {
            index = 0;
        }
        return (JumpPointType)index;
    }
    #endregion

    private void OnPlayerJump(object data = null)
    {
        jumpCount++;
        currentPlayerPointType = GetNextPointType(currentPlayerPointType);
        if (checkShowTutorial && currentTutorial == TutorialType.ShowLine)
        {
            ChangeState(currentTutorial);
        }
        else if (currentTutorial == TutorialType.Jumping)
        {
            if (jumpCount >= 4)
            {
                currentArrow.transform.DOKill();
                Destroy(currentArrow);
                currentArrowShadow.transform.DOKill();
                Destroy(currentArrowShadow);
                ChangeState(currentTutorial);
                return;
            }
            SpawnArrowAtNextPoint();
        }
        else if (currentTutorial == TutorialType.TryJump)
        {
            lineCtrl.TurnOffLine();
            ChangeClockSkin(false);

            CurrentTutorial = TutorialType.BreakTime;
            TutorialPanel.Instance.SetTextTutorial(currentTutorial);
            ChangeState(currentTutorial);
        }
    }

    private void OnTutorialShow(object data = null)
    {
        guideFake.SetActive(false);
        TutorialPanel.Instance.HideTutorial();
        // maskIcon.gameObject.SetActive(false);
        animHighLighIcon.gameObject.SetActive(false);
        checkShowTutorial = false;
    }
    private void OnTutorialHide(object data = null)
    {
        CurrentTutorial = TutorialType.Completed;
        ChangeState(currentTutorial);
    }
    public void ChangeClockSkin(bool isGlow)
    {
        if (isGlow)
        {
            minuteHandRenderer.sprite = minuteHandGlow;
            hourHandRenderer.sprite = hourHandGlow;
            centerClockRenderer.sprite = centerClockGlow;
        }
        else
        {
            minuteHandRenderer.sprite = minuteHand;
            hourHandRenderer.sprite = hourHand;
            centerClockRenderer.sprite = centerClock;
        }
    }


    // private void EffectMask()
    // {
    //     if (maskIcon == null) return;
    //     
    //     maskIcon.transform.localScale = Vector3.one * minScale;
    //     
    //     pulseTween = maskIcon.transform
    //         .DOScale(maxScale, duration)
    //         .SetEase(Ease.InOutSine)
    //         .SetLoops(-1, LoopType.Yoyo);
    // }

}
