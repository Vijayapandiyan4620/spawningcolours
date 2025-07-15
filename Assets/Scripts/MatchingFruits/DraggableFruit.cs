using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableFruit : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemId;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector2 originalPosition;
    public GameObject dragOutsideEffectPrefab;
private GameObject currentEffectInstance;



    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Store original position and parent in case drop is invalid
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;

        // Temporarily disable raycasts so target can receive drop
        canvasGroup.blocksRaycasts = false;

        // Bring to front
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
        // Re-enable raycasts
        canvasGroup.blocksRaycasts = true;

        // Restore parent
        transform.SetParent(originalParent);

        // If not dropped on a valid target, snap back to original position
        if (eventData.pointerEnter == null || eventData.pointerEnter.GetComponent<ChilloutSlot>() == null)
        {
            rectTransform.anchoredPosition = originalPosition;
        }
    }
}


