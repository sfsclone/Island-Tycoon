using UnityEngine;

public class PlayerIslandBuyer : MonoBehaviour
{
    public int islandCost = 10;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            TryBuyIsland();
        }
    }

    public void TryBuyIsland()
    {
        IslandManager.Instance.BuyNewIsland(transform.position);
    }
}
