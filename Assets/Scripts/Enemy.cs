using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform[] patrolPoints;
    Transform targetPoint;
    int patrolNumber = 0;
    [SerializeField] float patrolLerpSpeed = 2f;
    [SerializeField] float attackLerpSpeed = 15f;

    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float distanceThreshold = 0.01f;

    private GameObject player;

    [SerializeField] float stunThreshold = 500f;

    [SerializeField] float attackRange = 10f;
    float timer;
    [SerializeField] float attackTimer = 4f;

    [SerializeField] Transform attackSpawnPoint;
    [SerializeField] GameObject enemyProjectile;

    float dazedCount;
    [SerializeField] float dazedTimer = 2f;
    bool alert;
    bool prevAlert;
    float alertTimer;

    bool stunned;

    private PauseMenu pauseMenu;
    private Animator animator;

    [SerializeField] private Transform particleSocket;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private ParticleSystem stunParticles;
    [SerializeField] private ParticleSystem surpriseParticle;
    [SerializeField] private ParticleSystem confusedParticles;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip robotMoveAudio;

    enum EnemyState { PATROLLING, ATTACKING, APPROACHING };
    EnemyState enemyState;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        pauseMenu = FindObjectOfType<PauseMenu>();
        player = FindObjectOfType<FirstPersonController>().gameObject;
        targetPoint = patrolPoints[0];
        timer = attackTimer;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = AudioManager.GetAudioClip(SoundType.ROBOTMOVING);
        audioSource.loop = true;
        audioSource.volume = Mathf.Clamp(moveSpeed , 0, 1);
        audioSource.Play();
    }

    private void Update()
    {
        if (dazedCount > 0)
        {
            if (!stunned)
            {
                Instantiate(stunParticles, particleSocket.transform.position, Quaternion.identity, particleSocket.transform);
                stunned = true;
            }
            dazedCount -= Time.deltaTime;
            return;
        }
        else if (stunned) stunned = false;

        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        Vector3 direction = player.transform.position - transform.position;

        if (distanceToPlayer < attackRange)
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
        else if (alert) enemyState = EnemyState.APPROACHING;
        else enemyState = EnemyState.PATROLLING;

        if (alert && !prevAlert)
        {
            Instantiate(surpriseParticle, particleSocket.position, Quaternion.identity, particleSocket);
            prevAlert = alert;
        }
        if(!alert && prevAlert)
        {
            Instantiate(confusedParticles, particleSocket.position, Quaternion.identity, particleSocket);
            prevAlert = alert;
        }
        

        switch (enemyState)
        {
            case EnemyState.PATROLLING:
                animator.SetBool("IsAggro", false);
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
                break;
            case EnemyState.ATTACKING:
                animator.SetBool("IsAggro", true);
                alert = true;
                //face towards player
                direction = player.transform.position - transform.position;
                targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, attackLerpSpeed * Time.deltaTime);

                //shoot projectile on a timer
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    //Attack
                    Instantiate(enemyProjectile, attackSpawnPoint.position, attackSpawnPoint.rotation);
                    //attack Animation
                    AudioManager.PlaySound(SoundType.ZOGCOUGH, 1);
                    timer = attackTimer;
                }
                //play sound effect
                break;
            case EnemyState.APPROACHING:
                //face towards player
                direction = player.transform.position - transform.position;
                targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, attackLerpSpeed * Time.deltaTime);

                RaycastHit hit;
                if (Physics.Raycast(attackSpawnPoint.position, direction.normalized, out hit, 100f))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        //Mose Towards Player
                        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                    }
                    else alertTimer += Time.deltaTime;
                    if (alertTimer > 4f) { alert = false; enemyState = EnemyState.PATROLLING; alertTimer = 0; }
                }
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
            if (collision.relativeVelocity.sqrMagnitude < stunThreshold) return;
            //Play dazed particle effect
            StunEnemy();
        }
    }
}