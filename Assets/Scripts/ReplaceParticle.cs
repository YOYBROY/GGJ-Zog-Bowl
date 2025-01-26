using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceParticle : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particles = new List<ParticleSystem>();

    void Start()
    {
        int index = Random.Range(0, particles.Count);
        Instantiate(particles[index], gameObject.transform.position, Quaternion.identity, gameObject.transform.parent);
        Destroy(gameObject);
    }
}
