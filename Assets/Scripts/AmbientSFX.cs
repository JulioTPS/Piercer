using UnityEngine;

public class AmbientSFX : MonoBehaviour
{
    public AudioSource ambientAudioSource;
    private float currentVolume = 0f;
    private float startVolume = 0f;
    private float targetVolume = 0f;

    public float minAudioVolume = 0.1f;
    public float maxAudioVolume = 0.3f;

    public float volumeTransitionTime = 3f;
    public float volumeStayTime = 3f;
    private float timer = 0f;

    public float BGMVolume = 0.2f;
    public float BGMInterval = 360f;
    private float timerBGM = 0f;

    private const float MIN_AUDIO_DISTANCE = 0f;
    private const float PITCH = 1f;
    private const float SPATIAL_BLEND = 0f;

    void Awake()
    {
        currentVolume = (minAudioVolume + maxAudioVolume) / 2f;
        timer = 0f;
        startVolume = currentVolume;
        targetVolume = Random.Range(minAudioVolume, maxAudioVolume);

        timerBGM = 60f;
    }

    void Start()
    {
        if (ambientAudioSource != null)
        {
            ambientAudioSource = SoundFXManager.Instance.PlayContinuousSound(
                "Ambient",
                currentVolume,
                PITCH,
                transform.position,
                ambientAudioSource,
                SPATIAL_BLEND
            );
        }
        ambientAudioSource.volume = currentVolume;
    }

    void Update()
    {
        float unscaledDeltaTime = Time.unscaledDeltaTime;
        timer += unscaledDeltaTime;
        timerBGM += unscaledDeltaTime;
        if (timer <= volumeTransitionTime)
        {
            currentVolume = Mathf.SmoothStep(
                startVolume,
                targetVolume,
                Mathf.SmoothStep(0f, 1f, Mathf.Sqrt(timer / volumeTransitionTime))
            );
        }
        if (timer > volumeTransitionTime + volumeStayTime)
        {
            timer = 0f;
            startVolume = currentVolume;
            targetVolume = Random.Range(minAudioVolume, maxAudioVolume);
        }
        SoundFXManager.Instance.UpdateContinuousSound(ambientAudioSource, currentVolume, PITCH);

        if (timerBGM > BGMInterval)
        {
            timerBGM = 0f;
            SoundFXManager.Instance.PlaySFX(
                "BGM",
                transform.position,
                BGMVolume,
                PITCH,
                MIN_AUDIO_DISTANCE,
                SPATIAL_BLEND
            );
        }
    }
}
