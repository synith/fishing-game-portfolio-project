using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeUI : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] TextMeshProUGUI text;

    public static event Action<float> OnVolumeChanged;

    const string MUSIC_VOLUME_PREF = "Music_Volume";


    void Awake()
    {
        volumeSlider.onValueChanged.AddListener((value) => { ChangeVolume(value); });
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey(MUSIC_VOLUME_PREF)) return;

        float volume = PlayerPrefs.GetFloat(MUSIC_VOLUME_PREF);
        ChangeVolume(volume);
        volumeSlider.value = volume;
    }

    void ChangeVolume(float value)
    {
        float volumePercent = value * 100;
        text.SetText($"{value * 100:00}");

        PlayerPrefs.SetFloat(MUSIC_VOLUME_PREF, value);
        OnVolumeChanged?.Invoke(value);
    }
}
