using UnityEngine;

public class AutoChopping : MonoBehaviour
{
    public float chopRange = 1f;

    private void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, chopRange);
        foreach (var hitCollider in hitColliders)
        {
            TreeChopping tree = hitCollider.GetComponent<TreeChopping>();
            if (tree != null)
            {
                tree.StartChopping();
                break; // Only chop one tree at a time
            }
        }
    }
}
