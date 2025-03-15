using UnityEngine;
using System.Collections;
using System.Linq;

public class NPCMiner : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f;
    public float detectionRadius = 10f;
    private Transform targetResource;
    private bool isMining = false;
    private bool isMoving = false;

    private void Update()
    {
        if (!isMining)
        {
            FindNewTarget();
        }
        else if (targetResource == null || !IsTargetActive(targetResource))
        {
            StopMining();
            FindNewTarget();
        }
    }

    private void FixedUpdate()
    {
        if (isMoving && targetResource != null && IsTargetActive(targetResource))
        {
            MoveToTarget();
        }
    }

    private void FindNewTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
        var resourceTargets = colliders
            .Select(col => col.transform)
            .Where(t => (t.CompareTag("Tree") || t.CompareTag("Stone")) && IsTargetActive(t))
            .OrderBy(t => Vector3.Distance(transform.position, t.position))
            .ToList();

        if (resourceTargets.Count > 0)
        {
            targetResource = resourceTargets[0];
            isMoving = true; // Start moving to the target
        }
    }

    private bool IsTargetActive(Transform resource)
    {
        if (resource.CompareTag("Tree"))
        {
            Tree tree = resource.GetComponent<Tree>();
            return tree != null && tree.IsTreeActive();
        }
        else if (resource.CompareTag("Stone"))
        {
            Stone stone = resource.GetComponent<Stone>();
            return stone != null && stone.IsStoneActive();
        }
        return false;
    }

    private void MoveToTarget()
    {
        if (targetResource == null) return;

        // Rotate toward target
        Vector3 direction = (targetResource.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Move toward target
        transform.position = Vector3.MoveTowards(transform.position, targetResource.position, moveSpeed * Time.fixedDeltaTime);
    }

    private void StopMining()
    {
        isMining = false;
        isMoving = false;
        targetResource = null;
    }

    // **Trigger-based mining**
    private void OnTriggerEnter(Collider other)
    {
        if (targetResource != null && other.transform == targetResource)
        {
            isMining = true;
            isMoving = false; // Stop moving since we reached the target

            if (targetResource.CompareTag("Tree"))
            {
                targetResource.GetComponent<Tree>()?.StartChopping();
            }
            else if (targetResource.CompareTag("Stone"))
            {
                targetResource.GetComponent<Stone>()?.StartBreaking();
            }

            Invoke(nameof(FindNewTarget), 0.5f); // Short delay before finding a new target
        }
    }
}
