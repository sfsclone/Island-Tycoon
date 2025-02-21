using UnityEngine;
using System.Collections;

public class WoodItem : MonoBehaviour
{
    public float attractionSpeed = 5f;
    public float pickupRange = 0.5f;
    public int woodAmount = 1; // Default to 1, can be set to 5 from TreeChopping

    private Transform player;
    private bool isMovingToPlayer = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isMovingToPlayer)
        {
            StartCoroutine(MoveToPlayer());
        }
    }

    private IEnumerator MoveToPlayer()
    {
        isMovingToPlayer = true;

        while (Vector3.Distance(transform.position, player.position) > pickupRange)
        {
            transform.position = Vector3.Lerp(transform.position, player.position, attractionSpeed * Time.deltaTime);
            yield return null;
        }

        // Give the player the correct amount of wood
        Inventory.Instance.AddResource("Wood", woodAmount);

        // Destroy the wood object after collection
        Destroy(gameObject);
    }

    public void Collect()
    {
        Inventory.Instance.AddResource("Wood", woodAmount);
        Destroy(gameObject);
    }
}
