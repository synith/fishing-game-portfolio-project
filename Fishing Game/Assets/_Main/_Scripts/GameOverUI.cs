using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] Transform _gameOverScreen;
    [SerializeField] Button _menuButton;
    [SerializeField] Button _quitButton;
    void Start()
    {
        _menuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            GameSceneManager.Load(GameSceneManager.Scene.Menu_Scene);
        });
        _quitButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif 
        });

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

    private void FishTracker_OnAllFishCaught()
    {
        Time.timeScale = 0f;
        Show();
    }

    void Hide() => _gameOverScreen.gameObject.SetActive(false);
    void Show() => _gameOverScreen.gameObject.SetActive(true);
}
