using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableFruit : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Fruit Info")]
    public string itemId;

    [Header("Particle Effect")]
    public GameObject particleEffectPrefab; // Assign your particle prefab in inspector
    private GameObject activeParticle;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        // Automatically spawn particle on fruit creation
        SpawnParticleEffect();
    }

    public void SpawnParticleEffect()
    {
        if (particleEffectPrefab != null && activeParticle == null)
        {
            // Spawn the particle as a child of this fruit
            activeParticle = Instantiate(particleEffectPrefab, transform);

            // Make sure it's drawn behind the fruit image
            activeParticle.transform.SetSiblingIndex(0);

            // Position and scale
            activeParticle.transform.localPosition = Vector3.zero;
            activeParticle.transform.localScale = Vector3.one * 1.5f; // adjust scale as needed
        }
    }

    public void StopParticleEffect()
    {
        if (activeParticle != null)
        {
            Destroy(activeParticle);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        transform.SetParent(originalParent);

        if (eventData.pointerEnter == null || eventData.pointerEnter.GetComponent<ChilloutSlot>() == null)
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}




