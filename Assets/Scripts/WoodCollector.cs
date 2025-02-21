using UnityEngine;

public class WoodCollector : MonoBehaviour
{
    public float collectionRadius = 2f; // Distance to collect wood automatically

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, collectionRadius);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Wood"))
            {
                collider.GetComponent<WoodItem>().Collect();
            }
        }
    }
}
