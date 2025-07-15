using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FruitGameManager : MonoBehaviour
{
    public static FruitGameManager Instance;


    [Header("Chillout Target")]
    public Image chilloutSlot;
    public Sprite[] chilloutSprites;
    public string[] chilloutIds;

    [Header("UI & Score")]
    public TMP_Text scoreText;

    [Header("Draggable Items")]
    public GameObject draggablePrefab;
    public Transform contentPanel;
    public Sprite[] itemSprites;
    public string[] itemIds;

    [Header("Match Info Panel")]
    public GameObject matchInfoPanel;
    public Image matchedItemImage;
    public TMP_Text matchedItemIdText;
    public float panelShowDelay = 1.5f;
    public float panelVisibleTime = 2f;

    [Header("Celebration Effects")]
    public GameObject matchEffectPrefab;
    public Transform effectSpawnPoint;
    public AudioSource audioSource;
    public AudioClip matchSound;

    // ⭐ NEW: Extra particle effect
    [Header("Extra Celebration Effect")]
    public GameObject extraMatchEffectPrefab;       // Assign in Inspector
    public Transform extraEffectSpawnPoint;        // Assign in Inspector

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public float gameOverVisibleTime = 2f;

    private int currentChilloutIndex = 0;
    private int score = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnDraggableItems();
        LoadChillout(0);

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (matchInfoPanel) matchInfoPanel.SetActive(false);
    }

    void SpawnDraggableItems()
    {
        for (int i = 0; i < itemSprites.Length; i++)
        {
            GameObject obj = Instantiate(draggablePrefab, contentPanel);
            obj.GetComponent<Image>().sprite = itemSprites[i];
            obj.GetComponent<DraggableFruit>().itemId = itemIds[i];

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.sizeDelta = new Vector2(200, 200);
        }
    }

    void LoadChillout(int index)
    {
        chilloutSlot.sprite = chilloutSprites[index];
        chilloutSlot.transform.localScale = Vector3.one;
        chilloutSlot.color = new Color(1f, 1f, 1f, 1f);
        chilloutSlot.gameObject.SetActive(true);
        chilloutSlot.GetComponent<ChilloutSlot>().expectedId = chilloutIds[index];
    }

    public void LoadNextChillout()
    {
        currentChilloutIndex++;
        if (currentChilloutIndex < chilloutSprites.Length)
        {
            LoadChillout(currentChilloutIndex);
        }
        else
        {
            Debug.Log("🎉 All matches complete!");
        }
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("❌ Game Over!");
            StartCoroutine(HideGameOverAfterDelay());
        }
    }

    private IEnumerator HideGameOverAfterDelay()
    {
        yield return new WaitForSeconds(gameOverVisibleTime);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void HandleCorrectMatch(Sprite matchedSprite, string itemId)
    {
        StartCoroutine(HandleCorrectMatchRoutine(matchedSprite, itemId));
    }

    private IEnumerator HandleCorrectMatchRoutine(Sprite matchedSprite, string itemId)
    {
        // ✅ Update chillout slot with matched sprite
        chilloutSlot.sprite = matchedSprite;

        // bounce animation
        LeanTween.scale(chilloutSlot.gameObject, Vector3.one * 1.2f, 0.2f)
            .setEaseOutBack()
            .setOnComplete(() =>
            {
                LeanTween.scale(chilloutSlot.gameObject, Vector3.one, 0.2f).setEaseInBack();
            });

           

        // 🎇 Main effect
        if (matchEffectPrefab && effectSpawnPoint)
            Instantiate(matchEffectPrefab, effectSpawnPoint.position, Quaternion.identity);

        // ⭐ NEW: Extra particle effect
        if (extraMatchEffectPrefab && extraEffectSpawnPoint)
            Instantiate(extraMatchEffectPrefab, extraEffectSpawnPoint.position, Quaternion.identity);

        // 🔊 sound
        if (audioSource && matchSound)
            audioSource.PlayOneShot(matchSound);

        // wait before showing panel
        yield return new WaitForSeconds(panelShowDelay);

        // ✅ show match panel
        matchedItemImage.sprite = matchedSprite;
        matchedItemIdText.text = itemId;
        matchInfoPanel.SetActive(true);

       LeanTween.scale(matchedItemImage.gameObject, Vector3.one * 1.2f, 2f)
    .setEaseOutBack();

matchInfoPanel.transform.localRotation = Quaternion.identity;
LeanTween.rotateZ(matchInfoPanel, 360f, 2f)
    .setEase(LeanTweenType.linear)
    .setLoopClamp();

        // keep panel on screen
        yield return new WaitForSeconds(panelVisibleTime);

LeanTween.cancel(matchInfoPanel);
matchInfoPanel.transform.localRotation = Quaternion.identity;
matchInfoPanel.SetActive(false);
    

        // ✅ hide panel
        matchInfoPanel.SetActive(false);

        // ✅ load next chillout sprite
        LoadNextChillout();

        // ✅ now ease out → ease in
        EaseOutChillout();
    }

    public void EaseOutChillout()
    {
        if (chilloutSlot != null)
        {
            // fade out & hide
            // LeanTween.scale(chilloutSlot.gameObject, Vector3.zero, 0.4f)
            //     .setEaseInBack()
            //     .setOnComplete(() =>
            //     {
            //         chilloutSlot.gameObject.SetActive(false);
                   
            //     });
                 EaseInChillout();
        }
    }

    private void EaseInChillout()
    {
        if (chilloutSlot != null)
        {
            chilloutSlot.transform.localScale = Vector3.zero;
            chilloutSlot.gameObject.SetActive(true);

            LeanTween.scale(chilloutSlot.gameObject, Vector3.one, 0.4f)
                .setEaseOutBack();
        }
    }
}











