using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] string gunType;

    //References
    GunController gunController;
    
    private Vector3 originalScale;

    void OnEnable()
    {
        gunController = GameObject.FindObjectOfType<GunController>();
        originalScale = transform.localScale;
    }

    private void OnMouseOver()
    {
        transform.localScale = originalScale * 1.2f;
    }

    private void OnMouseExit()
    {
        transform.localScale = originalScale;
    }

    private void OnMouseDown()
    {
        gunController.AddGun(gunType);
        Destroy(gameObject);
    }
}
