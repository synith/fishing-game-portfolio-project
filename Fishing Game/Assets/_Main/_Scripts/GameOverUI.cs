using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] Transform _gameOverScreen;
    [SerializeField] Button _menuButton;
    [SerializeField] Button _quitButton;

    [SerializeField] AudioClip _buttonSound;
    [SerializeField] AudioClip _victorySound;
    void Start()
    {
        _menuButton.onClick.AddListener(() => StartCoroutine(ReturnToMenu()));
        _quitButton.onClick.AddListener(() => StartCoroutine(QuitGame()));

        Hide();
    }

    private void OnEnable()
    {
        FishTracker.Instance.OnAllFishCaught += FishTracker_OnAllFishCaught;
    }
    private void OnDisable()
    {
        FishTracker.Instance.OnAllFishCaught -= FishTracker_OnAllFishCaught;
    }

    void FishTracker_OnAllFishCaught()
    {
        StartCoroutine(nameof(PauseAndShowGameOverScreen));       
    }

    IEnumerator PauseAndShowGameOverScreen()
    {
        PlaySound(_victorySound);
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 0f;
        Show();
    }

    IEnumerator ReturnToMenu()
    {
        PlaySound(_buttonSound);
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1f;
        GameSceneManager.Load(GameSceneManager.Scene.Menu_Scene);
    }

    IEnumerator QuitGame()
    {
        PlaySound(_buttonSound);
        yield return new WaitForSecondsRealtime(0.2f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif  
    }

    void PlaySound(AudioClip clip) => SoundEffects.Instance.PlayClip(clip);

    void Hide() => _gameOverScreen.gameObject.SetActive(false);
    void Show() => _gameOverScreen.gameObject.SetActive(true);
}
