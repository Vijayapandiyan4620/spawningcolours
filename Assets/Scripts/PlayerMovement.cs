using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(AudioSource))]
public class PlayerColor : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Color currentColor;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    private float moveInput;
    private float screenLimitX;
    private int matchCount = 0;

    [Header("Sound Effects")]
    public AudioClip matchSound;
    public AudioClip deathSound;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // Assign a random starting color
        currentColor = ColorObject.easyColors[Random.Range(0, ColorObject.easyColors.Length)];
        sr.color = currentColor;

        // Screen bounds calculation
        float halfPlayerWidth = sr.bounds.extents.x;
        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        screenLimitX = screenHalfWidth - halfPlayerWidth;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Clamp position inside screen
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

            // ✅ Play match sound
            if (matchSound != null && audioSource != null)
                audioSource.PlayOneShot(matchSound);

            matchCount++;

            if (matchCount >= 5)
            {
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
            // ✅ Play death sound at position
            if (deathSound != null)
                AudioSource.PlayClipAtPoint(deathSound, transform.position);

            Debug.Log("Wrong color! Player destroyed.");
            Destroy(gameObject);
            GameManager.Instance?.GameOver();
        }
    }
}








