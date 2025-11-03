using Data;
using UnityEngine;

public static class InGameData
{
    public static SceneType GAME_SCENE = SceneType.MainScene;
    public static GameState PRE_STATE = GameState.None;
    public static GameState GAME_STATE = GameState.None;
    public static GameState NEXT_STATE = GameState.None;
    
    public static bool RestartGame = false;
    public static bool NextLevel = false;
    public static bool UseBooster = false;
    public static float baseInterval = 3f;
    public static bool IS_TUTORIAL_DONE = false;
    public static int GIVE_UP_COUNT = 0;
}