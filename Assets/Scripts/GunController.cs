using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] GameObject gun;
    [SerializeField] float shakeAmount = 1f;

    FirstPersonController _controller;
    float storedRotationSpeed;

    private StarterAssetsInputs _input;

    void Start()
    {
        _controller = GetComponent<FirstPersonController>();
        _input = GetComponent<StarterAssetsInputs>();

        storedRotationSpeed = _controller.RotationSpeed;

    }

    void Update()
    {
        Vector3 moveInput = new Vector3(_input.look.x, 0, _input.look.y);

        if (Input.GetMouseButton(1))
        {
            _controller.RotationSpeed = 0;
            gun.transform.Translate(moveInput * shakeAmount * Time.deltaTime);
        }
        if(Input.GetMouseButtonUp(1))
        {
            _controller.RotationSpeed = storedRotationSpeed;
        }
    }
}
