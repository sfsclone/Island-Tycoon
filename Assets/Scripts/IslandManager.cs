using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    public static IslandManager Instance;

    public GameObject islandPrefab; // Assign your island prefab in the Inspector
    public GameObject shadowIslandPrefab; // Assign a transparent/ghost version of the island
    public Transform islandHolder; // A parent object to keep islands organized
    public int islandSize = 20; // Size of each island (assuming 20x1x20)
    private List<Vector3> islandPositions = new List<Vector3>();

    private int islandCost = 10; // Initial cost to buy an island
    private GameObject currentShadowIsland; // Stores the active shadow island

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        islandPositions.Add(Vector3.zero);
        CreateShadowIsland();
    }

    private void CreateShadowIsland()
    {
        if (shadowIslandPrefab != null)
        {
            currentShadowIsland = Instantiate(shadowIslandPrefab, Vector3.zero, Quaternion.identity);
            currentShadowIsland.SetActive(true);
            Debug.Log("Shadow Island Created!");
        }
        else
        {
            Debug.LogError("Shadow Island Prefab is missing!");
        }
    }

    public void BuyNewIsland(Vector3 playerPosition)
    {
        if (Inventory.Instance.GetResourceAmount("Coin") < islandCost)
        {
            Debug.Log("Not enough coins to buy a new island!");
            return;
        }

        Inventory.Instance.RemoveResource("Coin", islandCost);
        islandCost += 5;

        Vector3 newIslandPosition = FindNextIslandPosition(playerPosition);
        GameObject newIsland = Instantiate(islandPrefab, newIslandPosition, Quaternion.identity, islandHolder);
        islandPositions.Add(newIslandPosition);

        Debug.Log("New island purchased at: " + newIslandPosition);
        UpdateShadowIsland(playerPosition);
    }

    public void UpdateShadowIsland(Vector3 playerPosition)
    {
        if (currentShadowIsland == null) return;

        Vector3 nextPosition = FindNextIslandPosition(playerPosition);

        if (!islandPositions.Contains(nextPosition))
        {
            currentShadowIsland.transform.position = nextPosition;
            currentShadowIsland.SetActive(true); // Show the shadow
        }
        else
        {
            currentShadowIsland.SetActive(false); // Hide if no valid location
        }
    }

    private Vector3 FindNextIslandPosition(Vector3 playerPosition)
    {
        Vector3 snappedPlayerPos = new Vector3(
            Mathf.Round(playerPosition.x / islandSize) * islandSize,
            0,
            Mathf.Round(playerPosition.z / islandSize) * islandSize
        );

        float offsetX = playerPosition.x - snappedPlayerPos.x;
        float offsetZ = playerPosition.z - snappedPlayerPos.z;

        Vector3 firstChoice = Mathf.Abs(offsetX) > Mathf.Abs(offsetZ)
            ? (offsetX > 0 ? snappedPlayerPos + new Vector3(islandSize, 0, 0) : snappedPlayerPos + new Vector3(-islandSize, 0, 0))
            : (offsetZ > 0 ? snappedPlayerPos + new Vector3(0, 0, islandSize) : snappedPlayerPos + new Vector3(0, 0, -islandSize));

        Vector3[] possiblePositions = new Vector3[]
        {
            firstChoice,
            snappedPlayerPos + new Vector3(islandSize, 0, 0),
            snappedPlayerPos + new Vector3(-islandSize, 0, 0),
            snappedPlayerPos + new Vector3(0, 0, islandSize),
            snappedPlayerPos + new Vector3(0, 0, -islandSize)
        };

        foreach (Vector3 candidate in possiblePositions)
        {
            if (!islandPositions.Contains(candidate))
            {
                return candidate;
            }
        }

        return firstChoice;
    }

    public List<Vector3> GetIslandPositions()
    {
        return islandPositions;
    }

    private void Update()
    {
        if (PlayerController.Instance != null)
        {
            UpdateShadowIsland(PlayerController.Instance.transform.position);
        }
    }

}
