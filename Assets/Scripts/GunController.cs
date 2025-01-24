using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    [SerializeField] float lerpSpeed = 1f;
    Vector3 targetPos;
    Quaternion targetRot;

    [SerializeField] Transform gunSocket;
    Vector3 storedGunSocketPos;
    Quaternion storedGunSocketRot;
    [SerializeField] Transform shakePosition;
    GameObject activeGun;
    SpringDamperSystem gunSpringSystem;
    [SerializeField] float shakeAdjust = 1f;

    [SerializeField] float shakenThreshold = 5f;
    [HideInInspector] public float shaken = 0f;
    [SerializeField] private Slider shakenSlider;

    FirstPersonController _controller;
    float storedRotationSpeed;
    float storedMoveSpeed;

    StarterAssetsInputs _input;
    Camera cam;

    [HideInInspector] public bool canShoot;
    [HideInInspector] public bool hasShot;

    [SerializeField] GameObject sodaCanPrefab;

    void Start()
    {
        cam = Camera.main;
        _controller = GetComponent<FirstPersonController>();
        _input = GetComponent<StarterAssetsInputs>();

        targetRot = gunSocket.transform.localRotation;
        targetPos = gunSocket.localPosition;
        storedGunSocketPos = gunSocket.transform.localPosition;
        storedGunSocketRot = gunSocket.transform.localRotation;
        storedMoveSpeed = _controller.MoveSpeed;
        storedRotationSpeed = _controller.RotationSpeed;

        shaken = 0;
    }

    void Update()
    {
        if (shaken >= shakenThreshold) { shaken = shakenThreshold; canShoot = true; }
        shakenSlider.value = Remap(shaken, 0, shakenThreshold, 0, 1);
        Vector3 moveInput = new Vector3(_input.look.x, 0, _input.look.y);

        if (Input.GetMouseButton(1))
        {
            targetPos = shakePosition.localPosition;
            targetRot = shakePosition.localRotation;
            activeGun.transform.SetParent(null);
            gunSpringSystem.enabled = true;
            _controller.MoveSpeed = 0;
            _controller.RotationSpeed = 0;
            activeGun.transform.Translate(moveInput * shakeAdjust * Time.deltaTime);
        }
        if (Input.GetMouseButtonUp(1))
        {
            targetPos = storedGunSocketPos;
            targetRot = storedGunSocketRot;
            activeGun.transform.SetParent(gunSocket.transform);
            gunSpringSystem.enabled = false;
            activeGun.transform.position = gunSpringSystem.target.position;
            _controller.MoveSpeed = storedMoveSpeed;
            _controller.RotationSpeed = storedRotationSpeed;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        gunSocket.localPosition = Vector3.Lerp(gunSocket.localPosition, targetPos, lerpSpeed * Time.deltaTime);
        gunSocket.localRotation = Quaternion.Lerp(gunSocket.localRotation, targetRot, lerpSpeed * Time.deltaTime);
    }

    void Shoot()
    {
        if (activeGun == null) return;
        if (hasShot)
        {
            Debug.Log("Throw Gun");
            Destroy(activeGun);
            activeGun = null;
            gunSpringSystem = null;
            //Shoot Projectile that stuns
            hasShot = false;
        }
        if (canShoot)
        {
            canShoot = false;
            hasShot = true;
            shaken = 0;
            //Shoot Hitscan that kills
        }
    }

    public void AddGun(string gunType)
    {
        if (gunType == "Soda")
        {
            activeGun = Instantiate(sodaCanPrefab, gunSocket.position, gunSocket.rotation, cam.transform);
        }
        gunSpringSystem = activeGun.GetComponent<SpringDamperSystem>();
        gunSpringSystem.enabled = false;
        gunSpringSystem.target = gunSocket;
        gunSpringSystem.gunController = this;
    }

    float Remap(float value, float minA, float maxA, float minB, float maxB)
    {
        return minB + (value - minA) * (maxB - minB) / (maxA - minA);
    }
}
