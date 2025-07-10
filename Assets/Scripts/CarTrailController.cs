using UnityEngine;

public class CarTrailController : MonoBehaviour
{
    public TrailRenderer trail;
    public float minSpeedToShow = 0.5f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trail.emitting = false;
    }

    void Update()
    {
        if (rb.linearVelocity.magnitude > minSpeedToShow)
            trail.emitting = true;
        else
            trail.emitting = false;
    }
}

