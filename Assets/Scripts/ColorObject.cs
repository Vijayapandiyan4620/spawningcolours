using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ColorObject : MonoBehaviour
{
    public Color myColor;
    public static Color[] easyColors = { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta };

    public void SetColor(Color color)
    {
        myColor = color;
        GetComponent<SpriteRenderer>().color = color;
    }

    public void AssignRandomColor()
    {
        SetColor(easyColors[Random.Range(0, easyColors.Length)]);
    }
}




