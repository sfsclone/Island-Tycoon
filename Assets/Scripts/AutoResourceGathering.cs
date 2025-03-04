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
            TreeChopping tree = hitCollider.GetComponent<TreeChopping>();
            if (tree != null)
            {
                tree.StartChopping();
                break; // Only interact with one object per frame
            }

            // Check for rocks
            RockBreaking rock = hitCollider.GetComponent<RockBreaking>();
            if (rock != null)
            {
                rock.StartBreaking();
                break; // Only interact with one object per frame
            }
        }
    }
}
