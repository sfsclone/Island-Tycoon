using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingPlot : MonoBehaviour
{
    public GameObject finishedBuilding;
    public ResourceItem.ResourceType requiredResource = ResourceItem.ResourceType.Wood; // Default required resource
    public int requiredAmount = 5;
    private int currentAmount = 0;
    public float buildInterval = 1.0f; // Time between each resource submission
    private bool isBuilding = false;
    public Slider buildProgressBar; // Assign in Inspector
    private bool playerInRange = false;

    private void Start()
    {
        if (buildProgressBar != null)
        {
            buildProgressBar.gameObject.SetActive(false); // Hide slider at start
            buildProgressBar.maxValue = requiredAmount;
            buildProgressBar.value = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isBuilding)
        {
            playerInRange = true;
            if (buildProgressBar != null)
                buildProgressBar.gameObject.SetActive(true); // Show slider

            StartCoroutine(BuildingProcess());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private IEnumerator BuildingProcess()
    {
        isBuilding = true;

        while (currentAmount < requiredAmount && Inventory.Instance.GetResourceAmount(requiredResource.ToString()) > 0)
        {
            if (!playerInRange)
            {
                isBuilding = false;
                yield break; // Stop coroutine if player leaves
            }

            Inventory.Instance.RemoveResource(requiredResource.ToString(), 1);
            currentAmount++;

            // Update slider
            if (buildProgressBar != null)
                buildProgressBar.value = currentAmount;

            yield return new WaitForSeconds(buildInterval);
        }

        if (currentAmount >= requiredAmount)
        {
            // Get the building's height dynamically
            float buildingHeight = finishedBuilding.GetComponent<Renderer>().bounds.size.y / 2f;

            // Find the correct ground position and apply height offset
            Vector3 spawnPosition = GetGroundPosition(transform.position, buildingHeight);

            // Instantiate the finished building
            Instantiate(finishedBuilding, spawnPosition, Quaternion.identity);

            // Remove the building plot after the building is complete
            Destroy(gameObject);
        }

        if (buildProgressBar != null)
            buildProgressBar.gameObject.SetActive(false); // Hide slider after completion

        ResourceItem.RemoveResourceVisual(requiredResource); // Remove visual representation
        isBuilding = false;
    }

    // Function to get the ground position with a height offset
    // Adjust spawn position to ensure the bottom aligns with the ground
    private Vector3 GetGroundPosition(Vector3 position, float heightOffset)
    {
        RaycastHit hit;
        int groundLayerMask = LayerMask.GetMask("Ground");

        if (Physics.Raycast(position + Vector3.up * 100f, Vector3.down, out hit, 200f, groundLayerMask))
        {
            return hit.point + Vector3.up * (heightOffset / 2f); // Use half-height for proper alignment
        }
        return position;
    }
}
