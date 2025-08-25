using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    [SerializeField] private GameObject brokenVariant;
    [SerializeField] private float forceMultiplier = 200.0f;
    [SerializeField] private Transform alternativeTransformPosition;

    private int killCounter;

    public void SwapModel()
    {
        killCounter++;
    }

    public void SwapEnemyModel()
    {
        Transform spawnPosition = alternativeTransformPosition != null ? alternativeTransformPosition : transform;
        GameObject spawnedVariant = Instantiate(brokenVariant, spawnPosition.position, transform.rotation);

        foreach (Transform child in spawnedVariant.transform)
        {
            GameObject piece = child.gameObject;
            piece.layer = 6;

            piece.transform.localScale = transform.localScale;

            MeshCollider collider = piece.AddComponent<MeshCollider>();
            collider.convex = true;
            Rigidbody rb = piece.AddComponent<Rigidbody>();

            Vector3 force = (piece.transform.position - transform.position).normalized * forceMultiplier;
            rb.AddForceAtPosition(force, transform.position);
        }
        Destroy(gameObject);
    }

    private void LateUpdate()
    {
        if(killCounter > 0)
        {
            Transform spawnPosition = alternativeTransformPosition != null ? alternativeTransformPosition : transform;
            GameObject spawnedVariant = Instantiate(brokenVariant, spawnPosition.position, transform.rotation);

            foreach (Transform child in spawnedVariant.transform)
            {
                GameObject piece = child.gameObject;
                piece.layer = 6;

                piece.transform.localScale = transform.localScale;

                MeshCollider collider = piece.AddComponent<MeshCollider>();
                collider.convex = true;
                Rigidbody rb = piece.AddComponent<Rigidbody>();

                Vector3 force = (piece.transform.position - transform.position).normalized * forceMultiplier;
                rb.AddForceAtPosition(force, transform.position);
            }
            Destroy(gameObject);
        }
    }
}