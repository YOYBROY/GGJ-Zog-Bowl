using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 100f;
    private PauseMenu pauseMenu;

    void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeed);
        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy")) return;
        if (other.gameObject.CompareTag("Player")) pauseMenu.GameLose();
        Destroy(gameObject);
    }
}
