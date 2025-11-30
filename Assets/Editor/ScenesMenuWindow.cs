using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ScenesMenuWindow : EditorWindow
{
    [MenuItem("Scenes/Open Scenes List %#e")] // Ctrl + Shift + E
    public static void OpenWindow()
    {
        GetWindow<ScenesMenuWindow>("Scenes");
    }

    void OnGUI()
    {
        GUILayout.Label("Scenes in Build Settings", EditorStyles.boldLabel);

        var scenes = EditorBuildSettings.scenes;

        if (scenes.Length == 0)
        {
            EditorGUILayout.HelpBox("No scenes found in Build Settings!", MessageType.Warning);
            return;
        }

        foreach (var scene in scenes)
        {
            string path = scene.path;
            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            if (GUILayout.Button(name, GUILayout.Height(25)))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(path);
                }
            }
        }
    }
}