using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingOffsetForVendingMachines : MonoBehaviour
{
    private Animator animator;
    private float randomStartTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        randomStartTime = Random.Range(0.1f, 2.0f);
        StartCoroutine("StartAnimation");
    }

    private IEnumerator StartAnimation()
    {
        float timer = 0.0f;
        while (timer < randomStartTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        animator.SetTrigger("OnPickup");
    }
}
