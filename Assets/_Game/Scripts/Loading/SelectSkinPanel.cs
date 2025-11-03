using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
public class SelectSkinPanel : Singleton<SelectSkinPanel>
{
    [Header("Banner Select Skin")]
    [SerializeField] private Image panelSelectSkin;
    [SerializeField] private Image imgCover;
    [SerializeField] private Image btnNext;
    
    [Header("Dog Skin")]
    [SerializeField] private Image imgDog;
    [SerializeField] private SkinUI[] dogSkins;
    private int currentDogSkin;

    [Header("Cat Skin")]
    [SerializeField] private Image imgCat;
    [SerializeField] private SkinUI[] catSkins;
    private int currentCatSkin;
    
    [Header("Anim Select Skin")]
    [SerializeField] private Animator animSelect;
    private async UniTaskVoid Start()
    {
        InitSKin();
        animSelect.enabled = true;
        animSelect.SetBool("isClick", true);
    }
    private void InitSKin()
    {
        currentDogSkin = DBController.Instance.LoadDataByKey<int>(DBKey.DOG_SKIN);
        currentCatSkin = DBController.Instance.LoadDataByKey<int>(DBKey.CAT_SKIN);

        if (dogSkins != null && dogSkins.Length > 0)
            imgDog.sprite = dogSkins[currentDogSkin].skinImg;
        if (catSkins != null && catSkins.Length > 0)
            imgCat.sprite = catSkins[currentCatSkin].skinImg;
    }
    
    public void ShowPanelSelectSkin()
    {
        // GamePlayController.isSelectSkin = true;
        InGameData.GAME_STATE = GameState.SelectSkin;
        
        imgCover.gameObject.SetActive(true);
        panelSelectSkin.gameObject.SetActive(true);
        imgCover.DOFade(.9f, .5f).OnComplete(() =>
        {
            panelSelectSkin.transform.DOScale(1f, .5f);
            btnNext.DOFade(1f, 0.2f).From(0f).SetEase(Ease.OutQuad); 
        });
    }
    public void HidePanelSelectSkin()
    {
        btnNext.DOFade(0f, 0.5f).From(1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            panelSelectSkin.transform.DOScale(0f, .5f).OnComplete(() =>
            {
                imgCover.DOFade(0f, .5f);
                imgCover.gameObject.SetActive(false);
                panelSelectSkin.gameObject.SetActive(false);
                // GamePlayController.isSelectSkin = false;
                InGameData.GAME_STATE = GameState.Counting;
            });
        });
    }
    
    #region old Function
    public void NextDogSkin()
    {
        Debug.Log($"[Reskin] NextDogSkin");
        currentDogSkin++;
        if (currentDogSkin >= dogSkins.Length)
            currentDogSkin = 0;
        imgDog.sprite = dogSkins[currentDogSkin].skinImg;
    }

    public void PrevDogSkin()
    {
        Debug.Log($"[Reskin] PreDogSkin");
        currentDogSkin--;
        if (currentDogSkin < 0)
            currentDogSkin = dogSkins.Length - 1;
        imgDog.sprite = dogSkins[currentDogSkin].skinImg;
    }

    public void NextMewSkin()
    {
        Debug.Log($"[Reskin] NextMewSkin");
        currentCatSkin++;
        if (currentCatSkin >= catSkins.Length)
            currentCatSkin = 0;
        imgCat.sprite = catSkins[currentCatSkin].skinImg;
    }
    public void PrevMewSkin()
    {
        Debug.Log($"[Reskin] PreDogSkin");
        currentCatSkin--;
        if (currentCatSkin < 0)
            currentCatSkin = catSkins.Length - 1;
        imgCat.sprite = catSkins[currentCatSkin].skinImg;
    }
    

    #endregion

    public void OnClickSelectDogSkin(int index)
    {
        SelectDogSkin(index);
    }
    public void OnClickSelectCatSkin(int index)
    {
        SelectCatSkin(index);
    }
    public async UniTask SelectDogSkin(int index)
    {
        animSelect.SetBool("isClick", false);
        await AnimatorHelper.Instance.WaitForStateComplete(animSelect, "OffTutorial");
        if (index >= 0 && index < dogSkins.Length)
        {
            currentDogSkin = index;

            imgDog.sprite = dogSkins[currentDogSkin].skinImg;
            
            foreach (var skin in dogSkins)
                skin.hightlight.SetActive(false);
            
            dogSkins[currentDogSkin].hightlight.SetActive(true);
            Debug.Log($"[Reskin] Selected Dog Skin {currentDogSkin}");
        }
    }
    
    public async UniTask SelectCatSkin(int index)
    {
        animSelect.SetBool("isClick", false);
        await AnimatorHelper.Instance.WaitForStateComplete(animSelect, "OffTutorial");
        if (index >= 0 && index < catSkins.Length)
        {
            currentCatSkin = index;

            imgCat.sprite = catSkins[currentCatSkin].skinImg;
            
            foreach (var skin in catSkins)
                skin.hightlight.SetActive(false);

            catSkins[currentCatSkin].hightlight.SetActive(true);
            Debug.Log($"[Reskin] Selected Cat Skin {currentCatSkin}");
        }
    }
    public void OnClickNext()
    {
        DBController.Instance.DOG_SKIN = currentDogSkin;
        DBController.Instance.CAT_SKIN = currentCatSkin;

        Debug.Log($"[Reskin] Saved DOG skin = {currentDogSkin}, CAT skin = {currentCatSkin}");
        EventManager.ChangeSkin();
        HidePanelSelectSkin();
    }
}

[Serializable]
public class SkinData
{
    public CharacterType SkinType;
    public Sprite SkinImage;
}
[Serializable]
public class CharacterData
{
    public CharacterType characterType;
    public int currentSkin;       
}
