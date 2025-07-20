using UnityEngine;

public class PointAtRay : MonoBehaviour
{
    Camera cam;
    [SerializeField] LayerMask layerMask;

    void Start()
    {
        cam = Camera.main;   
    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
        {
            transform.LookAt(hit.point, Vector3.up);
            transform.Rotate(new Vector3(1.0f, 0, 0), 90);
        }
    }
}
