using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] string gunType;
    [SerializeField] float pickUpDistance = 5f;

    //References
    GunController gunController;
    
    private Vector3 originalScale;

    void OnEnable()
    {
        gunController = FindObjectOfType<GunController>();
        originalScale = transform.localScale;
    }

    private void OnMouseOver()
    {
        float distance = Vector3.Distance(gunController.transform.position, transform.position);
        if (distance > pickUpDistance) return;
        if (PauseMenu.isPaused) return;
        transform.localScale = originalScale * 1.2f;
    }

    private void OnMouseExit()
    {
        if (PauseMenu.isPaused) return;
        transform.localScale = originalScale;
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
        Destroy(gameObject);
    }
}
