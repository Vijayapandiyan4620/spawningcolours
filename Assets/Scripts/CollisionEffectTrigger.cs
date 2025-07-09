using UnityEngine;

public class CollisionEffectTrigger : MonoBehaviour
{
    public GameObject particleEffectPrefab; // Drag prefab GameObject here in Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ✅ Instantiate effect
            Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);

            // Optional: destroy self or player
            Destroy(gameObject);
        }
    }
}


