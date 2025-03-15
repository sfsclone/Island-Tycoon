using UnityEngine;
using System.Collections;

public class ResourceExchanger : MonoBehaviour
{
    public float exchangeInterval = 1.0f; // Time between each exchange
    private bool isExchanging = false;
    private bool playerInRange = false;
    public ResourceItem.ResourceType resourceType = ResourceItem.ResourceType.Wood; // Default resource type

    public Transform spawnPoint; // Where the new resource will spawn
    public GameObject plankPrefab;
    public GameObject brickPrefab;

    private int exchangeCounter = 0; // Track exchanged units

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (!isExchanging)
                StartCoroutine(ExchangeResource());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && !isExchanging && Inventory.Instance.GetResourceAmount(resourceType.ToString()) > 0)
        {
            StartCoroutine(ExchangeResource());
        }
    }

    private IEnumerator ExchangeResource()
    {
        isExchanging = true;

        while (playerInRange && Inventory.Instance.GetResourceAmount(resourceType.ToString()) > 0)
        {
            Inventory.Instance.RemoveResource(resourceType.ToString(), 1);
            Inventory.Instance.AddResource("Coin", 1);
            exchangeCounter++;

            // Only spawn prefab when 5 resources are exchanged
            if (exchangeCounter >= 5)
            {
                SpawnConvertedResource();
                exchangeCounter = 0; // Reset counter
            }

            yield return new WaitForSeconds(exchangeInterval);

            // Remove visually stacked resource
            ResourceItem.RemoveResourceVisual(resourceType);
        }

        isExchanging = false;
    }

    private void SpawnConvertedResource()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is not assigned in ResourceExchanger!");
            return;
        }

        GameObject newResource = null;
        if (resourceType == ResourceItem.ResourceType.Wood && plankPrefab != null)
        {
            newResource = Instantiate(plankPrefab, spawnPoint.position, Quaternion.identity);
        }
        else if (resourceType == ResourceItem.ResourceType.Rock && brickPrefab != null)
        {
            newResource = Instantiate(brickPrefab, spawnPoint.position, Quaternion.identity);
        }

        if (newResource != null && newResource.TryGetComponent<ResourceItem>(out ResourceItem resourceItem))
        {
            resourceItem.resourceAmount = 5; // Set to 5 units per prefab
        }
    }
}
