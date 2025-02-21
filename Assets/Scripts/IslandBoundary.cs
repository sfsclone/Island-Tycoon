// IslandBoundary.cs
using UnityEngine;

public class IslandBoundary : MonoBehaviour
{
    public float boundaryThickness = 1f;
    public Vector2 islandSize = new Vector2(40, 20);
    public float boundaryHeight = 5f;

    private void Start()
    {
        GenerateBoundaries();
    }

    private void GenerateBoundaries()
    {
        float halfWidth = islandSize.x / 2;
        float halfHeight = islandSize.y / 2;

        CreateBoundary(new Vector3(0, boundaryHeight / 2, halfHeight), new Vector3(islandSize.x, boundaryHeight, boundaryThickness)); // Top
        CreateBoundary(new Vector3(0, boundaryHeight / 2, -halfHeight), new Vector3(islandSize.x, boundaryHeight, boundaryThickness)); // Bottom
        CreateBoundary(new Vector3(halfWidth, boundaryHeight / 2, 0), new Vector3(boundaryThickness, boundaryHeight, islandSize.y)); // Right
        CreateBoundary(new Vector3(-halfWidth, boundaryHeight / 2, 0), new Vector3(boundaryThickness, boundaryHeight, islandSize.y)); // Left
    }

    private void CreateBoundary(Vector3 position, Vector3 size)
    {
        GameObject boundary = new GameObject("Boundary");
        boundary.transform.parent = transform;
        boundary.transform.position = transform.position + position; // Adjusted position

        // Physical Collider to block player
        BoxCollider physicalCollider = boundary.AddComponent<BoxCollider>();
        physicalCollider.size = size;
        physicalCollider.isTrigger = false; // Blocks player
    }
}
