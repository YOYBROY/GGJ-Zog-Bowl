using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerStartTriggerZone : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;

    [SerializeField] private AudioSource audioSource;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pauseMenu.StartStopWatch();
            audioSource.Play();
            Destroy(gameObject);
        }
    }
}