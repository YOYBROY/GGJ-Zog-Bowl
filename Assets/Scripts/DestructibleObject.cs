using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [SerializeField] private GameObject brokenVariant;
    [SerializeField] private float forceMultiplier = 200.0f;
    [SerializeField] private Transform alternativeTransformPosition;

    public void SwapModel()
    {
        Transform spawnPosition = alternativeTransformPosition != null ? alternativeTransformPosition : transform;
        GameObject spawnedVariant = Instantiate(brokenVariant, spawnPosition.position, Quaternion.identity);

        foreach (Transform child in spawnedVariant.transform)
        {
            GameObject piece = child.gameObject;
            piece.layer = 6;

            MeshCollider collider = piece.AddComponent<MeshCollider>();
            collider.convex = true;
            Rigidbody rb = piece.AddComponent<Rigidbody>();

            Vector3 force = (piece.transform.position - transform.position).normalized * forceMultiplier;
            rb.AddForceAtPosition(force, transform.position);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("PlayerProjectile"))
        {
            SwapModel();
        }
    }
}