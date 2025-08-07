using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance { get; private set; }

    public SoundBank SoundBank;

    public float stoppingLoopTime = 0.1f;
    private float audioStereoSpread = 40f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() { }

    void Update() { }

    public void PlaySFX(
        string clipKey,
        Vector3 position = default,
        float volume = 1f,
        float pitch = 1f,
        float minAudioDistance = 10f,
        float spatialBlend = 1f
    )
    {
        AudioClip clip = SoundBank.GetRandomClip(clipKey);
        if (clip == null)
        {
            Debug.LogWarning($"Sound key doesn't exist: {clipKey}");
            return;
        }

        GameObject soundObj = new GameObject("SoundFX");
        soundObj.transform.SetParent(transform);
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        audioSource.transform.position = position;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = spatialBlend;
        audioSource.pitch = pitch;
        audioSource.spread = audioStereoSpread;

        audioSource.minDistance = minAudioDistance;
        audioSource.Play();
        Destroy(soundObj, clip.length);
    }

    public AudioSource PlayContinuousSound(
        string clipKey,
        float volume = 1f,
        float pitch = 1f,
        Vector3 position = default,
        AudioSource audioSource = null,
        float spatialBlend = 1f,
        float minAudioDistance = 10f
    )
    {
        if (audioSource == null)
        {
            GameObject soundObj = new GameObject("SoundFX");
            audioSource = soundObj.AddComponent<AudioSource>();
            soundObj.transform.SetParent(transform);
            audioSource.clip = SoundBank.GetRandomClip(clipKey);
            audioSource.loop = true;
            audioSource.spatialBlend = spatialBlend;
            audioSource.minDistance = minAudioDistance;
            audioSource.dopplerLevel = 0f;
            audioSource.spread = audioStereoSpread;

            audioSource.transform.position = position;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.Play();
        }
        else
        {
            audioSource.transform.position = position;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
        }

        return audioSource;
    }

    public void UpdateContinuousSound(
        AudioSource audioSource,
        float volume = 1f,
        float pitch = 1f,
        Vector3 position = default
    )
    {
        if (position != default)
        {
            audioSource.transform.position = position;
        }
        audioSource.volume = volume;
        audioSource.pitch = pitch;
    }

    public void StopContinuousSound(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            StartCoroutine(StopSoundCoroutine(audioSource));
        }
    }

    private System.Collections.IEnumerator StopSoundCoroutine(AudioSource audioSource)
    {
        float initialVolume = audioSource.volume;
        if (initialVolume <= 0f)
        {
            audioSource.Stop();
            Destroy(audioSource.gameObject);
            yield break;
        }
        else
        {
            for (float t = 0; t < stoppingLoopTime; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(initialVolume, 0f, t / stoppingLoopTime);
                yield return null;
            }
            audioSource.Stop();
            Destroy(audioSource.gameObject);
        }
    }
}
