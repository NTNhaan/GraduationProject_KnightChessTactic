using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnPointDirection
{

    // public Transform point; 
    public DirectionType direction;
    public int spawnRate;
    public int fallSpeed;
}
public enum DirectionType
{
    CenterTop,     
    CenterDown,
    LeftTop,
    LeftDown,
    RightTop,
    RightDown
}

public enum ObjectType
{
    Black,
    Purple
}