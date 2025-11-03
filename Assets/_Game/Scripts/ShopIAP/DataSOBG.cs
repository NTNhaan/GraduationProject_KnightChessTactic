using UnityEngine;

[CreateAssetMenu(fileName = "DataSOBG", menuName = "GameData/DataSOBG")]
public class DataSOBG : ScriptableObject
{
    public BGModel[] themes;
}


[System.Serializable]
public class BGModel
{
    [Header("Shop")]
    public string id;
    public Sprite previewImage;
    public Sprite lockImage;
    public Sprite backgroundImage;
    public int price;
    
    [Header("GamePlay")]
    public Sprite background;
    public Sprite clock;
    public Sprite front;
}