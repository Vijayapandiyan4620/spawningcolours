using UnityEngine;
using UnityEngine.UI;

public class MenuAudioControls : MonoBehaviour
{
    public Button musicToggleButton;
    public Sprite musicOnIcon;
    public Sprite musicOffIcon;

    private void Start()
    {
        UpdateMusicIcon();

        musicToggleButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ToggleMusic();
            UpdateMusicIcon();
        });
    }

    void UpdateMusicIcon()
    {
        if (MusicManager.Instance.IsMuted())
            musicToggleButton.image.sprite = musicOffIcon;
        else
            musicToggleButton.image.sprite = musicOnIcon;
    }
}

