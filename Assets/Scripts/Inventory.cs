using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    private Dictionary<string, int> items = new Dictionary<string, int>();
    private Dictionary<string, List<GameObject>> collectedResources = new Dictionary<string, List<GameObject>>(); // Stores actual objects
    public InventoryUI inventoryUI; // UI Reference

    private void Start()
    {
        Inventory.Instance.AddResource("Coin", 100); // Give coins at the start of the game
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Add resource and store its GameObject
    public void AddResource(string resourceName, int amount, GameObject resourceObject = null)
    {
        if (!items.ContainsKey(resourceName))
            items[resourceName] = 0;
        items[resourceName] += amount;

        if (resourceObject != null)
        {
            if (!collectedResources.ContainsKey(resourceName))
                collectedResources[resourceName] = new List<GameObject>();

            collectedResources[resourceName].Add(resourceObject);
        }

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

            if (collectedResources.ContainsKey(resourceName))
            {
                for (int i = 0; i < amount; i++)
                {
                    if (collectedResources[resourceName].Count > 0)
                    {
                        GameObject obj = collectedResources[resourceName][0];
                        collectedResources[resourceName].RemoveAt(0);
                        Destroy(obj);
                    }
                }

                if (collectedResources[resourceName].Count == 0)
                    collectedResources.Remove(resourceName);
            }

            inventoryUI?.UpdateInventory(items);
        }
    }

    // Get collected GameObjects for a resource
    public List<GameObject> GetCollectedResources(string resourceName)
    {
        if (collectedResources.ContainsKey(resourceName))
            return collectedResources[resourceName];
        return new List<GameObject>();
    }

    // npc
    public bool HasEnoughCoins(int amount)
    {
        return GetResourceAmount("Coin") >= amount;
    }

    public void SpendCoins(int amount)
    {
        RemoveResource("Coin", amount);
    }
}
