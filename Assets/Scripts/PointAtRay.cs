using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtRay : MonoBehaviour
{
    Camera cam;

    void Start()
    {
        cam = Camera.main;   
    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.MaxValue))
        {
            transform.LookAt(hit.point, Vector3.up);
            transform.Rotate(new Vector3(1.0f, 0, 0), 90);
        }
    }
}
