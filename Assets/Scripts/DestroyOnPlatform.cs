using UnityEngine;

public class DestroyOnPlatform : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("FallingObject"))
        {
            Destroy(collision.gameObject);
        }
    }

    // Use this if "Is Trigger" is enabled
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FallingObject"))
        {
            Destroy(other.gameObject);
        }
    }
}

