using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;     // Assign the prefab in Inspector
    public float initialSpawnInterval = 1f;
    public float minSpawnInterval = 0.2f;  // Smallest allowed interval
    public float spawnSpeedupRate = 0.01f; // How much to reduce per spawn

    public float xMin = -8f, xMax = 8f;
    public float ySpawnPosition = 6f;

    private float currentInterval;

    void Start()
    {
        currentInterval = initialSpawnInterval;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnObject();

            // Wait for current interval
            yield return new WaitForSeconds(currentInterval);

            // Reduce interval gradually
            if (currentInterval > minSpawnInterval)
                currentInterval -= spawnSpeedupRate;
        }
    }

    void SpawnObject()
    {
        float randomX = Random.Range(xMin, xMax);
        Vector3 spawnPos = new Vector3(randomX, ySpawnPosition, 0f);

        GameObject spawnedObj = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);

        // Optional: assign color from ColorObject.cs (if you use fixed palette)
        ColorObject colorScript = spawnedObj.GetComponent<ColorObject>();
        if (colorScript == null)
        {
            SpriteRenderer sr = spawnedObj.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = new Color(Random.value, Random.value, Random.value); // fallback
        }
    }
}



