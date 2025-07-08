using UnityEngine;
using UnityEngine.UI;

public class SoundToggleButton : MonoBehaviour
{
    public Sprite soundOnIcon;
    public Sprite soundOffIcon;
    public Image buttonImage;

    void Start()
    {
        UpdateIcon();
    }

    public void ToggleSound()
    {
        AudioManager.Instance.ToggleMusic();
        UpdateIcon();
    }

    void UpdateIcon()
    {
        buttonImage.sprite = AudioManager.Instance.isMusicOn ? soundOnIcon : soundOffIcon;
    }
}

