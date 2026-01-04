using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Data;

public interface ISceneController
{
    UniTask ChangeScene(SceneType _sceneType, int midSpriteIndex, UnityAction onCompleteFade = null);
}
public class SceneController : Singleton<SceneController>, ISceneController
{
    [SerializeField] private Image imgFadeUI;
    public UnityAction callBackLoadScreen;
    [SerializeField] private SceneType previousScene;
    [SerializeField] private SceneType currentScene;

    public SceneType PreviousScene => previousScene;
    public SceneType CurrentScene => currentScene;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Start()
    {
        Debug.Log($"CheckInitStart");
        Application.targetFrameRate = 60;
        currentScene = previousScene = SceneType.MainScene;

        imgFadeUI.gameObject.SetActive(true);
        imgFadeUI.DOFade(1, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            imgFadeUI.DOFade(0, 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                imgFadeUI.gameObject.SetActive(false);
            });
        });
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        callBackLoadScreen?.Invoke();
        callBackLoadScreen = FadeOutBlackScreen;
    }

    public async UniTask ChangeScene(SceneType _sceneType, int midSpriteIndex = -1, UnityAction onCompleteFade = null)
    {
        Debug.Log("ChangeScene 1");
        previousScene = currentScene;
        currentScene = _sceneType;
        Debug.Log("ChangeScene 2");
        /*        imgFadeUI.gameObject.SetActive(true);
                imgFadeUI.DOFade(0, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    onCompleteFade?.Invoke();
                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        SceneManager.LoadScene($"{currentScene}");
                    });
                });
        */
        Debug.Log("ChangeScene 3");
        await LoadingFade.Instance.ShowLoadingFade();
        Debug.Log("ChangeScene 4");
        await Task.Delay(2000);
        Debug.Log("ChangeScene 5");
        SceneManager.LoadScene($"{currentScene}");
        Debug.Log("ChangeScene 6");
        await Task.Delay(2000);
        Debug.Log("ChangeScene 7");
        await LoadingFade.Instance.HideLoadingFade();
        Debug.Log("ChangeScene 8");
    }

    public void FadeOutBlackScreen()
    {
        imgFadeUI.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(() => { imgFadeUI.gameObject.SetActive(false); });
    }


}
public enum SceneType
{
    LoadingScene = 0,
    MainScene = 1,
    GameScene = 2,
}