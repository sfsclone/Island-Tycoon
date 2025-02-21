using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;



public class InventoryUI : MonoBehaviour
{
    public TMP_Text inventoryText; // jgn lupa assign TMP_Text

    public void UpdateInventory(Dictionary<string, int> inventory)
    {
        inventoryText.text = "Inventory:\n";
        foreach (var item in inventory)
        {
            inventoryText.text += item.Key + " x" + item.Value + "\n";
        }
    }

}
