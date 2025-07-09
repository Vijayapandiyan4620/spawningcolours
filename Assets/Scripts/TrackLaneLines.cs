using UnityEngine;

public class DynamicLaneLines : MonoBehaviour
{
    public float segmentHeight = 0.5f;
    public float segmentSpacing = 0.3f;
    public float startY = 6f;
    public float endY = -6f;
    public float lineWidth = 0.5f;
    public Material lineMaterial;

    void Start()
    {
        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        float screenWidth = screenHalfWidth * 2f;
        float laneWidth = screenWidth / 3f;

        // Dividers between 3 lanes: 2 lines
        float[] dividerX = new float[2]
        {
            -screenHalfWidth + laneWidth * 1f,
            -screenHalfWidth + laneWidth * 2f
        };

        foreach (float x in dividerX)
        {
            float y = startY;
            while (y > endY)
            {
                CreateDashSegment(x, y);
                y -= (segmentHeight + segmentSpacing);
            }
        }
    }

    void CreateDashSegment(float x, float centerY)
    {
        GameObject dash = new GameObject("Dash");
        dash.transform.parent = this.transform;

        LineRenderer lr = dash.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, new Vector3(x, centerY - segmentHeight / 2f, 0));
        lr.SetPosition(1, new Vector3(x, centerY + segmentHeight / 2f, 0));
        lr.startWidth = lr.endWidth = 0.07f;
        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = Color.white;
        lr.sortingOrder = 2;
    }
}





