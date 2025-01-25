using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform[] patrolPoints;
    Transform targetPoint;
    int patrolNumber = 0;
    [SerializeField] float lerpSpeed = 1f;

    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float distanceThreshold = 0.01f;

    private GunController gunController;
    private Material storedMaterial;
    [SerializeField] private Material stunnedMat;

    [SerializeField] float stunThreshold = 500f;

    void Start()
    {
        gunController = FindObjectOfType<GunController>();
        storedMaterial = GetComponent<Material>();
        targetPoint = patrolPoints[0]; 
    }

    private void Update()
    {
        //STATE 1: PATROLLING
        //move towards a patrol point.
        Vector3 direction = targetPoint.position - transform.position;
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        //when we get within a certain range of patrol point.
        float distance = Vector3.Distance(targetPoint.position, transform.position);
        if (distance < distanceThreshold) UpdatePatrolPoints();
        //go to next.
        //rotate in direction of movement.
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, direction.normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);

        //STATE 2: ATTACKING
        //face towards player
        //shoot projectile on a timer
        //play sound effect
    }

    void UpdatePatrolPoints()
    {
        if (patrolNumber == patrolPoints.Length - 1) patrolNumber = 0;
        else patrolNumber++;
        targetPoint = patrolPoints[patrolNumber];
    }

    private void OnMouseDown()
    {
        if (gunController.canShoot)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("PlayerProjectile"))
        {
            Debug.Log(collision.relativeVelocity.sqrMagnitude);
            if (collision.relativeVelocity.sqrMagnitude < stunThreshold) return;
            GetComponent<Renderer>().material = stunnedMat;
        }
    }
}
