using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    [Header("Designer Adjustments")]
    [Tooltip("Sensitivity of shaking, higher number makes it easier to shake")]
    [SerializeField] float shakeAdjust = 1f;

    [Tooltip("Total shake needed to load the shot")]
    [SerializeField] float shakenThreshold = 5f;

    [Tooltip("How fast the can moves positions, higher number is faster")]
    [SerializeField] float lerpSpeed = 1f;

    [Tooltip("Max distance the soda can will damage")]
    [SerializeField] float sodaCanDamageRange = 10f;

    [Tooltip("Random spread amount of the soda can")]
    [SerializeField] float sodaCanSpread = 0.5f;

    [Tooltip("Number of bullets shot from soda can")]
    [SerializeField] int sodaCanProjectiles = 10;

    [Tooltip("The layermask used to detect bullet hits")]
    [SerializeField] LayerMask shootLayerMask;

    [Header("References")]
    [SerializeField] Transform gunSocket;

    [SerializeField] Transform shakePosition;
    [SerializeField] Transform launchPosition;
    [SerializeField] Transform lookAtRayPosition;
    
    [SerializeField] private Slider shakenSlider;

    [Tooltip("Bullet trail")]
    [SerializeField] TrailRenderer hitscanTrail;

    [SerializeField] GameObject sodaCanPrefab;
    [SerializeField] GameObject sodaCanProjectile;
    [SerializeField] ParticleSystem sodaShootParticle;
    [SerializeField] ParticleSystem sodaImpactParticle;

    [SerializeField] GameObject ChampainGunPrefab;
    [SerializeField] GameObject ChampainProjectile;
    [SerializeField] ParticleSystem champainShootParticle;
    [SerializeField] ParticleSystem champaignImpactParticle;

    //Private Variables
    private Vector3 storedGunSocketPos;
    private Quaternion storedGunSocketRot;

    private Vector3 targetPos;
    private Quaternion targetRot;

    [HideInInspector] public GameObject activeGun;
    private SpringDamperSystem gunSpringSystem;
    [HideInInspector] public float shaken = 0f;

    private FirstPersonController _controller;
    [HideInInspector] public int storedRotationSpeed;

    private StarterAssetsInputs _input;
    private Camera cam;
    private string activeGunType;
    private PauseMenu pauseMenu;

    [HideInInspector] public bool canShoot;
    [HideInInspector] public bool hasShot;

    private bool shakingGun;


    void Start()
    {
        cam = Camera.main;
        _controller = GetComponent<FirstPersonController>();
        _input = GetComponent<StarterAssetsInputs>();
        pauseMenu = FindObjectOfType<PauseMenu>().GetComponent<PauseMenu>();
        targetRot = gunSocket.transform.localRotation;
        targetPos = gunSocket.localPosition;
        storedGunSocketPos = gunSocket.transform.localPosition;
        storedGunSocketRot = gunSocket.transform.localRotation;
        storedRotationSpeed = _controller.RotationSpeed;
        activeGunType = "Null";
        shaken = 0;
    }

    void Update()
    {
        if (PauseMenu.isPaused) return;
        if (shaken >= shakenThreshold) { shaken = shakenThreshold; canShoot = true; }
        shakenSlider.value = Remap(shaken, 0, shakenThreshold, 0, 1);
        Vector3 moveInput = new Vector3(_input.look.x, -_input.look.y, 0f);

        if (Input.GetMouseButton(1)) //rmb
        {
            if (activeGun == null) return;
            if(!canShoot)
            {
                targetPos = shakePosition.localPosition;
                targetRot = shakePosition.localRotation;
                gunSpringSystem.enabled = true;
                activeGun.transform.Translate(moveInput * shakeAdjust * Time.deltaTime);
                shakingGun = true;
            }
            else { StopShakingGun(); }
            _controller.RotationSpeed = 0;
        }
        else
        {
            targetRot = lookAtRayPosition.localRotation;
        }

        if (Input.GetMouseButtonUp(1)) //rmb
        {
            StopShakingGun();
            _controller.RotationSpeed = storedRotationSpeed;
        }

        if (Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1))
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
        pauseMenu.UpdateReticleUI(gunType);
    }

    private void ShootGun()
    {
        canShoot = false;
        hasShot = true;
        shaken = 0;
        //switch gun model to used
        SwitchGunToUsed();

        if (activeGunType == "Soda")
        {
            CinemachineShake.Instance.AddTrauma(CinemachineShake.Instance.bigShake);

            //AudioManager.PlaySound(SoundType.SODACANPOP, 1);
            //Particle Effect at launch point
            Instantiate(sodaShootParticle, activeGun.transform.GetChild(0).position, activeGun.transform.GetChild(0).rotation, activeGun.transform);

            

            for (int i = 0; i < sodaCanProjectiles; i++)
            {
                Vector3 shootDirection = cam.transform.forward + new Vector3(
                                                                 Random.Range(-sodaCanSpread, sodaCanSpread),
                                                                 Random.Range(-sodaCanSpread, sodaCanSpread),
                                                                 Random.Range(-sodaCanSpread, sodaCanSpread)
                                                                 );
                shootDirection.Normalize();

                RaycastHit sodaHit;

                if (Physics.Raycast(gunSpringSystem.fireLocation.position, shootDirection, out sodaHit, float.MaxValue, shootLayerMask))
                {
                    if (sodaHit.collider.CompareTag("Enemy"))
                    {
                        Enemy enemy = sodaHit.collider.GetComponent<Enemy>();
                        ZaceyEnemy zaceyEnemy = sodaHit.collider.GetComponentInParent<ZaceyEnemy>();
                        bool zacey = false;
                        if (enemy == null) zacey = true;
                        float distance = Vector3.Distance(sodaHit.point, cam.transform.position);
                        if (distance < sodaCanDamageRange)
                        {
                            if (zacey) zaceyEnemy.KillEnemy();
                            else enemy.KillEnemy();
                        }
                    }
                    else if (sodaHit.collider.CompareTag("Destructible"))
                    {
                        sodaHit.collider.GetComponent<DestructibleObject>().SwapModel();
                    }
                    StartCoroutine(SpawnTrail(gunSpringSystem.fireLocation.position, sodaHit.point, sodaHit));
                }
                else
                {
                    StartCoroutine(SpawnTrail(gunSpringSystem.fireLocation.position, gunSpringSystem.fireLocation.position + (shootDirection * sodaCanDamageRange), sodaHit)); //magic number for how far away to send the trail
                }
            }
        }
        else
        {
            CinemachineShake.Instance.AddTrauma(CinemachineShake.Instance.smallShake);
            AudioManager.PlaySound(SoundType.CHAMPAIGNPOP, 1);
            //Particle Effect At launch point
            Instantiate(champainShootParticle, activeGun.transform.GetChild(0).position, activeGun.transform.GetChild(0).rotation, activeGun.transform);
        }


        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.MaxValue, shootLayerMask))
        {
            float distance = Vector3.Distance(hit.point, cam.transform.position);
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                ZaceyEnemy zaceyEnemy = hit.collider.GetComponentInParent<ZaceyEnemy>();
                bool zacey = false;
                if (enemy == null) zacey = true;
                if (activeGunType == "Soda")
                {
                    if (distance < sodaCanDamageRange)
                    {
                        if (zacey) zaceyEnemy.KillEnemy();
                        else enemy.KillEnemy();
                    }
                    else
                    {
                        StartCoroutine(SpawnTrail(gunSpringSystem.fireLocation.position, gunSpringSystem.fireLocation.position + (gunSpringSystem.fireLocation.forward * sodaCanDamageRange), hit));
                    }
                }
                else
                {
                    if (zacey) zaceyEnemy.KillEnemy();
                    else enemy.KillEnemy();
                }
            }
            else if (hit.collider.CompareTag("Destructible"))
            {
                hit.collider.GetComponent<DestructibleObject>().SwapModel();
            }
            if (activeGunType == "Soda" && distance > sodaCanDamageRange)
            {
                StartCoroutine(SpawnTrail(gunSpringSystem.fireLocation.position, gunSpringSystem.fireLocation.position + (gunSpringSystem.fireLocation.forward * sodaCanDamageRange), hit));
            }
            else
            {
                //Shoot Hitscan that kills
                StartCoroutine(SpawnTrail(gunSpringSystem.fireLocation.position, hit.point, hit));
            }
        }
        else
        {
            if (activeGunType == "Soda")
            {
                //CinemachineShake.Instance.AddTrauma(500);
                StartCoroutine(SpawnTrail(gunSpringSystem.fireLocation.position, gunSpringSystem.fireLocation.position + (gunSpringSystem.fireLocation.forward * sodaCanDamageRange), hit));
            }
            else
            {
                //CinemachineShake.Instance.AddTrauma(250);
                StartCoroutine(SpawnTrail(gunSpringSystem.fireLocation.position, gunSpringSystem.fireLocation.position + (gunSpringSystem.fireLocation.forward * 100), hit));
            }
        }
    }

    public void ThrowGun()
    {
        Destroy(activeGun);
        activeGun = null;
        gunSpringSystem = null;
        if (activeGunType == "Soda") { Instantiate(sodaCanProjectile, launchPosition.position, launchPosition.rotation); }
        else if (activeGunType == "Champ") { Instantiate(ChampainProjectile, launchPosition.position, launchPosition.rotation); }
        activeGunType = "Null";
        pauseMenu.UpdateReticleUI(activeGunType);
        shaken = 0;
        canShoot = false;
        hasShot = false;
    }

    private void SwitchGunToUsed()
    {
        activeGun.GetComponent<UsedGunSwitcher>().SwitchGunModel();
    }

    private void StopShakingGun()
    {
        if (activeGun == null) return;
        if(shakingGun)
        {
            targetPos = storedGunSocketPos;
            targetRot = storedGunSocketRot;
            gunSpringSystem.enabled = false;
            activeGun.transform.parent = gunSocket.transform;
            activeGun.transform.position = gunSpringSystem.target.position;
            shakingGun = false;
        }
    }

    float Remap(float value, float minA, float maxA, float minB, float maxB)
    {
        return minB + (value - minA) * (maxB - minB) / (maxA - minA);
    }

    private IEnumerator SpawnTrail(Vector3 startPosition, Vector3 endPosition, RaycastHit hit)
    {
        TrailRenderer trail = Instantiate(hitscanTrail, startPosition, Quaternion.identity);

        float time = 0;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, endPosition, time);
            time += Time.deltaTime / trail.time * 10f;
            yield return null;
        }
        trail.transform.position = endPosition;

        if (activeGunType == "Soda") Instantiate(sodaImpactParticle, endPosition, Quaternion.Euler(hit.normal));
        else Instantiate(champaignImpactParticle, endPosition, Quaternion.Euler(hit.normal));


        Destroy(trail.gameObject, trail.time);
    }
}