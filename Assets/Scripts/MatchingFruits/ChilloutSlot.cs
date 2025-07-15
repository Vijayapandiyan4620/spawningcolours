using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChilloutSlot : MonoBehaviour, IDropHandler
{
    public string expectedId;

    public void OnDrop(PointerEventData eventData)
    {
        var item = eventData.pointerDrag.GetComponent<DraggableFruit>();
        if (item != null)
        {
            if (item.itemId == expectedId)
            {
                // ✅ Pass sprite and ID to FruitGameManager
                Sprite matchedSprite = item.GetComponent<Image>().sprite;
                FruitGameManager.Instance.IncreaseScore();
                FruitGameManager.Instance.HandleCorrectMatch(matchedSprite, item.itemId);

                // ✅ Destroy the dragged item
                Destroy(item.gameObject);
            }
            else
            {
                FruitGameManager.Instance.ShowGameOver();
            }
        }
    }
}







