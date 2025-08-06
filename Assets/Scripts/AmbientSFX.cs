using System.Collections;
using UnityEngine;

public class AmbientSFX : MonoBehaviour
{
    public AudioSource audioSource;
    private float currentVolume = 0f;
    private float startVolume = 0f;
    private float targetVolume = 0f;

    public float minAudioVolume = 0.1f;
    public float maxAudioVolume = 0.3f;

    public float volumeTransitionTime = 3f;
    public float volumeStayTime = 3f;
    private float timer = 0f;

    private const float PITCH = 1f;
    private const float SPATIAL_BLEND = 0f;

    void Awake()
    {
        currentVolume = (minAudioVolume + maxAudioVolume) / 2f;
        timer = 0f;
        startVolume = currentVolume;
        targetVolume = Random.Range(minAudioVolume, maxAudioVolume);
    }

    void Start()
    {
        audioSource.volume = currentVolume;
        if (audioSource != null)
        {
            SoundFXManager.Instance.PlayContinuousSound(
                "Ambient",
                currentVolume,
                PITCH,
                transform.position,
                audioSource,
                SPATIAL_BLEND
            );
        }
    }

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer <= volumeTransitionTime)
        {
            currentVolume = Mathf.SmoothStep(
                startVolume,
                targetVolume,
                timer / volumeTransitionTime
            );
        }
        if (timer > volumeTransitionTime + volumeStayTime)
        {
            timer = 0f;
            startVolume = currentVolume;
            targetVolume = Random.Range(minAudioVolume, maxAudioVolume);
        }
        SoundFXManager.Instance.UpdateContinuousSound(audioSource, currentVolume, PITCH);
    }
}
