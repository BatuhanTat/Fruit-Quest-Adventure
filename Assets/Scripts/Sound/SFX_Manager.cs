using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Manager : MonoBehaviour
{
    public static SFX_Manager instance { get; private set; }

    [Header("Sound Effects")]
    [SerializeField] AudioClip buttonClick_SFX;
    [SerializeField] AudioClip match3_SFX;
    [SerializeField] AudioClip match4_SFX;
    [SerializeField] AudioClip levelComplete_SFX;
    [SerializeField] AudioClip levelLose_SFX;

    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public bool isMuted = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(transform.parent);
        audioSource = GetComponent<AudioSource>();
        SetAudioSource();
    }

    public void MuteToggle()
    {
        audioSource.volume = audioSource.volume == 0.0f ? 1.0f : 0.0f;
        isMuted = !isMuted;
        GameManager.instance.SaveSFX_Setting(isMuted);
    }

    public void PlaySFX(ClipType type, float volume = 1)
    {
        var clip = GetClip(type);
        audioSource.PlayOneShot(clip, volume);
    }

    private AudioClip GetClip(ClipType type)
    {
        return type switch
        {
            ClipType.Click => buttonClick_SFX,
            ClipType.Match3 => match3_SFX,
            ClipType.Match4 => match4_SFX,
            ClipType.LevelComplete => levelComplete_SFX,
            ClipType.LevelLose => levelLose_SFX,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private void SetAudioSource()
    {
        isMuted = PlayerPrefs.GetInt("IsMutedSFX", 0) == 1 ? true : false;
        audioSource.volume = isMuted ? 0.0f : 1.0f;
    }
}





public enum ClipType
{
    Click,
    Match3,
    Match4,
    LevelComplete,
    LevelLose,
}
