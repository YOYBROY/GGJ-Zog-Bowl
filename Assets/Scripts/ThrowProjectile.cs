using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowProjectile : MonoBehaviour
{
    [SerializeField] float force = 1f;
    [SerializeField] float angularForce = 1f;
    [SerializeField] bool isChampaign;
    [SerializeField] ParticleSystem impactParticle;

    void OnEnable()
    {
        GetComponent<Rigidbody>().AddForce(transform.up * force);
        GetComponent<Rigidbody>().angularVelocity += transform.right * angularForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isChampaign)
        {
            if(impactParticle != null) Instantiate(impactParticle, transform.position, Quaternion.Euler(collision.contacts[0].normal)); 
            AudioManager.PlaySound(SoundType.GLASSSMASH, 1);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log(collision.impulse.sqrMagnitude);
            if (collision.impulse.sqrMagnitude < 5f) return;
            if (impactParticle != null) Instantiate(impactParticle, transform.position, Quaternion.Euler(collision.contacts[0].normal));
            AudioManager.PlaySound(SoundType.SODALANDING, 1);
        }
    }
}
