using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Audio;
using Data;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using TMPro;

public class LevelController : Singleton<LevelController>
{
    [SerializeField] private LevelData levelData;

    private int exp
    {
        get => DBController.Instance.EXP;
    }

    private int level
    {
        get => DBController.Instance.LEVEL;
    }
    [Header("Exp Bar")]
    [SerializeField] private Image progressBar;

    [SerializeField] private List<Image> stars;
    [SerializeField] private List<Image> starsActice;
    [Range(0f, 1f)][SerializeField] private List<float> starThresholds;

    [SerializeField] private float offset;
    [SerializeField] private Text levelText;
    private int minExp, maxExp;
    private RectTransform progressRect;

    [Header("Congratulation Popup")]
    [SerializeField] private GameObject congratulationPopup;
    [SerializeField] private Image coverCongratulationPopup;
    [SerializeField] private Image effectShine;
    [SerializeField] private Image effectGilter;
    [SerializeField] private Animator starAnim;

    [Header("Level Up Popup")]
    [SerializeField] private GameObject levelUpPopup;
    [SerializeField] private Image coverLevelUpPopup;
    [SerializeField] private Image effectShineBlue;
    [SerializeField] private Image effectGilterBlue;
    [SerializeField] private GameObject effectFirework;
    [SerializeField] private Transform shopBtn;
    [SerializeField] private Image btnNext;

    [SerializeField] private Image coinField;
    [SerializeField] private Text coinShow;
    [SerializeField] private Text coinPlus;

    [Header("Coin Field GamePlay")]
    [SerializeField] private Text coinShopGamePlay;
    [SerializeField] private Transform parentTransform;
    public int Exp => exp;
    public int MaxExp() => maxExp;
    public int MinExp() => minExp;

    private bool isBrickAppear = false;
    private bool isMewAppear = false;

    private float startPos;

    private int currentLevel = 1;
    private int currentCoinPlus = GameConfig.COIN_PLUS;
    private void Start()
    {
        Init();
    }

    void Update()
    {
        float fill = progressBar.fillAmount;

        for (int i = 0; i < starThresholds.Count && i < starsActice.Count; i++)
        {
            // Debug.Log($"CheckProgress: {fill} : {starThresholds[i]}");
            starsActice[i].enabled = fill >= starThresholds[i];
            UpdateStarActive();
        }
    }
    private void Init()
    {
        minExp = levelData.levels[level].min_exp;
        maxExp = levelData.levels[level].max_exp;
        DBController.Instance.EXP = minExp;
        Debug.Log($"LevelInit {level} {DBController.Instance.EXP} {minExp} {maxExp}");
        LevelText(levelText);
        SetupExpStage();
    }

    void UpdateStarActive()
    {
        progressRect = progressBar.GetComponent<RectTransform>();
        for (int i = 0; i < starThresholds.Count && i < stars.Count; i++)
        {
            float ratio = Mathf.Clamp01(starThresholds[i]);
            float width = progressRect.rect.width;


            Vector2 pos = stars[i].rectTransform.anchoredPosition;
            pos.x = width * ratio;
            stars[i].rectTransform.anchoredPosition = pos;
        }
    }
    void SetupExpStage()
    {
        Debug.Log($"[SetupExpStage] Start - Current level: {level}");

        if (levelData == null)
        {
            Debug.LogError("[SetupExpStage] levelData is null!");
            return;
        }

        if (levelData.levels == null || levelData.levels.Count == 0)
        {
            Debug.LogError("[SetupExpStage] levelData.levels is null or empty!");
            return;
        }

        Debug.Log($"[SetupExpStage] Searching in {levelData.levels.Count} levels");
        bool found = false;

        for (int i = 0; i < levelData.levels.Count; i++)
        {
            var levelInfo = levelData.levels[i];
            Debug.Log($"[SetupExpStage] Checking level {levelInfo.level} - min_exp: {levelInfo.min_exp}, max_exp: {levelInfo.max_exp}");

            if (level == levelInfo.level)
            {
                Debug.Log($"[SetupExpStage] Found matching level! min_exp: {levelInfo.min_exp}, max_exp: {levelInfo.max_exp}");
                ChangeStageExp(levelInfo.min_exp, levelInfo.max_exp);
                found = true;
                break;
            }
        }
        UpdatePercent();
    }

    void ChangeStageExp(int minExp, int maxExp)
    {
        Debug.Log($"[ChangeStageExp] Setting exp range - Min: {minExp}, Max: {maxExp}");
        this.minExp = minExp;
        this.maxExp = maxExp;
        Debug.Log($"[ChangeStageExp] After change - minExp: {this.minExp}, maxExp: {this.maxExp}");
    }

    private void UpdatePercent()
    {
        // ExpText(percentExp);
        //CalculateExp(stage, exp_bar);
        CalculateExp();
    }

    public void CalculateExp(RectTransform stage, RectTransform exp_bar)
    {
        float parentWidth = stage.rect.width;
        Vector2 centerPos = stage.anchoredPosition;
        centerPos.x -= parentWidth / 2;

        var newWidth = (exp - minExp) * parentWidth / (maxExp - minExp);
        centerPos.x += newWidth - parentWidth / 2;

        exp_bar.DOAnchorPosX(centerPos.x + offset, 1);
    }

    public void CalculateExp()
    {
        Debug.Log($"CalculateExp: {exp} {minExp} {maxExp}");
        float percent = (exp - minExp) * 1f / (maxExp - minExp);
        progressBar.DOFillAmount(percent, 0.5f).From(progressBar.fillAmount);
    }

    public void UpdateExp(int exp)
    {
        var userCoin = DBController.Instance.COIN;
        // GamePlayController.isLevelUp = false;
        if (!DBController.Instance.TUTORIAL_COMPLETED)
        {
            // ProgressController.Instance.ShowPointValueText();
        }
        int _exp = this.exp;

        _exp += exp;
        if (_exp >= maxExp)
        {
            LevelUp();
            ShowCongratulationPopup();
            LevelText(levelText);
            SaveNewLevel(_exp);
            SetupExpStage();
            RealLevel();

            userCoin = DBController.Instance.COIN;
            // Debug.Log($"CheckCoinPlus: {userCoin} {currentCoinPlus}");
            DBController.Instance.COIN_PER_LEVEL += currentCoinPlus;
        }
        else
        {
            SaveNewLevel(_exp);
            UpdatePercent();
        }
    }
    void RealLevel()
    {
        var userInfo = DBController.Instance.EXP;
        if (userInfo == 0)
        {
            return;
        }

        var realLvl = 1;
        for (int i = 0; i < levelData.levels.Count; i++)
        {
            if (levelData.levels[i].min_exp <= userInfo &&
                userInfo < levelData.levels[i].max_exp)
            {
                realLvl = levelData.levels[i].level;
                break;
            }
        }
    }

    void SaveNewLevel(int exp)
    {
        var userExp = DBController.Instance.EXP;
        if (userExp == exp)
        {
            return;
        }
        DBController.Instance.EXP = exp;
    }


    void LevelUp()
    {
        InGameData.GAME_STATE = GameState.LevelUp;
        InGameData.NextLevel = true;
        EventManager.LevelUpEvent(); // event for stop all obstacle
        Debug.Log($"CheckLevelUpUser: {level}");
        int lvl = level;
        var userCoinLevel = DBController.Instance.COIN_PER_LEVEL;

        lvl++;
        DBController.Instance.LEVEL = lvl;
        Debug.Log($"CheckCoinPlus LevelUp: {userCoinLevel} {currentCoinPlus}");
        if (currentCoinPlus >= userCoinLevel)
            DBController.Instance.COIN_PER_LEVEL = currentCoinPlus;
    }

    private void LevelText(Text level)
    {
        //var leveTxt = this.level.GetDecrypted();
        level.text = $"{this.level}"; // Level text
    }

    public void ExpText(Text exp)
    {
        exp.text = $"{this.exp - minExp}/{maxExp - minExp}";
    }

    public void IncreaseLevel()
    {
        // TutorialPanel.Instance.HideTutorial();
        var level = DBController.Instance.LEVEL + 1;
        if (level >= levelData.levels.Count)
            return;
        DBController.Instance.LEVEL = level;
        DBController.Instance.EXP = levelData.levels[level].min_exp;
        Debug.Log($"[CheatingLevel] Increase Lvl {level}");
        SetupExpStage();
        CalculateExp();
        LevelText(levelText);
        InGameData.UseBooster = true;

        InGameData.GAME_STATE = GameState.Loading;
        InGameData.NEXT_STATE = GameState.SelectSkin;
        // InGameData.GAME_SCENE = SceneType.GamePlayScene;
        // SceneController.Instance.ChangeScene(SceneType.GamePlayScene);
    }

    public void DecreaseLevel()
    {
        // TutorialPanel.Instance.HideTutorial();
        var level = DBController.Instance.LEVEL - 1;
        if (level < 0)
            return;
        DBController.Instance.LEVEL = level;
        DBController.Instance.EXP = levelData.levels[level].min_exp;
        Debug.Log($"[CheatingLevel] Decrease Lvl {level}");

        SetupExpStage();
        CalculateExp();
        LevelText(levelText);
        InGameData.UseBooster = true;

        InGameData.GAME_STATE = GameState.Loading;
        InGameData.NEXT_STATE = GameState.SelectSkin;
        // InGameData.GAME_SCENE = SceneType.GamePlayScene;
        // SceneController.Instance.ChangeScene(SceneType.GamePlayScene);
    }
    public void ShowCongratulationPopup()
    {
        Debug.Log($"CheckLevelUpUser: ShowPopup");
        coverCongratulationPopup.gameObject.SetActive(true);
        congratulationPopup.gameObject.SetActive(true);
        coverCongratulationPopup.DOFade(1f, .5f).OnComplete(() =>
        {
            congratulationPopup.transform.DOScale(Vector3.one, .5f).OnComplete(() =>
            {
                effectShine.DOFade(.5f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                effectGilter.DOFade(.5f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                starAnim.SetBool("isFill", true);

                DOVirtual.DelayedCall(2f, () =>
                {
                    effectShine.DOFade(0f, 0.5f);
                    effectGilter.DOFade(0f, 0.5f);
                    starAnim.SetBool("isFill", false);
                    ShowLevelPopup();
                    congratulationPopup.transform.DOScale(Vector3.zero, .3f).OnComplete(() =>
                    {
                        coverCongratulationPopup.DOFade(0f, .5f).OnComplete(() =>
                        {
                            coverCongratulationPopup.gameObject.SetActive(false);
                            congratulationPopup.gameObject.SetActive(false);
                        });
                    });
                });
            });
        });
    }
    public void ShowLevelPopup()
    {
        var coinUser = DBController.Instance.COIN;

        coverLevelUpPopup.gameObject.SetActive(true);
        levelUpPopup.SetActive(true);
        coverLevelUpPopup.DOFade(1f, .5f).OnComplete(() =>
        {
            levelUpPopup.transform.DOScale(1f, 0.3f).OnComplete(() =>
            {
                // var oldCoin = coinUser - currentCoinPlus;
                // coinPlus.text = "+" + currentCoinPlus.ToString("N0");
                // coinShow.text = oldCoin.ToString("N0");
                var cointmp = 0;
                coinPlus.text = "+" + cointmp.ToString("N0");
                effectShineBlue.DOFade(1f, 0.5f);
                effectGilterBlue.DOFade(1f, 0.5f);
                effectFirework.SetActive(true);
                coinField.DOFade(1f, 1f).OnComplete(() =>
                {
                    DOTween.To(() => cointmp, x => cointmp = x, GameConfig.COIN_PLUS, 1)
                        .OnUpdate(() => { coinPlus.text = "+" + cointmp.ToString("N0"); })
                        .OnComplete(() =>
                        {
                            btnNext.DOFade(1f, 1f).SetEase(Ease.Linear);
                        });
                });
            });
        });
    }

    public void HideLevelPopup()
    {
        EffectHide();
    }
    async UniTask EffectHide()
    {
        effectShineBlue.DOFade(0f, 0.5f);
        effectGilterBlue.DOFade(0f, 0.5f);
        effectFirework.SetActive(false);
        btnNext.DOFade(0f, 0.5f).SetEase(Ease.Linear);
        await Task.Delay(500);
        // levelUpPopup.transform.DOScale(0f, .5f).OnComplete(() =>
        // {
        //     coverLevelUpPopup.DOFade(0f, .5f).OnComplete(() =>
        //     {
        //         AnimatorHelper.Instance.ObjectFly(Vector3.zero, 10, shopBtn, parentTransform);
        //         UpdateCoinText(GameConfig.COIN_PLUS);
        //         levelUpPopup.SetActive(false);
        //         coverLevelUpPopup.gameObject.SetActive(false);
        //         SceneController.Instance.ChangeScene(SceneType.GamePlayScene);
        //         // if (level <= 5)
        //         // {
        //         //     SceneController.Instance.ChangeScene(SceneType.GamePlayScene);
        //         // }
        //     });
        // });
    }
    public void UpdateCoinText(int coin)
    {
        var oldCoin = DBController.Instance.COIN;
        var coinUser = oldCoin + coin;
        Debug.Log($"CheckCoinPlus: curr: {oldCoin} coinPlus: {coinUser}");
        DOTween.To(() => oldCoin, x => oldCoin = x, coinUser, 2)
            .OnUpdate(() => { coinShopGamePlay.text = oldCoin.ToString("N0"); })
            .OnComplete(() =>
            {
                if (InGameData.GAME_STATE == GameState.UseBooster)
                {
                    return;
                }
                DBController.Instance.COIN = coinUser;
                // InGameData.GAME_STATE = GameState.ResumeGame;
                // EventManager.ResumeGame();
            });
    }
}