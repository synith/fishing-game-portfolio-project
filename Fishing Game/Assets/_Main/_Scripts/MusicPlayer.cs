using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] _musicTracks;
    VolumeManager volumeManager;

    AudioSource audioSource;
    bool isFading;
    float fadeTime = 2f;
    int currentTrack = -1;
    float volumeLevel;

    float maxVolume = 0.5f;

    const string MUSIC_VOLUME_PREF = "Music_Volume";

    void Awake()
    {
        volumeManager = GetComponent<VolumeManager>();
        audioSource = GetComponent<AudioSource>();
        currentTrack = GetRandomTrack();
    }

    void OnEnable()
    {
        if (FindObjectOfType<MusicVolumeUI>() != null)
        {
            MusicVolumeUI.OnVolumeChanged += MusicVolumeUI_OnVolumeChanged;
        }
        else if (PlayerPrefs.HasKey(MUSIC_VOLUME_PREF))
        {
            float volume = PlayerPrefs.GetFloat(MUSIC_VOLUME_PREF);
            ChangeAudioMixerVolume(volume);
        }
    }

    void MusicVolumeUI_OnVolumeChanged(float volume) => ChangeAudioMixerVolume(volume);

    void OnDisable()
    {
        if (FindObjectOfType<MusicVolumeUI>() != null)
        {
            MusicVolumeUI.OnVolumeChanged -= MusicVolumeUI_OnVolumeChanged;
        }
    }

    public void PlayRandomTrack() => PlayMusic(GetRandomTrack());

    public void PlayNextTrack() => PlayMusic(GetNextTrack());

    void ChangeAudioMixerVolume(float volume)
    {
        volumeManager.ChangeVolume(volume);
    }

    void ChangeAudioSourceVolume(float volume)
    {
        volumeLevel = volume * maxVolume;
        audioSource.volume = volumeLevel;
    }

    int GetRandomTrack() => Random.Range(0, _musicTracks.Length);

    int GetNextTrack() => (currentTrack + 1) % _musicTracks.Length;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayNextTrack();
        }

        if (!audioSource.isPlaying)
        {
            PlayNextTrack();
        }

        if (!isFading) return;
        audioSource.volume -= volumeLevel * Time.deltaTime / fadeTime;
    }

    void PlayMusic(int trackNumber)
    {
        if (trackNumber == currentTrack) return;

        if (audioSource.isPlaying)
        {
            StartCoroutine(FadeOut(trackNumber));
        }
        else
        {
            currentTrack = trackNumber;
            audioSource.clip = _musicTracks[trackNumber];
            audioSource.Play();
        }
    }

    IEnumerator FadeOut(int trackNumber)
    {
        float startVolume = audioSource.volume;

        isFading = true;

        yield return new WaitUntil(() => audioSource.volume < 0.1f);
        isFading = false;

        audioSource.Stop();
        audioSource.volume = startVolume;
        volumeLevel = audioSource.volume;

        currentTrack = trackNumber;
        audioSource.clip = _musicTracks[trackNumber];
        audioSource.Play();
    }
}
