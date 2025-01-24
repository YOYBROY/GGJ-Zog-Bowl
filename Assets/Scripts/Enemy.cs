using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GunController gunController;

    void Start()
    {
        gunController = FindObjectOfType<GunController>();
    }

    private void OnMouseDown()
    {
        if (gunController.canShoot)
        {
            Destroy(gameObject);
        }
    }
}
