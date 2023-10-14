using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusic : MonoBehaviour
{
    public static BGMusic instance { get; private set; }
    [SerializeField] public float pauseVolume;
    [SerializeField] float fadeDuration = 2.0f; // Duration of the fade effect

    [HideInInspector] public AudioSource audioSource;
    private float startVolume;

    [HideInInspector] public bool isMuted = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.transform.parent.gameObject);
            return;
        }
        instance = this;
        audioSource = GetComponent<AudioSource>();
        SetAudioSource();
    }

    public void MuteToggle()
    {
        StopAllCoroutines();
        isMuted = !isMuted;
        BGMusic.instance.audioSource.volume = BGMusic.instance.isMuted == true ? 0.0f : BGMusic.instance.pauseVolume;
        audioSource.volume = isMuted == true ? 0.0f : BGMusic.instance.pauseVolume;
        GameManager.instance.SaveBGMusic_Setting(isMuted);
    }

    // Call this method to start a fade from the current volume to the target volume
    public void BGPauseToggle(bool onPause)
    {
        // Get the initial volume from the AudioSource component
        startVolume = audioSource.volume;
        if (!isMuted)
        {
            if (onPause)
            { StartCoroutine(FadeVolume(pauseVolume)); }
            else
            { StartCoroutine(FadeVolume(1.0f)); }
        }
    }

    private System.Collections.IEnumerator FadeVolume(float targetVolume)
    {
        float startTime = Time.time;

        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            float newVolume = Mathf.Lerp(startVolume, targetVolume, t);

            // Set the new volume on the AudioSource component
            audioSource.volume = newVolume;

            yield return null;
        }

        // Ensure the volume reaches the exact target value
        audioSource.volume = targetVolume;
        startVolume = targetVolume;
    }

    private void SetAudioSource()
    {
        isMuted = PlayerPrefs.GetInt("IsMutedBG", 0) == 1 ? true : false;
        audioSource.volume = isMuted ? 0.0f : 1.0f;
    }
}
