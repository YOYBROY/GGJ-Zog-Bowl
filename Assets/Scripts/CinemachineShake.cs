using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }

    public float bigShake = 60f;
    public float smallShake = 30f;
    [SerializeField] private float traumaReductionSpeed;
    private float trauma;

    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake()
    {
        Instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void AddTrauma(float intensity)
    {
        trauma += intensity;
    }

    private void Update()
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = trauma;

        if(trauma > 0)
        {
            //trauma -= traumaReductionSpeed * Time.deltaTime;
            trauma = Mathf.Lerp(trauma, 0, traumaReductionSpeed * Time.deltaTime);
        }
    }
}