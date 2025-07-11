using UnityEngine;
using System.Collections.Generic;

public class LaneLineScroller : MonoBehaviour
{
    public GameObject linePrefab;
    public int linesPerLane = 10;
    public float lineSpacing = 2f;

    private List<Transform> lines = new List<Transform>();
    private Camera cam;
    private float screenTop;
    private float screenBottom;
    private float[] lanePositions;

    void Start()
    {
        cam = Camera.main;
        screenTop = cam.orthographicSize;
        screenBottom = -screenTop;

        float screenWidth = screenTop * 2f * cam.aspect;
        float laneWidth = screenWidth / 3f;

        lanePositions = new float[2];
        lanePositions[0] = -screenWidth / 2f + laneWidth;
        lanePositions[1] = -screenWidth / 2f + laneWidth * 2;

        foreach (float x in lanePositions)
        {
            for (int i = 0; i < linesPerLane; i++)
            {
                Vector3 pos = new Vector3(x, screenBottom + i * lineSpacing, 0);
                GameObject line = Instantiate(linePrefab, pos, Quaternion.identity, transform);
                lines.Add(line.transform);
            }
        }
    }

    void Update()
    {
        float scrollSpeed = ObjectSpawner.ScrollSpeed;

        Debug.Log("=======>"+scrollSpeed);

        foreach (Transform line in lines)
        {
            line.position += Vector3.down * scrollSpeed * Time.deltaTime;

            if (line.position.y < screenBottom - 1f)
            {
                line.position += new Vector3(0, lineSpacing * linesPerLane, 0);
            }
        }
    }
}







