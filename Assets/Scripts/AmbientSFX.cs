using System.Collections;
using UnityEngine;

public class AmbientSFX : MonoBehaviour
{
    public AudioSource audioSource;
    private float currentVolume = 1f;
    public float minAudioVolume = 0.1f;
    public float maxAudioVolume = 0.5f;

    public float volumeUpdateTime = 3f;
    private float timer = 0f;

    private Coroutine currentVolumeCoroutine;

    private const float PITCH = 1f;
    private const float SPATIAL_BLEND = 0f;

    void Start()
    {
        currentVolume = minAudioVolume;
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
        if (timer >= volumeUpdateTime)
        {
            if (currentVolumeCoroutine != null)
                StopCoroutine(currentVolumeCoroutine);

            currentVolumeCoroutine = StartCoroutine(UpdateAudioVolume());
            timer = 0f;
        }
    }

    private IEnumerator UpdateAudioVolume()
    {
        float targetVolume = Random.Range(minAudioVolume, maxAudioVolume);
        float startVolume = currentVolume;
        for (float t = 0; t < volumeUpdateTime; t += Time.unscaledDeltaTime)
        {
            currentVolume = Mathf.Lerp(startVolume, targetVolume, t / volumeUpdateTime);
            SoundFXManager.Instance.UpdateContinuousSound(audioSource, currentVolume, PITCH);
            yield return null;
        }
    }

    // private float SimulateWindSpeed()
    // {
    //     float dt = Time.deltaTime;
    //     float theta = 0.5f; // Return rate to mean
    //     float mu = 6f; // Long-term average wind speed
    //     float sigma = 2f; // Noise intensity (gustiness)
    //     float noise = Random.Range(-1f, 1f);

    //     windSpeed += theta * (mu - windSpeed) * dt + sigma * Mathf.Sqrt(dt) * noise;
    // }
}
