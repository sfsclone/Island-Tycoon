using UnityEngine;
using System.Collections.Generic;

public class ResourceCollector : MonoBehaviour
{
    public float collectionRadius = 2f;

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, collectionRadius);
        foreach (var collider in hitColliders)
        {
            if (collider.TryGetComponent(out ResourceItem resourceItem))
            {
                resourceItem.Collect(); // Instantly collect without delay
            }
        }
    }
}
