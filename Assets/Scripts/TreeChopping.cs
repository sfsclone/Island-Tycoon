using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TreeChopping : MonoBehaviour
{
    public GameObject woodPrefab;
    public int woodAmount = 5;
    public float chopTime = 3f; // Time required to chop
    public Slider chopProgressBar;
    private bool isChopping = false;
    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            chopProgressBar.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            chopProgressBar.gameObject.SetActive(false);
            StopCoroutine(ChopTreeCoroutine()); // Stop chopping if player leaves
        }
    }

    public void StartChopping()
    {
        if (!isChopping && playerInRange)
        {
            StartCoroutine(ChopTreeCoroutine());
        }
    }

    private IEnumerator ChopTreeCoroutine()
    {
        isChopping = true;
        float elapsedTime = 0f;
        chopProgressBar.value = 0f;

        while (elapsedTime < chopTime)
        {
            if (!playerInRange) // Stop if player moves away
            {
                isChopping = false;
                chopProgressBar.value = 0f;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            chopProgressBar.value = elapsedTime / chopTime;
            yield return null;
        }

        ChopTree();
        chopProgressBar.gameObject.SetActive(false);
        isChopping = false;
    }

    private void ChopTree()
    {
        // Spawn only 1 wood prefab instead of 5
        GameObject wood = Instantiate(woodPrefab, transform.position, Quaternion.identity);

        // Add some force for effect
        Rigidbody rb = wood.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(new Vector3(Random.Range(-1f, 1f), 10f, Random.Range(-1f, 1f)), ForceMode.Impulse);
        }

        // Set wood amount on the prefab
        WoodItem woodItem = wood.GetComponent<WoodItem>();
        if (woodItem != null)
        {
            woodItem.woodAmount = woodAmount; // This will be 5 (set in inspector)
        }

        // Destroy the tree after spawning wood
        Destroy(gameObject);
    }

}
