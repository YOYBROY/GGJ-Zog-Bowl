using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class AnimEventManager : MonoBehaviour
{
    [SerializeField] private AudioSource ObjectAudioSource;
    [SerializeField] private AudioClip ClickSound;
    [SerializeField] private AudioClip ElevatorDoors;
    [SerializeField] private AudioClip ElevatorBell;
    [SerializeField] private AudioClip ElevatorWhirr;
    [SerializeField] private AudioClip ElevatorRumble;

    [SerializeField] private AudioMixer myAudioMixer;

    AudioSource aud;

    void Start()
    {
        aud = GetComponent<AudioSource>();
        
    }

    void Update()


    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlayClickSound();
        }

    }
    public void DisableObject()
    {
        gameObject.SetActive(false);
    }

    public void EnableObject()
    {
        gameObject.SetActive(true);
    }

    public void PlayClickSound()
    {
        if (ClickSound == null)
        {
            return;
        }
        else
        {
            Debug.Log("You are clicking");
            ObjectAudioSource.PlayOneShot(ClickSound);
        }
    }


    public void PlayBellSound()
    {
        ObjectAudioSource.PlayOneShot(ElevatorBell);
    }
    public void PlayDoorSound()
    {
        ObjectAudioSource.PlayOneShot(ElevatorDoors);
    }
    public void PlayRumbleSound()
    {
        ObjectAudioSource.PlayOneShot(ElevatorRumble);
    }
    public void PlayWhirrSound()
    {
        ObjectAudioSource.PlayOneShot(ElevatorWhirr);
    }

    public void PlaySoundClip()
    {
        ObjectAudioSource.Play();
    }

    public void AdjustVolume(float value)
    {
        myAudioMixer.SetFloat("MasterAudio", Mathf.Log10(value) * 20);
    }


}
