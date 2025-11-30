
using System;
using System.Collections;
using Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialPanel : Singleton<TutorialPanel>
{
    // [Header("Tutorial Panel")]
    // [SerializeField] private Text textCountDown;
    // [SerializeField] private Text textTutorial;
    // [SerializeField] private GameObject panelGameTutorial;
    // [SerializeField] private SpriteRenderer imageCover;
    //
    //
    // [Header("Hand Anim")]
    // [SerializeField] private Animator animHandTouch;
    // [SerializeField] private Animator animHandHold;
    // [SerializeField] private SpriteRenderer transHandHold;
    // private bool isCountingDown;
    // private TutorialType currentTutorial;
    //
    // private void Start()
    // {
    //     currentTutorial = TutorialController.Instance.CurrentTutorial;
    // }
    //
    // public void ShowTutorial(UnityAction onComplete = null)
    // {
    //     currentTutorial = TutorialController.Instance.CurrentTutorial;
    //     if (!DBController.Instance.TUTORIAL_COMPLETED)
    //     {
    //         InitData();
    //         panelGameTutorial.SetActive(true);
    //         panelGameTutorial.transform.DOScale(1f, .2f).OnComplete(() =>
    //         {
    //             SetTextTutorial(currentTutorial);
    //             if (currentTutorial == TutorialType.ShowLine)
    //             {
    //                 animHandTouch.transform.DOScale(1f, .3f).SetEase(Ease.OutBack);
    //             }
    //             else if (currentTutorial == TutorialType.WaitSpawnPoint)
    //             {
    //                 animHandHold.gameObject.SetActive(true);
    //                 transHandHold.DOFade(1f, .3f);
    //             }
    //             else if (currentTutorial == TutorialType.None && !DBController.Instance.TUTORIAL_COMPLETED)
    //                 return;
    //             else
    //             {
    //                 imageCover.DOFade(0.9843137f, .5f).OnComplete(() =>
    //                 {
    //                     imageCover.gameObject.SetActive(true);
    //                     if (currentTutorial == TutorialType.AvoidObstacle)
    //                     {
    //                         imageCover.sortingOrder = 1;
    //                     }
    //                 });
    //             }
    //             SetTextTutorial(currentTutorial);
    //             onComplete?.Invoke();
    //         });
    //     }
    //     else if (DBController.Instance.GUIDE_BOOSTER == 0 && !GamePlayController.Instance.CheckLineState && DBController.Instance.TUTORIAL_COMPLETED)
    //     {
    //         panelGameTutorial.SetActive(true);
    //         panelGameTutorial.transform.DOScale(1f, .2f).OnComplete(() =>
    //         {
    //             imageCover.DOFade(0.9843137f, .5f).OnComplete(() =>
    //             {
    //                 imageCover.gameObject.SetActive(true);
    //                 textTutorial.gameObject.SetActive(true);
    //                 textTutorial.text = "Use the booster to freeze clock for 5 seconds!";
    //                 onComplete?.Invoke();
    //             });
    //         });
    //     }
    //     else
    //     {
    //         SetTextTutorial(currentTutorial);
    //         panelGameTutorial.SetActive(true);
    //         panelGameTutorial.transform.DOScale(1f, .2f);
    //     }
    // }
    // public void HideTutorial()
    // {
    //     if (!DBController.Instance.TUTORIAL_COMPLETED)
    //     {
    //         animHandTouch.transform.DOScale(0f, .3f).OnComplete(() =>
    //         {
    //             animHandTouch.gameObject.SetActive(false);
    //             textTutorial.gameObject.SetActive(false);
    //             imageCover.DOFade(0f, .2f);
    //             panelGameTutorial.transform.DOScale(0f, .2f).SetEase(Ease.InBack).OnComplete(() =>
    //             {
    //                 panelGameTutorial.SetActive(false);
    //                 imageCover.gameObject.SetActive(false);
    //             });
    //         });
    //     }
    //     else
    //     {
    //         animHandTouch.transform.DOScale(0f, .3f).OnComplete(() =>
    //         {
    //             animHandTouch.gameObject.SetActive(false);
    //             textTutorial.gameObject.SetActive(false);
    //             imageCover.DOFade(0f, .2f);
    //             panelGameTutorial.transform.DOScale(0f, .2f).SetEase(Ease.InBack).OnComplete(() =>
    //             {
    //                 panelGameTutorial.SetActive(false);
    //                 imageCover.gameObject.SetActive(false);
    //             });
    //         });
    //     }
    // }
    // void InitData()
    // {
    //     if (TutorialController.Instance.CurrentTutorial == TutorialType.ShowLine)
    //     {
    //         imageCover.gameObject.SetActive(false);
    //         textTutorial.gameObject.SetActive(true);
    //         animHandTouch.gameObject.SetActive(true);
    //     }
    // }
    // public void SetTextTutorial(TutorialType currentTutorial)
    // {
    //     switch (currentTutorial)
    //     {
    //         case TutorialType.ShowLine:
    //             textTutorial.text = "Tap to jump for point";
    //             break;
    //         case TutorialType.WaitSpawnPoint:
    //             textTutorial.text = "Collect for extra points";
    //             break;
    //         case TutorialType.AvoidObstacle:
    //             textTutorial.text = "Avoid obstacles by timing your jump";
    //             break;
    //         case TutorialType.TryJump:
    //             textTutorial.text = "Tap Now";
    //             break;
    //         case TutorialType.BreakTime:
    //             textTutorial.text = "Survive for as long as possible for even more points";
    //             break;
    //         case TutorialType.Completed:
    //             textTutorial.text = "Tutorial Complete";
    //             break;
    //         default:
    //             textTutorial.text = "Tap to start!!!";
    //             break;
    //     }
    //
    //     if (currentTutorial != TutorialType.HowToPlay)
    //     {
    //         textTutorial.gameObject.SetActive(true);
    //     }
    // }
    // IEnumerator StartCountDown()
    // {
    //     textCountDown.gameObject.SetActive(true);
    //
    //     int count = 3;
    //     while (count > 0)
    //     {
    //         textCountDown.text = count.ToString();
    //         yield return new WaitForSeconds(1f);
    //         count--;
    //     }
    //
    //     textCountDown.text = "GO!";
    //     yield return new WaitForSeconds(1f);
    //     textCountDown.gameObject.SetActive(false);
    //
    //     InGameData.GAME_STATE = GameState.PlayingGame;
    //     panelGameTutorial.transform.DOScale(0f, .5f).OnComplete(() =>
    //     {
    //         panelGameTutorial.SetActive(false);
    //         isCountingDown = false;
    //     });
    // }
}
