using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
public class PlayerColor : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Color currentColor;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private float moveInput;
    private float screenLimitX;

    private int matchCount = 0; // Tracks same-color collisions

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // ✅ Assign a random starting color from the easy palette
        currentColor = ColorObject.easyColors[Random.Range(0, ColorObject.easyColors.Length)];
        sr.color = currentColor;

        // Calculate horizontal screen bounds
        float halfPlayerWidth = sr.bounds.extents.x;
        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        screenLimitX = screenHalfWidth - halfPlayerWidth;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Clamp player position within screen
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -screenLimitX, screenLimitX);
        transform.position = pos;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    public void SetPlayerColor(Color newColor)
    {
        currentColor = newColor;
        sr.color = newColor;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("FallingObject")) return;

        ColorObject objColor = collision.gameObject.GetComponent<ColorObject>();
        if (objColor == null) return;

        if (currentColor == objColor.myColor)
        {
            Destroy(collision.gameObject);
            ScoreManager.Instance?.AddScore(1);

            matchCount++;

            if (matchCount >= 5)
            {
                // Change to new random color different from current
                Color newColor;
                do
                {
                    newColor = ColorObject.easyColors[Random.Range(0, ColorObject.easyColors.Length)];
                } while (newColor == currentColor);

                SetPlayerColor(newColor);
                matchCount = 0;
            }
        }
        else
        {
            Debug.Log("Wrong color! Player destroyed.");
            Destroy(gameObject);
        }
    }
}








