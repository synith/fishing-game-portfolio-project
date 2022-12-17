using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] GameObject _pauseScreen;
    [SerializeField] Button _resumeButton;
    [SerializeField] Button _menuButton;
    [SerializeField] Button _quitButton;

    bool isPaused;

    void Start()
    {
        _pauseScreen.SetActive(false);

        _resumeButton.onClick.AddListener(ResumeGame);
        _menuButton.onClick.AddListener(GoToMainMenu);
        _quitButton.onClick.AddListener(QuitToDesktop);
    }

    void OnEnable()
    {
        FishingControls.Instance.OnPausePressed += FishingControls_OnPausePressed;
    }

    void FishingControls_OnPausePressed(object sender, System.EventArgs e)
    {
        TogglePause();
    }

    void OnDisable()
    {
        FishingControls.Instance.OnPausePressed -= FishingControls_OnPausePressed;
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        _pauseScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    void ResumeGame() 
    {
        TogglePause();
    }

    void GoToMainMenu()
    {
        GameSceneManager.Load(GameSceneManager.Scene.Menu_Scene);
    }

    void QuitToDesktop()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif 
    }
}
