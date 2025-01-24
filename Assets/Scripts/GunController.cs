using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    [SerializeField] GameObject gun;
    private SpringDamperSystem gunSpringSystem;
    [SerializeField] float shakeAdjust = 1f;

    [SerializeField] float shakenThreshold = 5f;
    [HideInInspector] public float shaken = 0f;
    [SerializeField] private Slider shakenSlider;

    FirstPersonController _controller;
    float storedRotationSpeed;
    float storedMoveSpeed;

    private StarterAssetsInputs _input;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        _controller = GetComponent<FirstPersonController>();
        _input = GetComponent<StarterAssetsInputs>();
        gunSpringSystem = gun.GetComponent<SpringDamperSystem>();
        gunSpringSystem.enabled = false;

        storedMoveSpeed = _controller.MoveSpeed;
        storedRotationSpeed = _controller.RotationSpeed;

        shaken = 0;
    }

    void Update()
    {
        if(shaken >= shakenThreshold) { shaken = shakenThreshold; }
        shakenSlider.value = shaken;
        Vector3 moveInput = new Vector3(_input.look.x, 0, _input.look.y);

        if (Input.GetMouseButton(1))
        {
            gun.transform.SetParent(null);
            gunSpringSystem.enabled = true;
            _controller.MoveSpeed = 0;
            _controller.RotationSpeed = 0;
            gun.transform.Translate(moveInput * shakeAdjust * Time.deltaTime);
        }
        if(Input.GetMouseButtonUp(1))
        {
            gun.transform.SetParent(cam.transform);
            gunSpringSystem.enabled = false;
            gun.transform.position = gunSpringSystem.target.position;
            _controller.MoveSpeed = storedMoveSpeed;
            _controller.RotationSpeed = storedRotationSpeed;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    { 
        if (shaken >= shakenThreshold)
        {
            shaken = 0;
            Debug.Log("Shot Weapon");
        }
    }
}
