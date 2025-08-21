using UnityEngine;

public class PickupMovement: MonoBehaviour
{
    [SerializeField] float spinSpeed = 1.0f;
    [SerializeField] float spinSpeedVariance = 0.5f;
    [SerializeField] bool randomRotationDirection;
     enum ROTATEAXIS { X, Y, Z };
    [SerializeField] ROTATEAXIS rotateAxis;

    [SerializeField] bool bob;
    [SerializeField] float bobHeight = 1.0f;
    [SerializeField] float bobSpeed = 1.0f;
    [SerializeField] float bobSpeedVariance = 0.4f;
    private float bobOffset;

    private Vector3 rotationVector;
    private Vector3 originalPosition;

    private void Start()
    {
        switch (rotateAxis)
        {
            case ROTATEAXIS.X:
                rotationVector = new Vector3(1, 0, 0);
                break;
            case ROTATEAXIS.Y:
                rotationVector = new Vector3(0, 1, 0);
                break;
            case ROTATEAXIS.Z:
                rotationVector = new Vector3(0, 0, 1);
                break;
            default:
                break;
        }
        originalPosition = transform.localPosition;
        transform.Rotate(rotationVector * Random.Range(0.0f, 360f));
        spinSpeed += Random.Range(-spinSpeedVariance, spinSpeedVariance);
        bobSpeed += Random.Range(-bobSpeedVariance, bobSpeedVariance);
        bobOffset = Random.Range(-bobSpeed, bobSpeed);

        if (randomRotationDirection)
        {
            if (Random.Range(0.0f, 1f) > 0.5f)
            {
                rotationVector *= -1;
            }
        }
    }

    void Update()
    {
        transform.Rotate(rotationVector * spinSpeed * Time.deltaTime);

        if(bob)
        {
            Vector3 moveDir = new Vector3(0, Mathf.Sin(bobOffset + Time.time * bobSpeed) * bobHeight, 0);

            Vector3 newPos = originalPosition + moveDir;

            transform.localPosition = newPos;
        }
    }
}
