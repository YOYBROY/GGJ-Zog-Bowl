using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class ZaceyEnemy : MonoBehaviour
{
    [SerializeField] Transform[] patrolPoints;
    Transform targetPoint;
    int patrolNumber = 0;
    [SerializeField] float patrolLerpSpeed = 2f;

    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float distanceThreshold = 0.01f;

    private GameObject player;
    private GunController gunController;

    [SerializeField] float stunThreshold = 500f;

    [SerializeField] float attackRange = 10f;
    float timer;
    [SerializeField] float attackTimer = 4f;

    [SerializeField] Transform attackSpawnPoint;
    [SerializeField] GameObject enemyProjectile;

    float dazedCount;
    [SerializeField] float dazedTimer = 2f;

    [SerializeField] bool ignoreRayAgro;

    [SerializeField] GameObject topHalf;
    [SerializeField] Transform topHalfLocator;

    private AudioSource audioSource;
    [SerializeField] private AudioClip robotMoveAudio;

    private PauseMenu pauseMenu;

    [SerializeField] ParticleSystem deathParticles;
    private Animator animator;

    enum EnemyState { PATROLLING, ATTACKING, APPROACHING };
    EnemyState enemyState;

    void Start()
    {
        animator = transform.parent.GetComponentInChildren<Animator>();
        pauseMenu = FindObjectOfType<PauseMenu>();
        player = FindObjectOfType<FirstPersonController>().gameObject;
        gunController = FindObjectOfType<GunController>();
        targetPoint = patrolPoints[0];
        timer = attackTimer;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = AudioManager.GetAudioClip(SoundType.ROBOTMOVING);
        audioSource.loop = true;
        audioSource.volume = Mathf.Clamp(moveSpeed, 0, 1);
        audioSource.Play();
    }

    private void Update()
    {
        if (dazedCount > 0)
        {
            dazedCount -= Time.deltaTime;
            return;
        }
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        Vector3 direction = player.transform.position - transform.position;

        if (distanceToPlayer < attackRange)
        {
            if (ignoreRayAgro)
            {
                enemyState = EnemyState.ATTACKING;
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(attackSpawnPoint.position, direction.normalized, out hit, 100f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        enemyState = EnemyState.ATTACKING;
                    }
                }
            }
        }
        else enemyState = EnemyState.PATROLLING;


        topHalf.transform.position = topHalfLocator.position;
        //move towards a patrol point.
        direction = targetPoint.position - transform.position;
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        //when we get within a certain range of patrol point.
        float distance = Vector3.Distance(targetPoint.position, transform.position);
        //go to next point
        if (distance < distanceThreshold) UpdatePatrolPoints();
        //rotate in direction of movement.
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, direction.normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, patrolLerpSpeed * Time.deltaTime);

        switch (enemyState)
        {
            case EnemyState.PATROLLING:
                animator.SetBool("IsAggro", false);
                topHalf.transform.rotation = transform.rotation;
                break;
            case EnemyState.ATTACKING:
                animator.SetBool("IsAggro", true);
                //face towards player
                direction = player.transform.position - transform.position;
                targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                topHalf.transform.rotation = targetRotation;
                //topHalf.transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, attackLerpSpeed * Time.deltaTime);

                //shoot projectile on a timer
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    //Attack
                    Instantiate(enemyProjectile, attackSpawnPoint.position, attackSpawnPoint.rotation);
                    //attack Animation
                    timer = attackTimer;
                }
                //play sound effect
                break;
            default:
                break;
        }
    }

    void UpdatePatrolPoints()
    {
        if (patrolNumber == patrolPoints.Length - 1) patrolNumber = 0;
        else patrolNumber++;
        targetPoint = patrolPoints[patrolNumber];
    }

    public void KillEnemy()
    {
        pauseMenu.totalEnemyCount--;
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        Instantiate(deathParticles, topHalf.transform.position, Quaternion.identity);
        Destroy(topHalf);
        Destroy(gameObject);
    }
    public void StunEnemy()
    {
        dazedCount = dazedTimer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            Debug.Log(collision.relativeVelocity.sqrMagnitude);
            if (collision.relativeVelocity.sqrMagnitude < stunThreshold) return;
            //Play dazed particle effect
            StunEnemy();
        }
    }
}
