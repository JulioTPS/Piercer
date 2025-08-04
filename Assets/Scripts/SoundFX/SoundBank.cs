using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundBank", menuName = "Scriptable Objects/SoundBank")]
public class SoundBank : ScriptableObject
{
    [System.Serializable]
    public class SoundEntry
    {
        public string key;
        public AudioClip[] clips;
    }

    public SoundEntry[] soundEntries;

    private Dictionary<string, AudioClip[]> soundEntriesdDictionary;

    void OnEnable()
    {
        soundEntriesdDictionary = new Dictionary<string, AudioClip[]>();
        foreach (var entry in soundEntries)
        {
            soundEntriesdDictionary[entry.key] = entry.clips;
        }
    }

    public AudioClip GetRandomClip(string key)
    {
        if (soundEntriesdDictionary.TryGetValue(key, out var clips) && clips.Length > 0)
        {
            return clips[Random.Range(0, clips.Length)];
        }
        return null;
    }
}
