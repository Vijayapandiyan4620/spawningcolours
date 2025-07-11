using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(AudioSource))]
public class PlayerColor : MonoBehaviour
{
    public Color currentColor;
    [HideInInspector] public bool isColorSet = false;

    public AudioClip matchSound;
    public AudioClip deathSound;

    public GameObject matchParticlePrefab;
    public GameObject mismatchParticlePrefab;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Camera mainCamera;

    private float screenLimitX;
    private int matchCount = 0;

    private bool isDragging = false;
    private float dragThreshold = 0.1f;
    private float initialTouchX;

    private float smoothSpeed = 10f; // ✅ Controls how fast the player follows the finger
    private float targetX;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;

        AssignInitialColor();

        float halfPlayerWidth = sr.bounds.extents.x;
        float screenHalfWidth = mainCamera.orthographicSize * Screen.width / Screen.height;
        screenLimitX = screenHalfWidth - halfPlayerWidth;

        targetX = transform.position.x;
    }

    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        HandleTouchInput();
#elif UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#endif

        // ✅ Smooth movement toward the targetX
        Vector3 newPos = transform.position;
        newPos.x = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * smoothSpeed);
        transform.position = newPos;
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isDragging = false;
                    initialTouchX = worldPos.x;
                    break;

                case TouchPhase.Moved:
                    if (!isDragging && Mathf.Abs(worldPos.x - initialTouchX) > dragThreshold)
                        isDragging = true;

                    if (isDragging)
                    {
                        targetX = Mathf.Clamp(worldPos.x, -screenLimitX, screenLimitX);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetX = Mathf.Clamp(worldPos.x, -screenLimitX, screenLimitX);
        }
    }

    void AssignInitialColor()
    {
        currentColor = ColorObject.easyColors[Random.Range(0, ColorObject.easyColors.Length)];
        sr.color = currentColor;
        isColorSet = true;
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

            if (matchSound != null)
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












