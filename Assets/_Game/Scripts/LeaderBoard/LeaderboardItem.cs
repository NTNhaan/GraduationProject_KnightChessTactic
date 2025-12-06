using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour
{
    [Header("UI")]
    public Text txtName;
    public Text txtScore;
    public Text txtLevel;

    [Header("Rank UI")]
    public GameObject iconTopRank;
    public Image iconTopRankImg;
    public Sprite sprRank1;
    public Sprite sprRank2;
    public Sprite sprRank3;

    public GameObject iconDefault;
    public Text txtRank;

    /// <summary>
    /// rankIndex = 0-based (0 = top 1)
    /// </summary>
    public void SetData(LeaderboardEntry e, int rankIndex)
    {
        txtName.text = e.playerName;
        txtScore.text = e.score.ToString();
        txtLevel.text = e.level.ToString();

        int rank = rankIndex + 1;

        // TOP 1–3
        if (rank <= 3)
        {
            iconTopRank.SetActive(true);
            iconDefault.SetActive(false);
            txtRank.gameObject.SetActive(false);

            switch (rank)
            {
                case 1:
                    iconTopRankImg.sprite = sprRank1;
                    break;
                case 2:
                    iconTopRankImg.sprite = sprRank2;
                    break;
                case 3:
                    iconTopRankImg.sprite = sprRank3;
                    break;
            }
        }
        else
        {
            // NORMAL RANK
            iconTopRank.SetActive(false);
            iconDefault.SetActive(true);

            txtRank.gameObject.SetActive(true);
            txtRank.text = rank.ToString();
        }
    }
}