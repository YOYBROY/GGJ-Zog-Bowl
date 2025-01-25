using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanTrailer : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

    void Start()
    {
        Invoke("Launch", 0.01f);
    }
    void Launch()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, float.MaxValue, layerMask))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.Translate(transform.forward * 100f);
        }
    }
}
