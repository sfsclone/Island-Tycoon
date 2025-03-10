using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance; // Singleton reference

    public float moveSpeed;
    private Vector2 move;

    [Header("Falling Mechanic")]
    public float fallHeight = -5f; // The height where the player is considered "fallen"
    public float teleportDelay = 0.5f; // Delay before teleporting

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    private void Start()
    {
        StartCoroutine(CheckFall());
    }

    private void Update()
    {
        MovePlayer();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void MovePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y);
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
        }
    }

    private IEnumerator CheckFall()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f); // Check every 0.2 seconds

            if (transform.position.y < fallHeight)
            {
                yield return new WaitForSeconds(teleportDelay); // Optional delay before teleporting

                Vector3 safePosition = FindNearestIsland();
                transform.position = safePosition;
            }
        }
    }

    private Vector3 FindNearestIsland()
    {
        if (IslandManager.Instance == null) return Vector3.zero;

        Vector3 nearestIsland = Vector3.zero;
        float shortestDistance = Mathf.Infinity;

        foreach (Vector3 islandPos in IslandManager.Instance.GetIslandPositions())
        {
            float distance = Vector3.Distance(transform.position, islandPos);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestIsland = islandPos;
            }
        }

        return nearestIsland + new Vector3(0, 1, 0); // Adjust height to spawn above ground
    }
}
