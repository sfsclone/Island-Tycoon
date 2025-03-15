using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tree : MonoBehaviour
{
    public GameObject woodPrefab;
    public int woodAmount = 5;
    public float chopTime = 3f; // Time required to chop
    public float regrowTime = 10f; // Time before the tree regrows
    public Slider chopProgressBar;

    private bool isChopping = false;
    private bool isTreeActive = true; // Tracks if tree is available for chopping
    private MeshRenderer treeRenderer;
    private Collider treeCollider;
    private List<Collider> entitiesInRange = new List<Collider>(); // Tracks NPCs/players in range

    private void Start()
    {
        treeRenderer = GetComponent<MeshRenderer>();
        treeCollider = GetComponent<Collider>();
        if (chopProgressBar != null)
            chopProgressBar.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTreeActive && (other.CompareTag("Player") || other.CompareTag("NPC")))
        {
            if (!entitiesInRange.Contains(other))
                entitiesInRange.Add(other); // Track NPCs/players in range

            if (chopProgressBar != null)
                chopProgressBar.gameObject.SetActive(true);

            StartChopping();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (entitiesInRange.Contains(other))
            entitiesInRange.Remove(other); // Remove NPC/player when they leave

        if (entitiesInRange.Count == 0)
        {
            if (chopProgressBar != null)
                chopProgressBar.gameObject.SetActive(false);
            StopAllCoroutines();
            isChopping = false;
        }
    }

    public void StartChopping()
    {
        if (isTreeActive && !isChopping && entitiesInRange.Count > 0)
        {
            StartCoroutine(ChopTreeCoroutine());
        }
    }

    private IEnumerator ChopTreeCoroutine()
    {
        isChopping = true;
        if (chopProgressBar != null)
            chopProgressBar.value = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < chopTime)
        {
            if (entitiesInRange.Count == 0)
            {
                isChopping = false;
                if (chopProgressBar != null)
                    chopProgressBar.value = 0f;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            if (chopProgressBar != null)
                chopProgressBar.value = elapsedTime / chopTime;
            yield return null;
        }

        ChopTree();
        isChopping = false;
    }

    private void ChopTree()
    {
        isTreeActive = false;

        GameObject wood = Instantiate(woodPrefab, transform.position, Quaternion.identity);
        Rigidbody rb = wood.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(new Vector3(Random.Range(-1f, 1f), 10f, Random.Range(-1f, 1f)), ForceMode.Impulse);
        }

        ResourceItem resourceItem = wood.GetComponent<ResourceItem>();
        if (resourceItem != null)
        {
            resourceItem.resourceAmount = woodAmount;
        }

        // **Clear the list so NPCs/players are not remembered**
        entitiesInRange.Clear();

        StartCoroutine(RegrowTree());
    }

    private IEnumerator RegrowTree()
    {
        treeRenderer.enabled = false;
        treeCollider.enabled = false;
        if (chopProgressBar != null)
            chopProgressBar.gameObject.SetActive(false);

        yield return new WaitForSeconds(regrowTime);

        treeRenderer.enabled = true;
        treeCollider.enabled = true;
        isTreeActive = true;
    }

    public bool IsTreeActive() => isTreeActive; // NPC can check if tree is still available
}
