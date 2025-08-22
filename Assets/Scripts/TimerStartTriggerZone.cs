using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerStartTriggerZone : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            pauseMenu.StartStopWatch();
        }
    }
}