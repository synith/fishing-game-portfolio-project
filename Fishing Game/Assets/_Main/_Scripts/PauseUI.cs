using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] GameObject _pauseScreen;
    [SerializeField] Button _resumeButton;
    [SerializeField] Button _menuButton;
    [SerializeField] Button _quitButton;

    [SerializeField] AudioClip _buttonSound;

    bool isPaused;

    void Start()
    {
        _pauseScreen.SetActive(false);

        _resumeButton.onClick.AddListener(() => StartCoroutine(ResumeGame()));
        _menuButton.onClick.AddListener(() => StartCoroutine(GoToMainMenu()));
        _quitButton.onClick.AddListener(() => StartCoroutine(QuitToDesktop()));
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

    void PlaySound(AudioClip clip) => SoundEffects.Instance.PlayClip(clip);

    void TogglePause()
    {
        isPaused = !isPaused;
        _pauseScreen.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    IEnumerator ResumeGame() 
    {
        PlaySound(_buttonSound);
        yield return new WaitForSecondsRealtime(0.2f);
        TogglePause();
    }

    IEnumerator GoToMainMenu()
    {
        PlaySound(_buttonSound);
        yield return new WaitForSecondsRealtime(0.2f);
        GameSceneManager.Load(GameSceneManager.Scene.Menu_Scene);
    }

    IEnumerator QuitToDesktop()
    {
        PlaySound(_buttonSound);
        yield return new WaitForSecondsRealtime(0.2f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif 
    }
}
