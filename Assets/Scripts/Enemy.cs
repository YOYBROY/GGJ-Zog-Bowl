using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Designer Adjustments")]
    [Tooltip("List of transforms for the enemy to move between")]
    [SerializeField] Transform[] patrolPoints;
    [Tooltip("How close to the patrol point the enemy needs to get before moving to the next patrol point")]
    [SerializeField] float distanceThreshold = 0.01f;

    [Tooltip("How fast the enemy turns while patrolling")]
    [SerializeField] float patrolLerpSpeed = 2f;
    [Tooltip("How fast the enemy turns while attacking")]
    [SerializeField] float attackLerpSpeed = 15f;

    [Tooltip("Max speed of Enemy")]
    [SerializeField] float moveSpeed = 1f;

    [Tooltip("Distance the enemy can detect the player")]
    [SerializeField] float attackRange = 10f;
    [Tooltip("Time in seconds the enemy takes to go back to patrolling after losing sight of the player")]
    [SerializeField] float attackTimer = 4f;

    [Tooltip("Time in seconds the enemy is dazed for")]
    [SerializeField] float dazedTimer = 2f;

    [Header("References")]

    [SerializeField] Transform attackSpawnPoint;
    [SerializeField] GameObject enemyProjectile;

    [SerializeField] private Transform particleSocket;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private ParticleSystem stunParticles;
    [SerializeField] private ParticleSystem surpriseParticle;
    [SerializeField] private ParticleSystem confusedParticles;

    //private variables
    Transform targetPoint;
    int patrolNumber = 0;

    private GameObject player;

    private float startHeight;

    float timer;

    float dazedCount;
    bool alert;
    bool prevAlert;
    float alertTimer;

    bool stunned;

    private Animator animator;

    private int killCounter = 0;


    private AudioSource audioSource;

    enum EnemyState { PATROLLING, ATTACKING, APPROACHING };
    EnemyState enemyState;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        player = FindObjectOfType<FirstPersonController>().gameObject;
        targetPoint = patrolPoints[0];
        timer = attackTimer;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = AudioManager.GetAudioClip(SoundType.ROBOTMOVING);
        audioSource.loop = true;
        audioSource.Play();
        startHeight = transform.position.y;
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


        RaycastHit hit;
        if (Physics.Raycast(attackSpawnPoint.position, direction.normalized, out hit, attackRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                enemyState = EnemyState.ATTACKING;
            }
            else if (alert) enemyState = EnemyState.APPROACHING;
            else enemyState = EnemyState.PATROLLING;
        }
        

        if (alert && !prevAlert)
        {
            Instantiate(surpriseParticle, particleSocket.position, Quaternion.identity, particleSocket);
            prevAlert = alert;
        }
        if (!alert && prevAlert)
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

        transform.position = new Vector3(transform.position.x, startHeight, transform.position.z);
    }

    private void LateUpdate()
    {
        if(killCounter > 0)
        {
            PauseMenu.totalEnemyCount--;
            Instantiate(deathParticles, transform.position, Quaternion.identity);
            GetComponent<DestructibleObject>().SwapEnemyModel();
            Destroy(gameObject);
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
        killCounter++;
    }

    public void StunEnemy()
    {
        dazedCount = dazedTimer;
    }
}