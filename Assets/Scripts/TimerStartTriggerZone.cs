using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TimerStartTriggerZone : MonoBehaviour
{
    [SerializeField] private float lerpDuration;
    [SerializeField] private PauseMenu pauseMenu;

    [SerializeField] private AudioSource audioSource;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pauseMenu.StartStopWatch();

            StartCoroutine("VolumeLerp");

        }
    }

    private IEnumerator VolumeLerp()
    {
        float timer = 0.0f;
        while (timer < lerpDuration)
        {
            audioSource.volume = Mathf.Lerp(0, 0.08f, timer / lerpDuration);

            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}