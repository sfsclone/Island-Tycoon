using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Stone : MonoBehaviour
{
    public GameObject rockPrefab;
    public int rockAmount = 5;
    public float breakTime = 3f; // Time required to break
    public float regrowTime = 10f; // Time before the stone reappears
    public Slider breakProgressBar;

    private bool isBreaking = false;
    private bool isStoneActive = true; // Tracks if stone is available for breaking
    private MeshRenderer stoneRenderer;
    private Collider stoneCollider;
    private List<Collider> entitiesInRange = new List<Collider>(); // Tracks NPCs/players in range

    private void Start()
    {
        stoneRenderer = GetComponent<MeshRenderer>();
        stoneCollider = GetComponent<Collider>();
        if (breakProgressBar != null)
            breakProgressBar.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isStoneActive && (other.CompareTag("Player") || other.CompareTag("NPC")))
        {
            if (!entitiesInRange.Contains(other))
                entitiesInRange.Add(other); // Track NPCs/players in range

            if (breakProgressBar != null)
                breakProgressBar.gameObject.SetActive(true);

            StartBreaking();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (entitiesInRange.Contains(other))
            entitiesInRange.Remove(other); // Remove NPC/player when they leave

        if (entitiesInRange.Count == 0)
        {
            if (breakProgressBar != null)
                breakProgressBar.gameObject.SetActive(false);
            StopAllCoroutines();
            isBreaking = false;
        }
    }

    public void StartBreaking()
    {
        if (isStoneActive && !isBreaking && entitiesInRange.Count > 0)
        {
            StartCoroutine(BreakStoneCoroutine());
        }
    }

    private IEnumerator BreakStoneCoroutine()
    {
        isBreaking = true;
        if (breakProgressBar != null)
            breakProgressBar.value = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < breakTime)
        {
            if (entitiesInRange.Count == 0)
            {
                isBreaking = false;
                if (breakProgressBar != null)
                    breakProgressBar.value = 0f;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            if (breakProgressBar != null)
                breakProgressBar.value = elapsedTime / breakTime;
            yield return null;
        }

        BreakStone();
        isBreaking = false;
    }

    private void BreakStone()
    {
        isStoneActive = false;

        GameObject rock = Instantiate(rockPrefab, transform.position, Quaternion.identity);
        Rigidbody rb = rock.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(new Vector3(Random.Range(-1f, 1f), 10f, Random.Range(-1f, 1f)), ForceMode.Impulse);
        }

        ResourceItem resourceItem = rock.GetComponent<ResourceItem>();
        if (resourceItem != null)
        {
            resourceItem.resourceAmount = rockAmount;
        }

        // **Clear the list so NPCs/players are not remembered**
        entitiesInRange.Clear();

        StartCoroutine(RegrowStone());
    }

    private IEnumerator RegrowStone()
    {
        stoneRenderer.enabled = false;
        stoneCollider.enabled = false;
        if (breakProgressBar != null)
            breakProgressBar.gameObject.SetActive(false);

        yield return new WaitForSeconds(regrowTime);

        stoneRenderer.enabled = true;
        stoneCollider.enabled = true;
        isStoneActive = true;
    }

    public bool IsStoneActive() => isStoneActive; // NPC can check if stone is still available
}
