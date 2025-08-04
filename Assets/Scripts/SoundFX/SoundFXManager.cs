using Unity.VisualScripting;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance { get; private set; }

    public SoundBank SoundBank;

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
        float pitch = 1f
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
        audioSource.spatialBlend = 1f;
        audioSource.pitch = pitch;
        audioSource.Play();
        Destroy(soundObj, clip.length);
    }

    public AudioSource PlayContinuousSound(
        string clipKey,
        float volume,
        float pitch,
        Vector3 position = default,
        AudioSource audioSource = null
    )
    {
        if (audioSource == null)
        {
            GameObject soundObj = new GameObject("SoundFX");
            audioSource = soundObj.AddComponent<AudioSource>();
            soundObj.transform.SetParent(transform);
            audioSource.clip = SoundBank.GetRandomClip(clipKey);
            audioSource.loop = true;
            audioSource.spatialBlend = 1f;

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

    public void StopContinuousSound(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource.gameObject);
        }
    }
}
