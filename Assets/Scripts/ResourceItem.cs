using System.Collections.Generic;
using UnityEngine;

public class ResourceItem : MonoBehaviour
{
    public enum ResourceType { Wood, Rock, Plank, Brick }
    public ResourceType resourceType;

    public int resourceAmount = 5; // Default amount per prefab

    private bool collected = false; // Prevents duplicate collection
    private static List<GameObject> collectedResources = new List<GameObject>();
    private static Transform player;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public void Collect()
    {
        if (collected) return;

        collected = true;
        Inventory.Instance.AddResource(resourceType.ToString(), resourceAmount);
        AttachToPlayer();
        GetComponent<Collider>().enabled = false;
    }

    private void AttachToPlayer()
    {
        collectedResources.Add(gameObject);
        int stackIndex = collectedResources.Count - 1;

        Debug.Log($"Stacking {resourceType} at index {stackIndex}");

        float verticalSpacing = 0.4f;

        // Base position relative to the player
        Vector3 basePosition = player.position - (player.forward * 0.6f) + (Vector3.up * 0.5f);

        // Stack all items in a single vertical stack
        Vector3 stackedPosition = basePosition + (Vector3.up * stackIndex * verticalSpacing);

        transform.position = stackedPosition;
        transform.rotation = player.rotation;
        transform.SetParent(player, true);

        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            Destroy(rb);
        }
    }

    public static void RemoveResourceVisual(ResourceType type)
    {
        // Find the first resource of the specified type
        GameObject resourceToRemove = collectedResources.Find(resource =>
            resource.GetComponent<ResourceItem>().resourceType == type);

        if (resourceToRemove != null)
        {
            collectedResources.Remove(resourceToRemove);
            Destroy(resourceToRemove);
            ReorganizeStack(); // Adjust stacking after removal
        }
    }

    private static void ReorganizeStack()
    {
        float verticalSpacing = 0.4f;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        Vector3 basePosition = player.position - (player.forward * 0.6f) + (Vector3.up * 0.5f);

        for (int i = 0; i < collectedResources.Count; i++)
        {
            if (collectedResources[i] != null)
            {
                Vector3 stackedPosition = basePosition + (Vector3.up * i * verticalSpacing);
                collectedResources[i].transform.position = stackedPosition;
            }
        }
    }
}
