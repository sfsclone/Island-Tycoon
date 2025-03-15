using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class NPCManager : MonoBehaviour
{
    public GameObject npcPrefab;
    public Transform spawnPoint;
    public TMP_Text npcPriceText; // Use TMP_Text instead of Text
    private int npcPrice = 50; // Initial price
    private int npcCount = 0;

    private void Start()
    {
        UpdateNPCPriceUI();
    }

    public void SummonNPC()
    {
        if (Inventory.Instance.GetResourceAmount("Coin") >= npcPrice) // Check if player has enough coins
        {
            Inventory.Instance.RemoveResource("Coin", npcPrice); // Deduct coins

            Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
            npcCount++;
            npcPrice *= 2; // Double the price for the next NPC

            UpdateNPCPriceUI(); // Update UI
        }
    }

    private void UpdateNPCPriceUI()
    {
        if (npcPriceText != null)
        {
            npcPriceText.text = "Summon NPC: " + npcPrice + " Coins";
        }
    }
}
