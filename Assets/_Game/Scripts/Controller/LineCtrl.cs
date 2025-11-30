using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.Events;

public class LineCtrl : Singleton<LineCtrl>
{
    // [Header("Line Anim")]
    // [SerializeField] private GameObject LinePanel; 
    // [SerializeField] private SpriteRenderer Line1;
    // [SerializeField] private SpriteRenderer Line2;
    // [SerializeField] private SpriteRenderer Line3;
    // [SerializeField] private SpriteRenderer Line4;    
    //
    // private bool[] filled = new bool[4];
    // private bool isRunning = false;
    // private Coroutine fillCoroutine;
    // private bool isCancelled = false;
    //
    // private List<Coroutine> activeFillRoutines = new List<Coroutine>();
    // private void Awake()
    // {
    //     Line1.material = Instantiate(Line1.material);
    //     Line2.material = Instantiate(Line2.material);
    //     Line3.material = Instantiate(Line3.material);
    //     Line4.material = Instantiate(Line4.material);
    // }
    //
    // private void OnDisable()
    // {
    //     StopFillLoop();
    // }
    //
    // #region === FillLine Sequence ===
    // public void StartFillLoop()
    // {
    //     if (!DBController.Instance.TUTORIAL_COMPLETED)
    //     {
    //         if (isRunning || TutorialController.Instance.CurrentTutorial != TutorialType.ShowLine) 
    //             return; 
    //         isCancelled = false; 
    //         isRunning = true;
    //     
    //         ResetAll(false); 
    //         LinePanel.SetActive(true);
    //         fillCoroutine = StartCoroutine(FillLoop());   
    //     }
    //     else
    //     {
    //         if (isRunning) 
    //             return; 
    //         isCancelled = false; 
    //         isRunning = true;
    //         ResetAll(false);
    //         LinePanel.SetActive(true);
    //         fillCoroutine = StartCoroutine(FillLoop());   
    //     }
    // }
    // // public IEnumerator AvoidObstacleFill()
    // // {
    // //     while (TutorialController.Instance.CurrentTutorial == TutorialType.TryJump)
    // //     {
    // //         StartFillLine(2, 1f);
    // //         yield return new WaitForSeconds(2f);
    // //     }
    // //     isRunning = false;
    // // }
    // public IEnumerator AvoidObstacleFill()
    // {
    //     isRunning = true;
    //
    //     while (TutorialController.Instance.CurrentTutorial == TutorialType.TryJump)
    //     {
    //         yield return StartCoroutine(FillSingleLine(Line2, 1f));
    //         yield return new WaitForSeconds(0.2f);
    //         
    //         ResetLine(Line2);
    //         
    //         yield return new WaitForSeconds(0.5f);
    //     }
    //     
    //     ResetLine(Line2);
    //     isRunning = false;
    // }
    //
    //
    // public void StopFillLoop(UnityAction onCompleted = null)
    // {
    //     isCancelled = true;
    //
    //     if (fillCoroutine != null)
    //         StopCoroutine(fillCoroutine);
    //
    //     foreach (var c in activeFillRoutines)
    //         if (c != null) StopCoroutine(c);
    //     activeFillRoutines.Clear();
    //
    //     fillCoroutine = null;
    //     isRunning = false;
    //
    //     ResetAll(true);
    //     LinePanel.SetActive(false);
    //     onCompleted?.Invoke();
    // }
    // private IEnumerator FillLoop()
    // {
    //     // while (!isCancelled)
    //     // {
    //     //     if (TutorialController.Instance.CurrentTutorial != TutorialType.ShowLine)
    //     //     {
    //     //         Debug.Log("LineCtrl: Tutorial changed -> stopping fill");
    //     //         StopFillLoop();
    //     //         yield break;
    //     //     }
    //     //
    //     //     yield return FillAllSequence();
    //     //
    //     //     // Ẩn rồi reset
    //     //     LinePanel.SetActive(false);
    //     //     yield return new WaitForSeconds(0.3f);
    //     //     ResetAll(false);
    //     //     LinePanel.SetActive(true);
    //     // }
    //     //
    //     // isRunning = false;
    //     
    //     while (!isCancelled)
    //     {
    //         if (!DBController.Instance.TUTORIAL_COMPLETED)
    //         {
    //             if (TutorialController.Instance.CurrentTutorial != TutorialType.ShowLine)
    //             {
    //                 Debug.Log("LineCtrl: Tutorial changed -> stopping fill");
    //                 StopFillLoop();
    //                 yield break;
    //             }
    //         }
    //         
    //         yield return FillAllSequence();
    //         LinePanel.SetActive(false);
    //         yield return new WaitForSeconds(0.3f);
    //         ResetAll(false);
    //         LinePanel.SetActive(true);
    //     }
    //
    //     isRunning = false;
    // }
    //
    // private IEnumerator FillAllSequence()
    // {
    //     StartFillLine(1, 0.8f);
    //     yield return new WaitForSeconds(0.8f);
    //     StartFillLine(2, 0.8f);
    //     yield return new WaitForSeconds(0.8f);
    //     StartFillLine(3, 0.8f);
    //     yield return new WaitForSeconds(0.8f);
    //     StartFillLine(4, 0.8f);
    //
    //     yield return new WaitUntil(AllFilled);
    //     yield return new WaitForSeconds(0.3f);
    // }
    //
    // public void StartFillLine(int index, float duration)
    // {
    //     Coroutine c = null;
    //     switch (index)
    //     {
    //         case 1: c = StartCoroutine(FillRoutine(Line1.material, 0, duration)); break;
    //         case 2: c = StartCoroutine(FillRoutine(Line2.material, 1, duration)); break;
    //         case 3: c = StartCoroutine(FillRoutine(Line3.material, 2, duration)); break;
    //         case 4: c = StartCoroutine(FillRoutine(Line4.material, 3, duration)); break;
    //     }
    //     if (c != null) activeFillRoutines.Add(c);
    // }
    //
    // private IEnumerator FillRoutine(Material mat, int i, float duration)
    // {
    //     filled[i] = false;
    //     float t = 0f;
    //     mat.SetFloat("_FillAmount", 0f);
    //
    //     while (t < duration && !isCancelled)
    //     {
    //         if (!DBController.Instance.TUTORIAL_COMPLETED)
    //         {
    //             if (TutorialController.Instance.CurrentTutorial != TutorialType.ShowLine)
    //             {
    //                 Debug.Log("LineCtrl: Tutorial changed mid-fill -> stopping");
    //                 StopFillLoop();
    //                 yield break;
    //             }   
    //         }
    //
    //         t += Time.deltaTime;
    //         float value = Mathf.Clamp01(t / duration);
    //         mat.SetFloat("_FillAmount", value);
    //         yield return null;
    //     }
    //
    //     if (isCancelled) yield break;
    //
    //     mat.SetFloat("_FillAmount", 1f);
    //     filled[i] = true;
    // }
    //
    // private bool AllFilled()
    // {
    //     for (int i = 0; i < filled.Length; i++)
    //         if (!filled[i]) return false;
    //     return true;
    // }
    //
    // public void ResetAll(bool stopRoutine = false)
    // {
    //     if (stopRoutine)
    //     {
    //         StopAllCoroutines();
    //     }
    //     Line1.material.SetFloat("_FillAmount", 0f);
    //     Line2.material.SetFloat("_FillAmount", 0f);
    //     Line3.material.SetFloat("_FillAmount", 0f);
    //     Line4.material.SetFloat("_FillAmount", 0f);
    //
    //     for (int i = 0; i < filled.Length; i++)
    //     {
    //         filled[i] = false;
    //     }
    // }
    //
    // private IEnumerator FillSingleLine(SpriteRenderer line, float duration)
    // {
    //     float time = 0f;
    //     var mat = line.material;
    //     mat.SetFloat("_FillAmount", 0f);
    //
    //     while (time < duration)
    //     {
    //         if (TutorialController.Instance.CurrentTutorial != TutorialType.TryJump)
    //             yield break;
    //
    //         time += Time.deltaTime;
    //         mat.SetFloat("_FillAmount", Mathf.Lerp(0f, 1f, time / duration));
    //         yield return null;
    //     }
    //
    //     mat.SetFloat("_FillAmount", 1f);
    // }
    //
    // private void ResetLine(SpriteRenderer line)
    // {
    //     var mat = line.material;
    //     mat.SetFloat("_FillAmount", 0f);
    // }
    // #endregion
    //
    // public void TurnOnLine() => LinePanel.SetActive(true);
    // public void TurnOffLine() => LinePanel.SetActive(false);
}
