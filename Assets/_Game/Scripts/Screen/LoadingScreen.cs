using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class LoadingScreen : ScreenBase
{
    [Header("Loading Scene Variables")]
    [SerializeField] private Image progressBar;
    [SerializeField] private Text textPercent;
    private static string NEXT_SCENE_NAME = SceneType.MainScene.ToString();
    private float fixedTime = 3f;
    
    private void OnEnable()
    {
        // EventManager.OnInitData += OnInitData;
    }
    private void OnDisable()
    {
        // EventManager.OnInitData -= OnInitData;
    }
    private void Start()
    {
        LoadingSceneAsync(NEXT_SCENE_NAME);
        // OnInitData();
    }
    private async UniTaskVoid LoadingSceneAsync(string sceneName)
    {
        progressBar.DOFillAmount(1, 2f).SetEase(Ease.Linear).From(0);
        
        await DOVirtual.Int(0, 100, 2f, (X) =>
        {
            textPercent.text = X.ToString()+$"%";
        }).ToUniTask();
        // SceneManager.LoadScene(NEXT_SCENE_NAME);
        SceneController.Instance.ChangeScene(SceneType.MainScene);
    }
    void OnInitData()
    {
        Debug.Log("Initializing data before loading the next scene...");
    }
}