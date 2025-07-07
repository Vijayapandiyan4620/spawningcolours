using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ColorObject : MonoBehaviour
{
    public Color myColor;
    private SpriteRenderer sr;

    // Public static color list accessible by PlayerColor
    public static readonly Color[] easyColors = new Color[]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        new Color(1f, 0.5f, 0f), // orange
        Color.magenta,
        Color.cyan
    };

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        myColor = easyColors[Random.Range(0, easyColors.Length)];
        sr.color = myColor;
    }
}



