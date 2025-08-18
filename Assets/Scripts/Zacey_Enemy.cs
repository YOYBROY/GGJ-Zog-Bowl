using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations.Rigging;
[RequireComponent(typeof(AudioSource))]

public class ZaceyEnemy : MonoBehaviour
{
    [Header("Designer Adjustments")]
    [Tooltip("List of transforms for the enemy to move between")]
    [SerializeField] Transform[] patrolPoints;

    [Tooltip("How close to the patrol point the enemy needs to get before moving to the next patrol point")]
    [SerializeField] float distanceThreshold = 0.01f;

    [Tooltip("How fast the enemy turns while patrolling")]
    [SerializeField] float patrolLerpSpeed = 2f;

    [Tooltip("Max speed of Enemy")]
    [SerializeField] float moveSpeed = 1f;

    [Tooltip("Velocity threshold of incoming projectile before it actually stuns the enemy")]
    [SerializeField] float stunThreshold = 500f;

    [Tooltip("Distance the enemy can detect the player")]
    [SerializeField] float attackRange = 10f;

    [Tooltip("Time in seconds between shooting a projectile")]
    [SerializeField] float attackTimer = 4f;

    [Tooltip("Time in seconds the enemy takes to go back to patrolling after losing sight of the player")]
    [SerializeField] float attentionSpan = 3f;
    private float attentionSpanTimer;

    [Tooltip("Time in seconds the enemy is dazed for")]
    [SerializeField] float dazedTimer = 2f;

    [Tooltip("True = enemy will always be agro within the attackRange, False = enemy will also need to see player to agro")]
    [SerializeField] bool ignoreRayAgro;

    [Header("Public References")]

    private Rig rig;
    [SerializeField] Transform attackSpawnPoint;
    [SerializeField] GameObject enemyProjectile;

    [SerializeField] GameObject topHalf;
    [SerializeField] Transform topHalfLocator;

    [SerializeField] ParticleSystem deathParticles;

    //Private Variables
    Transform targetPoint;
    int patrolNumber = 0;

    private GameObject player;

    float timer;
    float dazedCount;
    private AudioSource audioSource;

    private PauseMenu pauseMenu;
    private Animator animator;

    private int killCounter = 0;

    enum EnemyState { PATROLLING, ATTACKING, ALERT };
    EnemyState enemyState;

    void Start()
    {
        animator = transform.parent.GetComponentInChildren<Animator>();
        pauseMenu = FindObjectOfType<PauseMenu>();
        player = FindObjectOfType<FirstPersonController>().gameObject;
        targetPoint = patrolPoints[0];
        timer = attackTimer;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = AudioManager.GetAudioClip(SoundType.ROBOTMOVING);
        audioSource.loop = true;
        audioSource.Play();
        rig = GetComponentInChildren<Rig>();
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
                        attentionSpanTimer = 0;
                        enemyState = EnemyState.ATTACKING;
                    }
                    else
                    {
                        attentionSpanTimer += Time.deltaTime;
                        if(attentionSpanTimer > attentionSpan)
                        {
                            enemyState = EnemyState.PATROLLING;
                        }
                        else
                        {
                            enemyState = EnemyState.ALERT;
                        }
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
                rig.weight = 0;
                animator.SetBool("IsAggro", false);
                topHalf.transform.rotation = transform.rotation;
                break;
            case EnemyState.ATTACKING:
                rig.weight = 1;
                animator.SetBool("IsAggro", true);
                //face towards player
                direction = player.transform.position - transform.position;
                targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                topHalf.transform.rotation = targetRotation;

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
            case EnemyState.ALERT:
                animator.SetBool("IsAggro", true);
                //face towards player
                direction = player.transform.position - transform.position;
                targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                topHalf.transform.rotation = targetRotation;
                break;
            default:
                break;
        }
    }

    private void LateUpdate()
    {
        if(killCounter > 0)
        {
            pauseMenu.totalEnemyCount--;
            Instantiate(deathParticles, transform.position, Quaternion.identity);
            Instantiate(deathParticles, topHalf.transform.position, Quaternion.identity);
            gameObject.GetComponent<DestructibleObject>().SwapModel();
            Destroy(gameObject.transform.parent.gameObject);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerProjectile"))
        {
            Debug.Log(collision.relativeVelocity.sqrMagnitude);
            if (collision.relativeVelocity.sqrMagnitude < stunThreshold) return;
            //Play dazed particle effect
            KillEnemy();
        }
    }
}
