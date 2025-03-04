using UnityEngine;
using System.Collections;

public class ResourceExchanger : MonoBehaviour
{
    public float exchangeInterval = 1.0f; // Time between each exchange
    private bool isExchanging = false;
    private bool playerInRange = false;
    public ResourceItem.ResourceType resourceType = ResourceItem.ResourceType.Wood; // Default resource type

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

            // Remove visually stacked resource
            ResourceItem.RemoveResourceVisual(resourceType);

            yield return new WaitForSeconds(exchangeInterval);
        }

        isExchanging = false;
    }
}
