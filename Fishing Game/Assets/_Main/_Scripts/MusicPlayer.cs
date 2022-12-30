using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] _musicTracks;
    VolumeManager _volumeManager;

    AudioSource _audioSource;
    bool _isFading;
    float _fadeTime = 2f;
    int _currentTrack = -1;
    float _volumeLevel;

    float _maxVolume = 0.5f;

    const string MUSIC_VOLUME_PREF = "Music_Volume";

    void Awake()
    {
        _volumeManager = GetComponent<VolumeManager>();
        _audioSource = GetComponent<AudioSource>();
        _currentTrack = GetRandomTrack();
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
        _volumeManager.ChangeVolume(volume);
    }

    void ChangeAudioSourceVolume(float volume)
    {
        _volumeLevel = volume * _maxVolume;
        _audioSource.volume = _volumeLevel;
    }

    int GetRandomTrack() => Random.Range(0, _musicTracks.Length);

    int GetNextTrack() => (_currentTrack + 1) % _musicTracks.Length;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayNextTrack();
        }

        if (!_audioSource.isPlaying)
        {
            PlayNextTrack();
        }

        if (!_isFading) return;
        _audioSource.volume -= _volumeLevel * Time.deltaTime / _fadeTime;
    }

    void PlayMusic(int trackNumber)
    {
        if (trackNumber == _currentTrack) return;

        if (_audioSource.isPlaying)
        {
            StartCoroutine(FadeOut(trackNumber));
        }
        else
        {
            _currentTrack = trackNumber;
            _audioSource.clip = _musicTracks[trackNumber];
            _audioSource.Play();
        }
    }

    IEnumerator FadeOut(int trackNumber)
    {
        float startVolume = _audioSource.volume;

        _isFading = true;

        yield return new WaitUntil(() => _audioSource.volume < 0.1f);
        _isFading = false;

        _audioSource.Stop();
        _audioSource.volume = startVolume;
        _volumeLevel = _audioSource.volume;

        _currentTrack = trackNumber;
        _audioSource.clip = _musicTracks[trackNumber];
        _audioSource.Play();
    }
}
