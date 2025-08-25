using UnityEditor;
using UnityEngine;
using System;

public enum SoundType
{
    ZOGCOUGH,
    SODACANPOP,
    CHAMPAIGNPOP,
    ROBOTMOVING,
    SHAKING,
    WEAPONPICKUP,
    GLASSSMASH,
    SODALANDING,
    ZACEYSHOOT,
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    private static AudioManager instance;
    private AudioSource audioSource;

    private AudioSource[] audioSources;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        instance.audioSource.PlayOneShot(randomClip, volume);
    }

    public static void AdjustPitch(float pitch)
    {
        foreach (AudioSource audioSource in instance.audioSources)
        {
            if (audioSource != null)
            {
                audioSource.pitch = pitch;
            }
        }
    }

    public static AudioClip GetAudioClip(SoundType sound)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        return randomClip;
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif
}

[Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}