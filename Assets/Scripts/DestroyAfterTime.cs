using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float timer = 1f;
    void Start()
    {
        Destroy(gameObject, timer);
    }
}
