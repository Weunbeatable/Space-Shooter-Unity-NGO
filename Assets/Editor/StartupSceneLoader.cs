using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class StartupSceneLoader 
{
    static StartupSceneLoader()
    {
        EditorApplication.playModeStateChanged += LoadStartupScene;
    }

    private static void LoadStartupScene(PlayModeStateChange state)
    {
        // read state to do something different if entering or exiting play mode
        if(state == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if(state == PlayModeStateChange.EnteredEditMode)
        {
            if(EditorSceneManager.GetActiveScene().buildIndex != 0)
            {
                EditorSceneManager.LoadScene(0);
            }
        }
    }
}
