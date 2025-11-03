using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardItem : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image bannerRank;
    [SerializeField] private Image frameImage;
    [SerializeField] private Text rankText;
    [SerializeField] private Text nameText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timeText;

    [Header("Rank Banners")]
    [SerializeField] private Sprite rank1Banner;
    [SerializeField] private Sprite rank2Banner;
    [SerializeField] private Sprite rank3Banner;
    [SerializeField] private Sprite defaultBanner;

    [Header("Frames")]
    [SerializeField] private Sprite defaultFrame; 
    [SerializeField] private Sprite userFrame;  
    public LeaderBoardData Data { get; private set; }

    public void SetData(LeaderBoardData data)
    {
        Data = data;
        rankText.text = data.rank.ToString();
        nameText.text = data.name;
        scoreText.text = data.score.ToString();
        timeText.text = data.time;
        
        switch (data.rank)
        {
            case 1: bannerRank.sprite = rank1Banner; break;
            case 2: bannerRank.sprite = rank2Banner; break;
            case 3: bannerRank.sprite = rank3Banner; break;
            default: bannerRank.sprite = defaultBanner; break;
        }
        
        if (frameImage != null)
        {
            if (data.name == "You")
                frameImage.sprite = userFrame;
            else
                frameImage.sprite = defaultFrame;
        }
    }
}