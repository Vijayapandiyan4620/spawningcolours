// using UnityEngine;
// using System.Collections;

// public class ObjectSpawner : MonoBehaviour
// {
//     public GameObject objectToSpawn;
//     public float initialSpawnInterval = 1f;
//     public float minSpawnInterval = 0.2f;
//     public float spawnSpeedupRate = 0.05f;
//     public float ySpawnPosition = 6f;

//     public float edgePadding = 0.5f;

//     private float currentInterval;
//     private float xMin;
//     private float xMax;

//     private int lastSpeedupThreshold = 0;

//     void Start()
//     {
//         float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
//         xMin = -screenHalfWidth + edgePadding;
//         xMax = screenHalfWidth - edgePadding;

//         currentInterval = initialSpawnInterval;
//         StartCoroutine(SpawnRoutine());
//     }

//     IEnumerator SpawnRoutine()
//     {
//         while (true)
//         {
//             SpawnObject();
//             yield return new WaitForSeconds(currentInterval);
//             UpdateSpawnSpeedByScore();
//         }
//     }

//     void UpdateSpawnSpeedByScore()
//     {
//         int score = ScoreManager.Instance.score;

//         if (score >= lastSpeedupThreshold + 7)
//         {
//             lastSpeedupThreshold += 7;

//             if (currentInterval > minSpawnInterval)
//             {
//                 currentInterval -= spawnSpeedupRate;
//                 currentInterval = Mathf.Max(currentInterval, minSpawnInterval);
//             }
//         }
//     }

//     void SpawnObject()
//     {
//         float randomX = Random.Range(xMin, xMax);
//         Vector3 spawnPos = new Vector3(randomX, ySpawnPosition, 0f);
//         GameObject spawnedObj = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);

//         ColorObject colorScript = spawnedObj.GetComponent<ColorObject>();
//         if (colorScript == null)
//         {
//             SpriteRenderer sr = spawnedObj.GetComponent<SpriteRenderer>();
//             if (sr != null)
//                 sr.color = new Color(Random.value, Random.value, Random.value);
//         }
//     }
// }


using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    public static float ScrollSpeed = 4f; // 📌 Shared scroll speed for lane lines + objects

    public GameObject objectToSpawn;
    public float spawnInterval = 1f;
    public float ySpawnPosition = 6f;
    public float spawnSpeedIncrement = 0.1f;
    public float startDelay = 1.5f;

    private float[] laneXPositions;
    private int nextScoreThreshold = 7;
    private int spawnedCount = 0;

    void Start()
    {
        SetupLanePositions();
        StartCoroutine(WaitForPlayerColorAndStartSpawning());
    }

    void SetupLanePositions()
    {
        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        float screenWidth = screenHalfWidth * 2f;
        float laneWidth = screenWidth / 3f;

        laneXPositions = new float[3];
        for (int i = 0; i < 3; i++)
        {
            laneXPositions[i] = -screenHalfWidth + laneWidth * (i + 0.5f);
        }
    }

    IEnumerator WaitForPlayerColorAndStartSpawning()
    {
        GameObject player = null;
        PlayerColor playerColor = null;

        while (playerColor == null || !playerColor.isColorSet)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                playerColor = player.GetComponent<PlayerColor>();

            yield return null;
        }

        yield return new WaitForSeconds(startDelay);
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
                yield break;

            SpawnObjects();

            yield return new WaitForSeconds(spawnInterval);

            if (ScoreManager.Instance != null && ScoreManager.Instance.score >= nextScoreThreshold)
            {
                spawnInterval = Mathf.Max(0.2f, spawnInterval - spawnSpeedIncrement);
                ScrollSpeed += 0.5f; // 🔼 Increase speed for both objects + lane lines
                ScrollSpeed = Mathf.Min(12f, ScrollSpeed); // Clamp max
                nextScoreThreshold += 7;
            }
        }
    }

    void SpawnObjects()
    {
        if (spawnedCount == 0)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerColor playerColor = player?.GetComponent<PlayerColor>();
            Color targetColor = playerColor != null ? playerColor.currentColor : Color.white;

            int playerColorLaneIndex = Random.Range(0, laneXPositions.Length);

            for (int i = 0; i < laneXPositions.Length; i++)
            {
                float x = laneXPositions[i];
                GameObject obj = Instantiate(objectToSpawn, new Vector3(x, ySpawnPosition, 0), Quaternion.identity);
                ColorObject objColor = obj.GetComponent<ColorObject>();

                if (objColor != null)
                {
                    if (i == playerColorLaneIndex)
                        objColor.SetColor(targetColor);
                    else
                    {
                        Color newColor;
                        do
                        {
                            newColor = ColorObject.easyColors[Random.Range(0, ColorObject.easyColors.Length)];
                        }
                        while (newColor == targetColor);

                        objColor.SetColor(newColor);
                    }
                }
            }

            spawnedCount++;
        }
        else
        {
            int laneIndex = Random.Range(0, 3);
            float x = laneXPositions[laneIndex];
            GameObject obj = Instantiate(objectToSpawn, new Vector3(x, ySpawnPosition, 0), Quaternion.identity);

            ColorObject objColor = obj.GetComponent<ColorObject>();
            if (objColor != null)
                objColor.AssignRandomColor();

            spawnedCount++;
        }
    }
}















