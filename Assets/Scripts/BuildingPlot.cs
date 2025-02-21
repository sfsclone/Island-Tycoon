//BuildingPlot.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingPlot : MonoBehaviour
{
    public GameObject finishedBuilding;
    public int requiredWood = 5;
    private int currentWood = 0;
    public float buildInterval = 1.0f; // Time between each wood submission
    private bool isBuilding = false;
    public Slider buildProgressBar; // Assign in Inspector
    private bool playerInRange = false;

    private void Start()
    {
        if (buildProgressBar != null)
        {
            buildProgressBar.gameObject.SetActive(false); // Hide slider at start
            buildProgressBar.maxValue = requiredWood;
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

        while (currentWood < requiredWood && Inventory.Instance.GetResourceAmount("Wood") > 0)
        {
            if (!playerInRange)
            {
                isBuilding = false;
                yield break; // Stop coroutine if player leaves
            }

            Inventory.Instance.RemoveResource("Wood", 1);
            currentWood++;

            // Update slider
            if (buildProgressBar != null)
                buildProgressBar.value = currentWood;

            yield return new WaitForSeconds(buildInterval);
        }

        if (currentWood >= requiredWood)
        {
            // Find the ground position to spawn the finished building
            Vector3 spawnPosition = GetGroundPosition(transform.position);

            // Adjust the spawn position slightly above the ground level to avoid clipping
            spawnPosition.y += 2.25f; // Adjust as needed based on the size of your building

            // Instantiate the finished building at ground level with slight offset
            Instantiate(finishedBuilding, spawnPosition, Quaternion.identity);

            // Remove the building plot after the building is complete
            Destroy(gameObject);
        }

        if (buildProgressBar != null)
            buildProgressBar.gameObject.SetActive(false); // Hide slider after completion

        isBuilding = false;
    }

    // Function to get the ground position using Raycast
    private Vector3 GetGroundPosition(Vector3 position)
    {
        RaycastHit hit;
        // Cast a ray downwards from a high point (e.g., 100 units above the position)
        if (Physics.Raycast(position + Vector3.up * 100f, Vector3.down, out hit, 200f)) // increased range
        {
            return hit.point; // Return the point where the ray hits the ground
        }
        return position; // If no ground is found, return the original position (fallback)
    }
}
