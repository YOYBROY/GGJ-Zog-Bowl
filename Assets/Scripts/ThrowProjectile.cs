using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowProjectile : MonoBehaviour
{
    [SerializeField] float force = 1f;
    [SerializeField] float angularForce = 1f;
    [SerializeField] bool isChampaign;
    [SerializeField] ParticleSystem impactParticle;
    [SerializeField] float killSpeed = 100f;

    void OnEnable()
    {
        GetComponent<Rigidbody>().AddForce(transform.up * force);
        GetComponent<Rigidbody>().angularVelocity += transform.right * angularForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.relativeVelocity.sqrMagnitude < killSpeed) return;
            //Get component for enemy
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy == null)
            {
                ZaceyEnemy zaceyEnemy = collision.gameObject.GetComponentInParent<ZaceyEnemy>();
                zaceyEnemy.KillEnemy();
            }
            else
            {
                enemy.KillEnemy();
            }
        }

        if (isChampaign)
        {
            if (impactParticle != null) Instantiate(impactParticle, transform.position, Quaternion.Euler(collision.contacts[0].normal));
            AudioManager.PlaySound(SoundType.GLASSSMASH, 1);
            Destroy(gameObject);
        }
        else
        {
            if (collision.impulse.sqrMagnitude < 5f) return;
            if (impactParticle != null) Instantiate(impactParticle, transform.position, Quaternion.Euler(collision.contacts[0].normal));
            AudioManager.PlaySound(SoundType.SODALANDING, 1);
        }

        DestructibleObject destructible = collision.gameObject.GetComponent<DestructibleObject>();
        if (destructible == null)
            { return; }
        destructible.SwapModel();
    }
}