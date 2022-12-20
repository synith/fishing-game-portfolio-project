using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] _musicTracks;

    bool _isFading;
    float _fadeTime = 2f;
    AudioSource _audioSource;
    int _currentTrack = -1;
    float _volumeLevel;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _volumeLevel = _audioSource.volume;
    }

    public void PlayNextTrack()
    {
        PlayMusic(GetNextTrack());
    }

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
