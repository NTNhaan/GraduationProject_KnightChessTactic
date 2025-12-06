using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpin : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private Text txtValue;
    public void Init(int value, Sprite sprIcon)
    {
        imgIcon.sprite = sprIcon;
        txtValue.text = $"+{value}";

    }
}