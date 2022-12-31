using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button quitButton;
    [SerializeField] AudioClip buttonSound;
    void Start()
    {
        playButton.onClick.AddListener(() => StartCoroutine(PlayGame()));
        quitButton.onClick.AddListener(() => StartCoroutine(QuitGame()));
    }
    IEnumerator PlayGame()
    {
        SoundEffects.Instance.PlayClip(buttonSound);
        yield return new WaitForSecondsRealtime(0.2f);
        GameSceneManager.Load(GameSceneManager.Scene.Game_Scene);
    }

    IEnumerator QuitGame()
    {
        SoundEffects.Instance.PlayClip(buttonSound);
        yield return new WaitForSecondsRealtime(0.2f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif  
    }
}
