using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button quitButton;
    void Start()
    {
        playButton.onClick.AddListener(() => 
        {
            GameSceneManager.Load(GameSceneManager.Scene.Game_Scene);
        });
        quitButton.onClick.AddListener(() =>
        {
            QuitGame();
        });

    }
    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif  
    }
}
