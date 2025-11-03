using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class LeaderBoardController : Singleton<LeaderBoardController>
{
    [Header("UI References")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform contentParent; 
    [SerializeField] private LeaderBoardItem itemPrefab;
    [SerializeField] private float rankMoveDuration = 0.8f;

    private List<LeaderBoardItem> _spawnedItems = new List<LeaderBoardItem>();
    private List<LeaderBoardData> _fakeData = new List<LeaderBoardData>();
    private List<LeaderBoardData> _fakeDataOld = new List<LeaderBoardData>();
    private List<LeaderBoardData> _fakeDataNew = new List<LeaderBoardData>();

    void OnEnable()
    {
        InitFakeData();
        _fakeData = new List<LeaderBoardData>(_fakeDataOld);
        RefreshLeaderBoard();
    }
    
    void InitFakeData()
    {
        _fakeDataOld.Clear();
        _fakeDataOld.Add(new LeaderBoardData(1, "Liam", 9500, "10:02"));
        _fakeDataOld.Add(new LeaderBoardData(2, "Emma", 8800, "09:15"));
        _fakeDataOld.Add(new LeaderBoardData(3, "Noah", 7900, "08:44"));
        _fakeDataOld.Add(new LeaderBoardData(4, "Olivia", 7200, "07:32"));
        _fakeDataOld.Add(new LeaderBoardData(5, "Ava", 6500, "06:21"));
        _fakeDataOld.Add(new LeaderBoardData(6, "Sophia", 5700, "05:10"));
        _fakeDataOld.Add(new LeaderBoardData(7, "Mason", 3000, "04:05"));
        
        int oldScore = Mathf.Max(DBController.Instance.BEST_SCORE - 1000, 0);
        _fakeDataOld.Add(new LeaderBoardData(8, "You", oldScore, System.DateTime.Now.ToString("HH:mm")));
    }
    
    void RefreshLeaderBoard()
    {
        foreach (var i in _spawnedItems)
            Destroy(i.gameObject);
        _spawnedItems.Clear();
        
        _fakeData.Sort((a, b) => b.score.CompareTo(a.score));
        for (int i = 0; i < _fakeData.Count; i++)
            _fakeData[i].rank = i + 1;
        
        foreach (var data in _fakeData)
        {
            var item = Instantiate(itemPrefab, contentParent);
            item.SetData(data);
            _spawnedItems.Add(item);
        }
    }
    
    
    public async UniTask TryAnimateUserClimb()
    {
        var user = _fakeData.Find(x => x.name == "You");
        if (user == null) return;

        int oldScore = _fakeDataOld.Find(x => x.name == "You")?.score ?? 0;
        int newScore = DBController.Instance.BEST_SCORE;
        
        if (newScore <= oldScore)
        {
            Debug.Log("[LeaderBoard] No new high score → skip climb");
            return;
        }
        user.score = newScore;
        _fakeData.Sort((a, b) => b.score.CompareTo(a.score));
        for (int i = 0; i < _fakeData.Count; i++)
            _fakeData[i].rank = i + 1;

        RefreshLeaderBoard();
        await AnimateUserClimb_Smooth();
    }
    private async UniTask AnimateUserClimb_Smooth()
{
    // 1️⃣ Lấy user item
    var userItem = _spawnedItems.Find(i => i.Data.name == "You");
    if (userItem == null) return;

    var scrollRect = contentParent.GetComponentInParent<ScrollRect>();
    RectTransform contentRect = scrollRect.content;
    RectTransform userRect = userItem.GetComponent<RectTransform>();

    int currentIndex = _spawnedItems.IndexOf(userItem);
    int targetIndex = GetUserTargetRank();

    if (targetIndex >= currentIndex)
    {
        Debug.Log("[LeaderBoard] User already at correct position");
        return;
    }

    Debug.Log($"[LeaderBoard] User climbs from {currentIndex + 1} → {targetIndex + 1}");
    
    await ScrollToUser();
    await UniTask.Delay(400);
    
    Transform oldParent = userRect.parent;
    Vector3 worldPos = userRect.position;
    userRect.SetParent(scrollRect.transform.parent, true);
    userRect.position = worldPos;
    userRect.SetAsLastSibling();
    
    await userItem.transform.DOScale(1.3f, 0.45f)
        .SetEase(Ease.OutBack)
        .SetUpdate(true)
        .AsyncWaitForCompletion();
    
    RectTransform targetRect = _spawnedItems[targetIndex].GetComponent<RectTransform>();
    Vector3 targetWorldPos = targetRect.TransformPoint(Vector3.zero);

    float moveDuration = 1.8f;
    
    await UniTask.WhenAll(
        SmoothScrollTo(scrollRect, targetRect, moveDuration),
        userItem.transform.DOMove(targetWorldPos, moveDuration)
            .SetEase(Ease.InOutCubic)
            .SetUpdate(true)
            .AsyncWaitForCompletion()
            .AsUniTask()
    );
    
    userRect.SetParent(oldParent, true);
    userRect.SetSiblingIndex(targetIndex);

    await userItem.transform.DOScale(1f, 0.45f)
        .SetEase(Ease.OutBack)
        .SetUpdate(true)
        .AsyncWaitForCompletion();

    RefreshLeaderBoard();
    await ScrollToUser();
}

    private async UniTask SmoothScrollTo(ScrollRect scrollRect, RectTransform targetRect, float duration = 1f)
    {
        RectTransform contentRect = scrollRect.content;
        RectTransform viewportRect = scrollRect.viewport;

        float contentHeight = contentRect.rect.height;
        float viewportHeight = viewportRect.rect.height;
        float itemPosY = Mathf.Abs(targetRect.anchoredPosition.y);

        float targetPosY = Mathf.Clamp(itemPosY - viewportHeight / 2f, 0, contentHeight - viewportHeight);
        float normalizedY = contentHeight <= viewportHeight
            ? 1f
            : 1f - (targetPosY / (contentHeight - viewportHeight));

        float start = scrollRect.normalizedPosition.y;
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0, 1, time / duration);
            float newY = Mathf.Lerp(start, normalizedY, t);
            scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, newY);
            await UniTask.Yield();
        }
        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, normalizedY);
    }
    int GetUserTargetRank()
    {
        int userScore = DBController.Instance.BEST_SCORE;
        for (int i = 0; i < _fakeData.Count; i++)
        {
            if (userScore > _fakeData[i].score)
                return i;
        }
        return _fakeData.Count - 1;
    }
    public async UniTask ScrollToUser()
    {
        var userItem = _spawnedItems.Find(i => i.Data.name == "You");
        if (userItem == null) return;

        var scrollRect = contentParent.GetComponentInParent<ScrollRect>();
        if (scrollRect == null) return;

        await UniTask.Yield();
        Canvas.ForceUpdateCanvases();

        RectTransform contentRect = scrollRect.content;
        RectTransform viewportRect = scrollRect.viewport;
        RectTransform userRect = userItem.GetComponent<RectTransform>();

        float contentHeight = contentRect.rect.height;
        float viewportHeight = viewportRect.rect.height;
        float itemPosY = Mathf.Abs(userRect.anchoredPosition.y);

        float targetPosY = Mathf.Clamp(itemPosY - viewportHeight / 2f, 0, contentHeight - viewportHeight);
        float normalizedY = contentHeight <= viewportHeight
            ? 1f
            : 1f - (targetPosY / (contentHeight - viewportHeight));

        normalizedY = Mathf.Clamp01(normalizedY);

        var tween = DOTween.To(
            () => scrollRect.normalizedPosition,
            pos => scrollRect.normalizedPosition = new Vector2(pos.x, normalizedY),
            new Vector2(scrollRect.normalizedPosition.x, normalizedY),
            1f
        ).SetEase(Ease.OutCubic);
        
        bool dragging = false;
        scrollRect.onValueChanged.AddListener(_ => dragging = true);

        while (tween.active && !tween.IsComplete())
        {
            if (dragging)
            {
                tween.Kill();
                break;
            }
            await UniTask.Yield();
        }

        scrollRect.onValueChanged.RemoveAllListeners();
    }

    #region Cheating
    [ContextMenu("Cheat: +1000 to You")]
    private void CheatAdd1000()
    {
        DBController.Instance.BEST_SCORE += 1000;
        Debug.Log("[Cheat] BEST_SCORE increased by 1000 -> " + DBController.Instance.BEST_SCORE);
        RefreshAfterCheat();
    }
    
    [ContextMenu("Cheat: Make You Top")]
    private void CheatMakeTop()
    {
        int topScore = 0;
        foreach (var d in _fakeData) if (d.score > topScore) topScore = d.score;
        DBController.Instance.BEST_SCORE = topScore + 100;
        Debug.Log("[Cheat] BEST_SCORE set to top -> " + DBController.Instance.BEST_SCORE);
        RefreshAfterCheat();
    }
    
    [SerializeField] private int cheatSetScoreValue = 12345;
    [ContextMenu("Cheat: Set You Score (Inspector value)")]
    private void CheatSetScoreInspector()
    {
        DBController.Instance.BEST_SCORE = cheatSetScoreValue;
        Debug.Log("[Cheat] BEST_SCORE set -> " + DBController.Instance.BEST_SCORE);
        RefreshAfterCheat();
    }
    
    [ContextMenu("Cheat: Reset Fake Data")]
    private void CheatResetFake()
    {
        InitFakeData();
        RefreshLeaderBoard();
        ScrollToUser();
        Debug.Log("[Cheat] Fake data reset");
    }

    private void RefreshAfterCheat()
    {
        var user = _fakeData.Find(x => x.name == "You");
        if (user != null) user.score = DBController.Instance.BEST_SCORE;
        RefreshLeaderBoard();
        
        // UniTask.DelayedCall(10, () => ScrollToUser()).Forget();
    }
    #endregion
}