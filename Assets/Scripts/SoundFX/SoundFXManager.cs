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

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.transform.position = position;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1f;
        audioSource.pitch = pitch;
        audioSource.Play();
        Destroy(audioSource, clip.length);
    }
}
