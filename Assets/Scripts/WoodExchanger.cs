using UnityEngine;
using System.Collections;

public class WoodExchanger : MonoBehaviour
{
    public float exchangeInterval = 1.0f; // Time between each exchange
    private bool isExchanging = false;
    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (!isExchanging)
                StartCoroutine(ExchangeWood());
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
        if (playerInRange && !isExchanging && Inventory.Instance.GetResourceAmount("Wood") > 0)
        {
            StartCoroutine(ExchangeWood());
        }
    }

    private IEnumerator ExchangeWood()
    {
        isExchanging = true;

        while (playerInRange && Inventory.Instance.GetResourceAmount("Wood") > 0)
        {
            Inventory.Instance.RemoveResource("Wood", 1);
            Inventory.Instance.AddResource("Coin", 1);
            yield return new WaitForSeconds(exchangeInterval);
        }

        isExchanging = false;
    }
}
