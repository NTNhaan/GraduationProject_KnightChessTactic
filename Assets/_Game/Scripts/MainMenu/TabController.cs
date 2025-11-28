using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Data;

public class TabController : Singleton<TabController>
{
    [SerializeField] private List<TabItem> tabButtons;
    [SerializeField] private List<TabContent> tabs;
    [SerializeField] private RectTransform tabContainer;
    [SerializeField] private RectTransform bottomTab;
    private TabType? currentTab = null;
    private TabType previousTab;
    private TabType nextTab;

    private bool isAnimating = false;
    private Tween slideTween;

    void OnEnable()
    {
        EventDispatcher.Register(EventId.OnMainScreen, OnMainScreen);
    }
    void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.OnMainScreen, OnMainScreen);
    }

    private void OnMainScreen(object data = null)
    {
        OnTabSelected(TabType.Home);
    }
    private void Start()
    {
        foreach (var btn in tabButtons)
            btn.Init(OnTabSelected);

        // OnTabSelected(TabType.Home);
    }
    private void OnTabSelected(TabType tab)
    {
        if (isAnimating)
            return;

        int nextIndex = GetTabIndex(tab);
        if (nextIndex < 0)
        {
            return;
        }
        if (!currentTab.HasValue)
        {
            currentTab = tab;

            foreach (var btn in tabButtons)
                btn.SetActiveVisual(btn.Tab == currentTab.Value);

            for (int i = 0; i < tabs.Count; i++)
            {
                bool isActive = i == nextIndex;
                tabs[i].gameObject.SetActive(isActive);
                if (isActive)
                {
                    var rt = tabs[i].GetComponent<RectTransform>();
                    rt.anchoredPosition = Vector2.zero;
                }
            }

            return;
        }

        if (tab == currentTab.Value)
            return;

        previousTab = currentTab.Value;
        nextTab = tab;

        int curIndex = GetTabIndex(previousTab);
        if (curIndex < 0)
        {
            return;
        }

        AnimateTabChange(curIndex, nextIndex).Forget();
    }

    private int GetTabIndex(TabType type)
    {
        for (int i = 0; i < tabs.Count; i++)
            if (tabs[i].Tab == type)
                return i;

        return -1;
    }

    private async UniTask AnimateTabChange(int curIndex, int nextIndex)
    {
        isAnimating = true;
        currentTab = nextTab;
        
        foreach (var btn in tabButtons)
            btn.SetActiveVisual(btn.Tab == currentTab);
        
        slideTween?.Kill();

        RectTransform curPanel = tabs[curIndex].GetComponent<RectTransform>();
        RectTransform nextPanel = tabs[nextIndex].GetComponent<RectTransform>();

        float width = tabContainer.rect.width;
        
        float outPos = nextIndex > curIndex ? -width : width;
        float inPos = nextIndex > curIndex ? width : -width;

        nextPanel.anchoredPosition = new Vector2(inPos, 0);
        nextPanel.gameObject.SetActive(true);
        
        Tween t1 = curPanel.DOAnchorPosX(outPos, 0.28f).SetEase(Ease.OutCubic);
        Tween t2 = nextPanel.DOAnchorPosX(0, 0.28f).SetEase(Ease.OutCubic);

        slideTween = DOTween.Sequence().Join(t1).Join(t2);

        await slideTween.AsyncWaitForCompletion();

        tabs[curIndex].gameObject.SetActive(false);
        isAnimating = false;
    }

    public void ShowBottomTab()
    {
        bottomTab.DOAnchorPosY(0, 0.5f);
    }
    public void HideBottomTab()
    {
        bottomTab.DOAnchorPosY(-250, 0.5f);
    }
}
public enum TabType
{
    Home,
    Shop,
    Leaderboard
}