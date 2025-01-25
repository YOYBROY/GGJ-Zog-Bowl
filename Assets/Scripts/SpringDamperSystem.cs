using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class SpringDamperSystem : MonoBehaviour
{
    [HideInInspector] public Transform target;
    Vector3 prevPosition;
    [SerializeField] float errorAdjust = 1;
    [SerializeField] float dragFactor = 1;
    [SerializeField] float shakeAmountAdjuster = 1f;

    [HideInInspector] public GunController gunController;


    private void OnEnable()
    {
        prevPosition = transform.position;
    }

    void Update()
    {
        transform.rotation = target.rotation;

        //Spring Damper System
        Vector3 error = target.position - transform.position;
        Vector3 velocity = transform.position - prevPosition;
        velocity += (error * errorAdjust) * Time.deltaTime;
        velocity -= velocity * dragFactor * Time.deltaTime;
        Vector3 newPos = transform.position + velocity;
        prevPosition = transform.position;
        transform.position = newPos;
        if (error.magnitude < 0.001f && velocity.magnitude < 0.001f) transform.position = target.position;
        if(!gunController.hasShot) { gunController.shaken += error.magnitude * shakeAmountAdjuster * Time.deltaTime; }
    }
}
