using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(AudioSource))]
public class PlayerColor : MonoBehaviour
{
    public float moveSpeed = 10f;
    public Color currentColor;

    [HideInInspector] public bool isColorSet = false;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    private float screenLimitX;
    private int matchCount = 0;

    private Vector3 dragTargetPos;
    private bool isDragging = false;

    [Header("Sound Effects")]
    public AudioClip matchSound;
    public AudioClip deathSound;

    [Header("Particle Effects (Prefabs)")]
    public GameObject matchParticlePrefab;
    public GameObject mismatchParticlePrefab;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        AssignInitialColor();

        float halfPlayerWidth = sr.bounds.extents.x;
        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        screenLimitX = screenHalfWidth - halfPlayerWidth;
    }

    void AssignInitialColor()
    {
        currentColor = ColorObject.easyColors[Random.Range(0, ColorObject.easyColors.Length)];
        sr.color = currentColor;
        isColorSet = true;
    }

    void Update()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        HandleMouseDrag();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchDrag();
#endif

        Vector3 clamped = transform.position;
        clamped.x = Mathf.Clamp(clamped.x, -screenLimitX, screenLimitX);
        transform.position = clamped;
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            Vector2 direction = (dragTargetPos - transform.position);
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void HandleTouchDrag()
    {
        isDragging = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchWorld = Camera.main.ScreenToWorldPoint(touch.position);
            dragTargetPos = new Vector3(touchWorld.x, transform.position.y, 0);
            isDragging = true;
        }
    }

    void HandleMouseDrag()
    {
        isDragging = false;

        if (Input.GetMouseButton(0))
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dragTargetPos = new Vector3(mouseWorld.x, transform.position.y, 0);
            isDragging = true;
        }
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
            // ✅ Spawn and color match particle
            if (matchParticlePrefab != null)
            {
                GameObject effect = Instantiate(matchParticlePrefab, transform.position, Quaternion.identity);
                ParticleSystem ps = effect.GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    var main = ps.main;
                    main.startColor = new ParticleSystem.MinMaxGradient(currentColor);
                }
            }

            Destroy(collision.gameObject);
            ScoreManager.Instance?.AddScore(1);

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
            // ❌ Spawn and color mismatch particle
            if (mismatchParticlePrefab != null)
            {
                GameObject effect = Instantiate(mismatchParticlePrefab, transform.position, Quaternion.identity);
                ParticleSystem ps = effect.GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    var main = ps.main;
                    main.startColor = new ParticleSystem.MinMaxGradient(Color.red);
                }
            }

            if (deathSound != null)
                AudioSource.PlayClipAtPoint(deathSound, transform.position);

            Debug.Log("Wrong color! Player destroyed.");
            Destroy(gameObject);
            GameManager.Instance?.GameOver();
        }
    }
}















