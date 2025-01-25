using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioChoir : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clearAudio;

    void Start()
    {
        audioSource.loop = true;
        audioSource.Play();
    }

    public void ChangeAudioClips()
    {
        float audioTime = audioSource.time;
        audioSource.clip = clearAudio;
        audioSource.loop = true;
        audioSource.Play((ulong)audioTime);
    }
}