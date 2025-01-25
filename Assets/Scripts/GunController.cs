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
    [SerializeField] Transform launchPosition;
    [SerializeField] Transform lookAtRayPosition;
    [HideInInspector] public GameObject activeGun;
    SpringDamperSystem gunSpringSystem;
    [SerializeField] float shakeAdjust = 1f;

    [SerializeField] float shakenThreshold = 5f;
    [HideInInspector] public float shaken = 0f;
    [SerializeField] private Slider shakenSlider;

    FirstPersonController _controller;
    [HideInInspector] public int storedRotationSpeed;
    float storedMoveSpeed;

    StarterAssetsInputs _input;
    Camera cam;
    string activeGunType;

    [HideInInspector] public bool canShoot;
    [HideInInspector] public bool hasShot;

    [SerializeField] TrailRenderer hitscanTrail;

    [SerializeField] GameObject sodaCanPrefab;
    [SerializeField] GameObject sodaCanProjectile;
    [SerializeField] float sodaCanDamageRange = 100f;

    [SerializeField] GameObject ChampainGunPrefab;
    [SerializeField] GameObject ChampainProjectile;

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
        if (PauseMenu.isPaused) return;
        if (shaken >= shakenThreshold) { shaken = shakenThreshold; canShoot = true; }
        shakenSlider.value = Remap(shaken, 0, shakenThreshold, 0, 1);
        Vector3 moveInput = new Vector3(_input.look.x, -_input.look.y, 0f);

        if (Input.GetMouseButton(1))
        {
            if (activeGun == null) return;
            targetPos = shakePosition.localPosition;
            targetRot = shakePosition.localRotation;
            gunSpringSystem.enabled = true;
            _controller.RotationSpeed = 0;
            activeGun.transform.Translate(moveInput * shakeAdjust * Time.deltaTime);
        }
        else
        {
            targetRot = lookAtRayPosition.localRotation;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (activeGun == null) return;
            targetPos = storedGunSocketPos;
            targetRot = storedGunSocketRot;
            activeGun.transform.SetParent(gunSocket.transform);
            gunSpringSystem.enabled = false;
            activeGun.transform.position = gunSpringSystem.target.position;
            _controller.RotationSpeed = storedRotationSpeed;
        }
        if (Input.GetMouseButtonDown(0))
        {
            AttemptShot();
        }

        gunSocket.localPosition = Vector3.Lerp(gunSocket.localPosition, targetPos, lerpSpeed * Time.deltaTime);
        gunSocket.localRotation = Quaternion.Lerp(gunSocket.localRotation, targetRot, lerpSpeed * Time.deltaTime);
    }

    void AttemptShot()
    {
        if (activeGun == null) return;
        if (hasShot)
        {
            ThrowGun();
        }
        if (canShoot)
        {
            ShootGun();
        }
    }

    public void AddGun(string gunType)
    {
        AudioManager.PlaySound(SoundType.WEAPONPICKUP);
        if (gunType == "Soda")
        {
            activeGun = Instantiate(sodaCanPrefab, gunSocket.position, gunSocket.rotation, cam.transform);
        }
        else if (gunType == "Champ")
        {

            activeGun = Instantiate(ChampainGunPrefab, gunSocket.position, gunSocket.rotation, cam.transform);
        }
        activeGunType = gunType;
        gunSpringSystem = activeGun.GetComponent<SpringDamperSystem>();
        gunSpringSystem.enabled = false;
    }

    private void ShootGun()
    {
        canShoot = false;
        hasShot = true;
        shaken = 0;

        if (activeGunType == "Soda") AudioManager.PlaySound(SoundType.SODACANPOP, 1);
        else AudioManager.PlaySound(SoundType.CHAMPAIGNPOP, 1);


        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.MaxValue))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                ZaceyEnemy zaceyEnemy = hit.collider.GetComponent<ZaceyEnemy>();
                bool zacey = false;
                if (enemy == null) zacey = true;
                if (activeGunType == "Soda")
                {
                    float distance = Vector3.Distance(hit.point, cam.transform.position);
                    if (distance > sodaCanDamageRange)
                    {
                        if (zacey) zaceyEnemy.StunEnemy();
                        else enemy.StunEnemy();
                    }
                    else
                    {
                        if (zacey) zaceyEnemy.KillEnemy();
                        else enemy.KillEnemy();
                    }
                }
                else
                {
                    if (zacey) zaceyEnemy.KillEnemy();
                    else enemy.KillEnemy();
                }
            }
            //Shoot Hitscan that kills
            TrailRenderer trail = Instantiate(hitscanTrail, gunSpringSystem.fireLocation.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));
        }
    }

    public void ThrowGun()
    {
        Destroy(activeGun);
        activeGun = null;
        gunSpringSystem = null;
        if (activeGunType == "Soda") { Instantiate(sodaCanProjectile, launchPosition.position, launchPosition.rotation); }
        else if (activeGunType == "Champ") { Instantiate(ChampainProjectile, launchPosition.position, launchPosition.rotation); }
        shaken = 0;
        canShoot = false;
        hasShot = false;
    }

    float Remap(float value, float minA, float maxA, float minB, float maxB)
    {
        return minB + (value - minA) * (maxB - minB) / (maxA - minA);
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPos = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        trail.transform.position = hit.point;
        //Instantiate Particles for Hit point

        Destroy(trail.gameObject, trail.time);
    }
}