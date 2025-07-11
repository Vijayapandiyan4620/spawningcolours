using UnityEngine;

public class DestroyOnPlatform : MonoBehaviour
{
    private int matchOnPlatformCount = 0;
    private Color lastCheckedPlayerColor;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("FallingObject"))
        {
            HandleObjectCollision(collision.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FallingObject"))
        {
            HandleObjectCollision(other.gameObject);
        }
    }

    void HandleObjectCollision(GameObject obj)
    {
        // Destroy object immediately if player is null
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Destroy(obj);
            Debug.Log("Player is null → Destroying object.");
            return;
        }

        PlayerColor playerColorScript = player.GetComponent<PlayerColor>();
        if (playerColorScript == null)
        {
            Destroy(obj);
            return;
        }

        Color currentPlayerColor = playerColorScript.currentColor;

        ColorObject colorObj = obj.GetComponent<ColorObject>();
        if (colorObj == null)
        {
            Destroy(obj);
            return;
        }

        // ✅ Reset count if player's color changed
        if (lastCheckedPlayerColor != currentPlayerColor)
        {
            matchOnPlatformCount = 0;
            lastCheckedPlayerColor = currentPlayerColor;
            Debug.Log("Player color changed → Match count reset.");
        }

        // ✅ Compare color
        if (colorObj.myColor == currentPlayerColor)
        {
            matchOnPlatformCount++;
            Debug.Log("Matched color on platform: " + matchOnPlatformCount);

            if (matchOnPlatformCount >= 3)
            {
                Debug.Log("❌ Player destroyed after 3 same-color matches on platform.");
                Destroy(player);

                GameManager.Instance?.GameOver(); // Show Game Over panel
            }
        }

        // Always destroy the falling object
        Destroy(obj);
    }
}




