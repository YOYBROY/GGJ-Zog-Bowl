using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowProjectile : MonoBehaviour
{
    [SerializeField] float force = 1f;
    [SerializeField] float angularForce = 1f;

    [HideInInspector] public bool hitSomething;

    void OnEnable()
    {
        GetComponent<Rigidbody>().AddForce(transform.up * force);
        GetComponent<Rigidbody>().angularVelocity += transform.right * angularForce;
    }    
}
