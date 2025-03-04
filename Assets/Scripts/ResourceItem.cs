using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceItem : MonoBehaviour
{
    public enum ResourceType { Wood, Stone }
    public ResourceType resourceType;

    public float pickupRange = 0.5f;
    public int resourceAmount = 1;

    private Transform player;
    private bool collected = false; // Prevents double collection

    private static Dictionary<ResourceType, List<GameObject>> collectedResources = new Dictionary<ResourceType, List<GameObject>>()
    {
        { ResourceType.Wood, new List<GameObject>() },
        { ResourceType.Stone, new List<GameObject>() }
    };

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collected && other.CompareTag("Player"))
        {
            collected = true;
            Collect();
        }
    }

    public void Collect()
    {
        Inventory.Instance.AddResource(resourceType.ToString(), resourceAmount);
        AttachToPlayer();
        GetComponent<Collider>().enabled = false;
    }

    private void AttachToPlayer()
    {
        transform.SetParent(player, true); // Keep world position when attaching

        // Ensure dictionary contains the resource type
        if (!collectedResources.ContainsKey(resourceType))
        {
            collectedResources[resourceType] = new List<GameObject>();
        }

        // Add the current resource to the correct list
        collectedResources[resourceType].Add(gameObject);
        int stackIndex = collectedResources[resourceType].Count - 1;

        Debug.Log($"Stacking {resourceType} at index {stackIndex}");

        int rowSize = 2; // Number of items per row
        float horizontalSpacing = 0.3f;
        float verticalSpacing = 0.4f;

        // Adjust stacking base position per resource type
        float offset = (resourceType == ResourceItem.ResourceType.Wood) ? -0.3f : 0.3f; // Separate Wood & Stone
        Vector3 basePosition = player.position - (player.forward * 0.6f) + (Vector3.up * 0.5f) + (player.right * offset);

        // Calculate row & column positions
        int row = stackIndex / rowSize;
        int col = stackIndex % rowSize;

        Vector3 stackedPosition = basePosition
                                + (player.right * (col - 0.5f) * horizontalSpacing) // Center items
                                + (Vector3.up * row * verticalSpacing); // Stack height

        transform.position = stackedPosition;
        transform.rotation = player.rotation; // Align with player rotation

        // Disable physics
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            Destroy(rb);
        }
    }




    public static void RemoveResourceVisual(ResourceType type)
    {
        if (collectedResources[type].Count > 0)
        {
            GameObject resourceToRemove = collectedResources[type][collectedResources[type].Count - 1];
            collectedResources[type].RemoveAt(collectedResources[type].Count - 1);
            Destroy(resourceToRemove);
        }
    }
}
