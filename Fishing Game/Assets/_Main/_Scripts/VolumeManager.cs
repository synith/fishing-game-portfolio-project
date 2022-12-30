using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;

    const string MASTER_AUDIO_GROUP_NAME = "MasterVolume";

    public void ChangeVolume(float value)
    {
        _audioMixer.SetFloat(MASTER_AUDIO_GROUP_NAME, Mathf.Log10(value) * 20);
    }
}
