using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] GunController gunController;
    [SerializeField] string gunType;

    private Vector3 originalScale;

    void Start()
    {
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
