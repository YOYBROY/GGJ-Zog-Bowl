using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class SpringDamperSystem : MonoBehaviour
{
    public Transform target;
    Vector3 prevPosition;
    Vector3 targetPrevPos;
    [SerializeField] float errorAdjust = 1;
    [SerializeField] float dragFactor = 1;
    [SerializeField] float shakeAmountAdjuster = 1f;

    [HideInInspector] public GunController gunController;
    Transform player;
    [HideInInspector] public Transform fireLocation;


    private void OnEnable()
    {
        fireLocation = transform.GetChild(0);
        player = FindObjectOfType<FirstPersonController>().transform;
        gunController = player.GetComponent<GunController>();
        target = Camera.main.transform.GetChild(0);
        targetPrevPos = target.localPosition;
        prevPosition = transform.localPosition;
    }

    void FixedUpdate()
    {
        transform.rotation = target.rotation;
        Vector3 targetVelocity = target.localPosition - targetPrevPos;

        //Spring Damper System
        Vector3 error = target.localPosition - transform.localPosition;
        Vector3 velocity = transform.localPosition - prevPosition;

        velocity += (error * errorAdjust) * Time.deltaTime;
        velocity -= velocity * dragFactor * Time.deltaTime;
        Vector3 newPos = transform.localPosition + velocity;
        prevPosition = transform.localPosition;
        targetPrevPos = target.localPosition;
        transform.localPosition = newPos;
        if (error.magnitude < 0.001f && velocity.magnitude < 0.001f) transform.localPosition = target.localPosition;
        if(!gunController.hasShot && targetVelocity.magnitude < 0.001f) { gunController.shaken += error.magnitude * shakeAmountAdjuster * Time.deltaTime; }
    }
}
