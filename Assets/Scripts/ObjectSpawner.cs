using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float initialSpawnInterval = 1f;
    public float minSpawnInterval = 0.2f;
    public float spawnSpeedupRate = 0.05f;
    public float ySpawnPosition = 6f;

    public float edgePadding = 0.5f;

    private float currentInterval;
    private float xMin;
    private float xMax;

    private int lastSpeedupThreshold = 0;

    void Start()
    {
        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        xMin = -screenHalfWidth + edgePadding;
        xMax = screenHalfWidth - edgePadding;

        currentInterval = initialSpawnInterval;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnObject();
            yield return new WaitForSeconds(currentInterval);
            UpdateSpawnSpeedByScore();
        }
    }

    void UpdateSpawnSpeedByScore()
    {
        int score = ScoreManager.Instance.score;

        if (score >= lastSpeedupThreshold + 7)
        {
            lastSpeedupThreshold += 7;

            if (currentInterval > minSpawnInterval)
            {
                currentInterval -= spawnSpeedupRate;
                currentInterval = Mathf.Max(currentInterval, minSpawnInterval);
            }
        }
    }

    void SpawnObject()
    {
        float randomX = Random.Range(xMin, xMax);
        Vector3 spawnPos = new Vector3(randomX, ySpawnPosition, 0f);
        GameObject spawnedObj = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);

        ColorObject colorScript = spawnedObj.GetComponent<ColorObject>();
        if (colorScript == null)
        {
            SpriteRenderer sr = spawnedObj.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = new Color(Random.value, Random.value, Random.value);
        }
    }
}








