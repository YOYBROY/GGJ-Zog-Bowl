using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 100f;
    private PauseMenu pauseMenu;
    private FirstPersonController controller;

    void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeed);
        pauseMenu = FindObjectOfType<PauseMenu>();
        controller = FindObjectOfType<FirstPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")) return;
        if (other.gameObject.CompareTag("Player"))
        {
            controller.StartCoroutine("animateDeath");
            //pauseMenu.GameLose();
        }
        Destroy(gameObject);
    }
}
