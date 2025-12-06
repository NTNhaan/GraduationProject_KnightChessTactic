using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "KnightChessTactic/ExpBarData", fileName = "exp_level_data")]
public class ExpLevelData : ScriptableObject
{
    public ExpData[] expDataArray;

    [ContextMenu("Auto Generate Exp Data")]
    void AutoGenerateExpData()
    {
        this.expDataArray = new ExpData[60];
        for (int i = 0; i < this.expDataArray.Length; i++)
        {
            this.expDataArray[i] = new ExpData();
            this.expDataArray[i].lvl = i + 1;
            this.expDataArray[i].minExp = GetExpAtLevel(i);
            this.expDataArray[i].maxExp = GetExpAtLevel(i + 1);
        }
    }

    int GetExpAtLevel(int level)
    {
        int EXP_BASE = 10000;
        return (level * (level + 1)) / 2 * EXP_BASE;
    }
    [ContextMenu("ConvertToJson")]
    public void ConvertToJson()
    {
        string jsonData = JsonUtility.ToJson(this, true);
        System.IO.File.WriteAllText(Application.dataPath + "/exp_level_data.json", jsonData);
        Debug.Log("Data converted to JSON and saved at: " + Application.dataPath);
    }

}

[Serializable]
public struct ExpData
{
    public int lvl;
    public int minExp;
    public int maxExp;
}