using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RockBreaking : MonoBehaviour
{
    public GameObject stonePrefab;
    public int stoneAmount = 5;
    public float breakTime = 3f; // Time required to break
    public Slider breakProgressBar;
    private bool isBreaking = false;
    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            breakProgressBar.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            breakProgressBar.gameObject.SetActive(false);
            StopCoroutine(BreakRockCoroutine()); // Stop breaking if player leaves
        }
    }

    public void StartBreaking()
    {
        if (!isBreaking && playerInRange)
        {
            StartCoroutine(BreakRockCoroutine());
        }
    }

    private IEnumerator BreakRockCoroutine()
    {
        isBreaking = true;
        float elapsedTime = 0f;
        breakProgressBar.value = 0f;

        while (elapsedTime < breakTime)
        {
            if (!playerInRange) // Stop if player moves away
            {
                isBreaking = false;
                breakProgressBar.value = 0f;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            breakProgressBar.value = elapsedTime / breakTime;
            yield return null;
        }

        BreakRock();
        breakProgressBar.gameObject.SetActive(false);
        isBreaking = false;
    }

    private void BreakRock()
    {
        // Spawn only 1 stone prefab instead of 5
        GameObject stone = Instantiate(stonePrefab, transform.position, Quaternion.identity);

        // Add some force for effect
        Rigidbody rb = stone.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(new Vector3(Random.Range(-1f, 1f), 10f, Random.Range(-1f, 1f)), ForceMode.Impulse);
        }

        // Set wood amount on the prefab
        ResourceItem resourceItem = stone.GetComponent<ResourceItem>();
        if (resourceItem != null)
        {
            resourceItem.resourceAmount = stoneAmount; // This will be 5 (set in inspector)
        }


        // Destroy the rock after breaking
        Destroy(gameObject);
    }
}
