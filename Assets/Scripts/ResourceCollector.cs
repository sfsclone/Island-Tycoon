using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    public float collectionRadius = 2f; // Distance to collect resources automatically
    public LayerMask resourceLayer; // To filter only resource items

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, collectionRadius, resourceLayer);
        foreach (var collider in hitColliders)
        {
            if (collider.TryGetComponent(out ResourceItem resourceItem))
            {
                resourceItem.Collect();
            }
        }
    }
}
