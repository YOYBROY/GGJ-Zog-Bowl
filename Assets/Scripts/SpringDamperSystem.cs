using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class SpringDamperSystem : MonoBehaviour
{
    [Header("Designer Adjustments")]
    [Tooltip("Higher number makes shaking can harder")]
    [SerializeField] float errorAdjust = 1;
    [Tooltip("Increases drag creating less springy motion on shake")]
    [SerializeField] float dragFactor = 1;
    [Tooltip("Increases the amount shaking gives to the slider")]
    [SerializeField] float shakeAmountAdjuster = 1f;
    [Tooltip("Can velocity threshold before it plays a sound")]
    [SerializeField] private float shakeSoundThreshold = 0.1f;

    [HideInInspector] public Transform target;
    private Vector3 prevPosition;
    private Vector3 targetPrevPos;
    private float prevSpeed;

    private Transform player;
    [HideInInspector] public GunController gunController;
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
        float shakenError = Mathf.Abs(prevSpeed - velocity.magnitude);

        //update prev frame variables
        prevPosition = transform.localPosition;
        targetPrevPos = target.localPosition;
        prevSpeed = velocity.magnitude;

        //update position
        transform.localPosition = newPos;

        if (error.magnitude < 0.001f && velocity.magnitude < 0.001f) transform.localPosition = target.localPosition;
        if(!gunController.hasShot && targetVelocity.magnitude < 0.001f) 
        {
            gunController.shaken += shakenError * shakeAmountAdjuster * Time.deltaTime;
            if(shakenError > shakeSoundThreshold) AudioManager.PlaySound(SoundType.SHAKING, 1);
        }
    }
}
