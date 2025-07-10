using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameManager : MonoBehaviour
{
        private IEnumerator ShowGameOverPanelWithDelay(float delay)
{
    yield return new WaitForSeconds(delay);

    if (gameOverPanel != null)
        gameOverPanel.SetActive(true);
}

    public static GameManager Instance;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    [Header("Pause UI")]
    public GameObject pausePanel;

    private bool isPaused = false;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public bool isGameOver { get; private set; } = false;

  public void GameOver()
{
    isGameOver = true;
    StartCoroutine(ShowGameOverPanelWithDelay(1.5f)); // ⏳ Delay in seconds
}


    public void PauseGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene"); // Your game scene name
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}



