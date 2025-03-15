using UnityEngine;

public class AutoResourceGathering : MonoBehaviour
{
    public float gatherRange = 1f; // Range for detecting trees and rocks

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, gatherRange);
        foreach (var hitCollider in hitColliders)
        {
            // Check for trees
            Tree tree = hitCollider.GetComponent<Tree>();
            if (tree != null)
            {
                tree.StartChopping();
                break; // Only interact with one object per frame
            }

            // Check for rocks
            Stone stone = hitCollider.GetComponent<Stone>();
            if (stone != null)
            {
                stone.StartBreaking();
                break; // Only interact with one object per frame
            }
        }
    }
}
