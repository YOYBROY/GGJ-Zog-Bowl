using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] string gunType;
    [SerializeField] float pickUpDistance = 5f;
    [SerializeField] bool deletesSelf = true;
    [SerializeField] float scaleUpAmount = 1.2f;
    [SerializeField] float lerpSpeed = 4f;

    [SerializeField] GameObject[] outliners;

    //References
    GunController gunController;
    
    private Vector3 originalScale;
    Vector3 targetScale;

    void OnEnable()
    {
        gunController = FindObjectOfType<GunController>();
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, lerpSpeed * Time.deltaTime);
    }

    private void OnMouseOver()
    {
        if (PauseMenu.isPaused) return;
        float distance = Vector3.Distance(gunController.transform.position, transform.position);
        if (distance > pickUpDistance) return;
        foreach (GameObject child in outliners)
        {
            //white outline
            child.layer = 7;
        }
        targetScale = originalScale * scaleUpAmount;
    }

    private void OnMouseExit()
    {
        if (PauseMenu.isPaused) return;
        float distance = Vector3.Distance(gunController.transform.position, transform.position);
        if (distance > pickUpDistance) return;
        foreach (GameObject child in outliners)
        {
            //black outline
            child.layer = 6;
        }
        targetScale = originalScale;
    }

    private void OnMouseDown()
    {
        if (PauseMenu.isPaused) return;
        float distance = Vector3.Distance(gunController.transform.position, transform.position);
        if (distance > pickUpDistance) return;
        if(gunController.activeGun != null)
        {
            gunController.ThrowGun();
        }
        gunController.AddGun(gunType);
        if(deletesSelf) Destroy(gameObject);
    }
}
