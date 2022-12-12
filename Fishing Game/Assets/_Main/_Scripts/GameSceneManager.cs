using System;
using UnityEngine.SceneManagement;

public static class GameSceneManager
{
    public enum Scene
    {
        Game_Scene,
        Menu_Scene,
    }

    public static void Load(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
}
