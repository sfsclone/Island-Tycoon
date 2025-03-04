using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    private Dictionary<string, int> items = new Dictionary<string, int>();
    private Dictionary<string, List<GameObject>> collectedResources = new Dictionary<string, List<GameObject>>(); // Stores actual objects
    public InventoryUI inventoryUI; // UI Reference

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Add resource and store its GameObject
    public void AddResource(string resourceName, int amount, GameObject resourceObject = null)
    {
        // Track counts
        if (!items.ContainsKey(resourceName))
            items[resourceName] = 0;
        items[resourceName] += amount;

        // Track GameObjects for stacking
        if (!collectedResources.ContainsKey(resourceName))
            collectedResources[resourceName] = new List<GameObject>();

        collectedResources[resourceName].Add(resourceObject);

        // Update UI
        inventoryUI?.UpdateInventory(items);
    }

    // Get count of a resource
    public int GetResourceAmount(string resourceName)
    {
        if (items.ContainsKey(resourceName))
            return items[resourceName];
        return 0;
    }

    // Remove resource and delete its GameObject
    public void RemoveResource(string resourceName, int amount)
    {
        if (items.ContainsKey(resourceName))
        {
            items[resourceName] -= amount;
            if (items[resourceName] <= 0)
                items.Remove(resourceName);

            // Also remove the physical GameObjects
            if (collectedResources.ContainsKey(resourceName))
            {
                for (int i = 0; i < amount; i++)
                {
                    if (collectedResources[resourceName].Count > 0)
                    {
                        GameObject obj = collectedResources[resourceName][0]; // Get first collected object
                        collectedResources[resourceName].RemoveAt(0);
                        Destroy(obj); // Destroy the object in the scene
                    }
                }

                // Remove the key if no more objects left
                if (collectedResources[resourceName].Count == 0)
                    collectedResources.Remove(resourceName);
            }

            // Update UI
            inventoryUI?.UpdateInventory(items);
        }
    }

    // Get the list of collected GameObjects for a resource
    public List<GameObject> GetCollectedResources(string resourceName)
    {
        if (collectedResources.ContainsKey(resourceName))
            return collectedResources[resourceName];
        return new List<GameObject>();
    }
}
