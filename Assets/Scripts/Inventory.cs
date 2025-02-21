using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    private Dictionary<string, int> items = new Dictionary<string, int>();
    public InventoryUI inventoryUI; // UI Reference

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void AddResource(string resourceName, int amount)
    {
        if (!items.ContainsKey(resourceName))
            items[resourceName] = 0;

        items[resourceName] += amount;

        // Update UI whenever inventory changes
        if (inventoryUI != null)
        {
            inventoryUI.UpdateInventory(items);
        }
    }

    // building
    public int GetResourceAmount(string resourceName)
    {
        if (items.ContainsKey(resourceName))
            return items[resourceName];
        return 0;
    }


    public void RemoveResource(string resourceName, int amount)
    {
        if (items.ContainsKey(resourceName))
        {
            items[resourceName] -= amount;
            if (items[resourceName] <= 0)
                items.Remove(resourceName);

            inventoryUI?.UpdateInventory(items);
        }
    }

}
