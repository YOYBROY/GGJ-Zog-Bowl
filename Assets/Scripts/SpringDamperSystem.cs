using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringDamperSystem : MonoBehaviour
{
    [SerializeField] Transform target;
    Vector3 prevPosition;
    [Range(0f, 5f)]
    [SerializeField] float errorAdjust = 1;
    [SerializeField] float dragFactor = 1;

    void Start()
    {
        prevPosition = transform.position;
    }

    void Update()
    {
        //Spring Damper System
        Vector3 error = target.position - transform.position;
        Vector3 velocity = transform.position - prevPosition;
        velocity += (error * errorAdjust) * Time.deltaTime;
        velocity -= velocity * dragFactor * Time.deltaTime;
        Vector3 newPos = transform.position + velocity;
        prevPosition = transform.position;
        newPos = new Vector3(newPos.x, transform.position.y, newPos.z);
        transform.position = newPos;
        if(error.magnitude < 0.01f && velocity.magnitude < 0.01f) transform.position = target.position;
    }
}
