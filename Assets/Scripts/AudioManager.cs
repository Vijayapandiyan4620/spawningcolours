using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource mainMenuMusic;
    public AudioSource gameMusic;

    public bool isMusicOn = true;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        UpdateMusicState();
    }

    public void UpdateMusicState()
    {
        if (mainMenuMusic != null) mainMenuMusic.mute = !isMusicOn;
        if (gameMusic != null) gameMusic.mute = !isMusicOn;
    }
}

