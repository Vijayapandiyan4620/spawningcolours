using System.Collections;
using System.Collections.Generic;
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

    [Header("Match Info Particle")]
public GameObject matchInfoParticlePrefab; // Assign in Inspector
private GameObject activeMatchParticle;    // Runtime instance


    [Header("Celebration Effects")]
    public GameObject matchEffectPrefab;
    public Transform effectSpawnPoint;
    public AudioSource audioSource;
    public AudioClip matchSound;

    [Header("Extra Celebration Effect")]
    public GameObject extraMatchEffectPrefab;
    public Transform extraEffectSpawnPoint;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public float gameOverVisibleTime = 2f;

   [Header("Matched Colors")]
public Color matchedBackgroundColor = new Color(0.3255f, 0.3608f, 0.0941f, 1f); // #535C18
public Color normalBackgroundColor = Color.white;

    private int currentChilloutIndex = 0;
    private int score = 0;

    // Track matched IDs
    private HashSet<string> matchedIds = new HashSet<string>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Optional shuffle
        ShuffleSpritesAndIds(ref itemSprites, ref itemIds);
        ShuffleSpritesAndIds(ref chilloutSprites, ref chilloutIds);

        SpawnDraggableItems();
        LoadChillout(0);

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (matchInfoPanel) matchInfoPanel.SetActive(false);
    }

    // 🔀 Shuffle helper
    void ShuffleSpritesAndIds(ref Sprite[] sprites, ref string[] ids)
    {
        for (int i = sprites.Length - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (sprites[i], sprites[rand]) = (sprites[rand], sprites[i]);
            (ids[i], ids[rand]) = (ids[rand], ids[i]);
        }
    }

    void SpawnDraggableItems()
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < itemSprites.Length; i++)
        {
            GameObject obj = Instantiate(draggablePrefab, contentPanel);
            Image img = obj.GetComponent<Image>();
            img.sprite = itemSprites[i];

            DraggableFruit drag = obj.GetComponent<DraggableFruit>();
            drag.itemId = itemIds[i];

            // OPTIONAL: if you want each fruit to have a particle prefab
              drag.SpawnParticleEffect();

            // set color if already matched
            if (matchedIds.Contains(itemIds[i]))
                img.color = matchedBackgroundColor;
            else
                img.color = normalBackgroundColor;

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

    // ✅ Called when a match is found
    public void HandleCorrectMatch(Sprite matchedSprite, string itemId)
    {
        StartCoroutine(HandleCorrectMatchRoutine(matchedSprite, itemId));
    }

    private IEnumerator HandleCorrectMatchRoutine(Sprite matchedSprite, string itemId)
    {
        // Update chillout slot with matched sprite
        chilloutSlot.sprite = matchedSprite;

        // bounce animation
        LeanTween.scale(chilloutSlot.gameObject, Vector3.one * 1.2f, 0.2f)
            .setEaseOutBack()
            .setOnComplete(() =>
            {
                LeanTween.scale(chilloutSlot.gameObject, Vector3.one, 0.2f).setEaseInBack();
            });

        // effects
        if (matchEffectPrefab && effectSpawnPoint)
            Instantiate(matchEffectPrefab, effectSpawnPoint.position, Quaternion.identity);

        if (extraMatchEffectPrefab && extraEffectSpawnPoint)
            Instantiate(extraMatchEffectPrefab, extraEffectSpawnPoint.position, Quaternion.identity);

        if (audioSource && matchSound)
            audioSource.PlayOneShot(matchSound);

        // delay
        yield return new WaitForSeconds(panelShowDelay);

        // show panel
        matchedItemImage.sprite = matchedSprite;
        matchedItemIdText.text = itemId;
        matchInfoPanel.SetActive(true);

      // 🔥 spawn particle behind fruit image
if (matchInfoParticlePrefab != null)
{
    // Remove old particle if exists
    if (activeMatchParticle != null)
        Destroy(activeMatchParticle);

    // Instantiate as child of MatchInfoPanel
    activeMatchParticle = Instantiate(matchInfoParticlePrefab, matchedItemImage.transform.parent);

    // Position it at the same place as the image
    activeMatchParticle.transform.localPosition = matchedItemImage.rectTransform.localPosition;
    activeMatchParticle.transform.localScale = Vector3.one * 10;

    // Set sibling index to behind image
    activeMatchParticle.transform.SetSiblingIndex(0);
}


       // IMAGE scale animation
matchedItemImage.transform.localScale = Vector3.zero;
LeanTween.scale(matchedItemImage.gameObject, Vector3.one * 1.2f, 0.6f).setEaseOutBack();

// TEXT position + scale animation
Vector3 originalPos = matchedItemIdText.rectTransform.localPosition;
matchedItemIdText.rectTransform.localPosition = new Vector3(originalPos.x - 500f, originalPos.y, originalPos.z);

LeanTween.moveLocalX(matchedItemIdText.gameObject, originalPos.x, 0.6f).setEaseOutBack();
matchedItemIdText.transform.localScale = Vector3.zero;
LeanTween.scale(matchedItemIdText.gameObject, Vector3.one, 0.6f).setEaseOutBack();


        matchInfoPanel.transform.localRotation = Quaternion.identity;
        LeanTween.rotateZ(matchInfoPanel, 360f, 2f)
            .setEase(LeanTweenType.linear)
            .setLoopClamp();

        yield return new WaitForSeconds(panelVisibleTime);

        LeanTween.cancel(matchInfoPanel);
        matchInfoPanel.transform.localRotation = Quaternion.identity;
        matchInfoPanel.SetActive(false);

        // mark as matched (no destroy)
        MarkAsMatched(itemId);

        LoadNextChillout();
        EaseOutChillout();
    }

    // ✅ called instead of destroying item
    public void MarkAsMatched(string itemId)
    {
        if (!matchedIds.Contains(itemId))
        {
            matchedIds.Add(itemId);
        }
        UpdateMatchedBackgrounds();
    }

   private void UpdateMatchedBackgrounds()
{
    foreach (Transform child in contentPanel)
    {
        DraggableFruit drag = child.GetComponent<DraggableFruit>();
        if (drag != null)
        {
            Image img = child.GetComponent<Image>(); // main Image component on the draggable prefab
            if (img != null)
            {
                if (matchedIds.Contains(drag.itemId))
                {
                    // 🌟 Directly tint the image itself
                    img.color = new Color(0.3255f, 0.3608f, 0.0941f, 1f); // #535C18
                }
                else
                {
                    // Reset to normal
                    img.color = Color.white;
                }
            }
        }
    }
}


    public void EaseOutChillout()
    {
        if (chilloutSlot != null)
        {
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















