using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowProjectile : MonoBehaviour
{
    [SerializeField] float force = 1f;
    [SerializeField] float angularForce = 1f;
    [SerializeField] bool isChampaign;

    void OnEnable()
    {
        GetComponent<Rigidbody>().AddForce(transform.up * force);
        GetComponent<Rigidbody>().angularVelocity += transform.right * angularForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isChampaign)
        {
            AudioManager.PlaySound(SoundType.GLASSSMASH, 1);
            Destroy(gameObject);
        }
        else
        {
            AudioManager.PlaySound(SoundType.SODALANDING, 1);
        }
    }
}
