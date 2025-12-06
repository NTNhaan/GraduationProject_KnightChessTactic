using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class LeaderBoardScreen : ScreenBase
{
    [Header("Tabs")]
    [SerializeField] private RectTransform levelPanel;
    [SerializeField] private RectTransform endlessPanel;

    [Header("Covers")]
    [SerializeField] private GameObject coverLevel;
    [SerializeField] private GameObject coverEndless;

    [Header("Anim Settings")]
    [SerializeField] private float moveDistance = 0f; 
    [SerializeField] private float animTime = 0.25f;
    
    [Header("Country Tabs")]
    [SerializeField] private GameObject coverWorld;
    [SerializeField] private GameObject coverVietNam;
    private bool isLevelTab = true;
    void Start()
    {
        ShowLevelTab();
    }
    public void OnClickLevel()
    {
        if (isLevelTab) return;
        isLevelTab = true;
        levelPanel.SetAsLastSibling();
        LeaderboardController.Instance.SetMode(LeaderboardMode.Level);
        ShowLevelTab();
        HideEndlessTab();
    }
    
    public void OnClickEndLess()
    {
        if (!isLevelTab) return;
        isLevelTab = false;
        endlessPanel.SetAsLastSibling();
        LeaderboardController.Instance.SetMode(LeaderboardMode.Endless);
        ShowEndlessTab();
        HideLevelTab();
    }

    public void OnClickWorld()
    {
        coverWorld.SetActive(false);
        coverVietNam.SetActive(true);
        LeaderboardController.Instance.SetCountry(LeaderboardCountry.World);
        Debug.Log("Selected WORLD");
    }

    public void OnClickVietNam()
    {
        coverVietNam.SetActive(false);
        coverWorld.SetActive(true);
        LeaderboardController.Instance.SetCountry(LeaderboardCountry.Vietnam);
        Debug.Log("Selected VIETNAM");
    }
    private void ShowLevelTab()
    {
        levelPanel.DOAnchorPosY(moveDistance, animTime).SetEase(Ease.OutQuad);
        coverLevel.SetActive(false);
    }

    private void HideLevelTab()
    {
        levelPanel.DOAnchorPosY(0, animTime).SetEase(Ease.OutQuad);
        coverLevel.SetActive(true);
    }

    private void ShowEndlessTab()
    {
        endlessPanel.DOAnchorPosY(moveDistance, animTime).SetEase(Ease.OutQuad);
        coverEndless.SetActive(false);
    }

    private void HideEndlessTab()
    {
        endlessPanel.DOAnchorPosY(0, animTime).SetEase(Ease.OutQuad);
        coverEndless.SetActive(true);
    }
}