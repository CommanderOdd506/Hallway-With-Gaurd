using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public string mainMenuScene = "MainMenu";
    public GameObject pausePanel;
    public GameObject deathPanel;
    public GameObject winPanel;

    private bool paused;
    private bool lost = false;
    private bool won = false;

    void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
        ResumeGame();
        lost = false;
        won = false;
    }

    // This replaces OnPause(InputAction.CallbackContext) style
    // Make sure your PlayerInput component has a Pause action and is set to "Send Messages"
    void OnPause(InputValue value)
    {
        if (!value.isPressed) return;

        if (!lost && !won)
        {
            if (IsPaused())
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void LoseGame()
    {
        ResumeGame();
        lost = true;
        paused = true;
        Time.timeScale = 0f;
        if (deathPanel != null)
            deathPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void WinGame()
    {
        ResumeGame();
        won = true;
        paused = true;
        Time.timeScale = 0f;
        if (winPanel != null)
            winPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PauseGame()
    {
        if (!paused && !lost && !won)
        {
            paused = true;
            Time.timeScale = 0f;
            if (pausePanel != null)
                pausePanel.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ResumeGame()
    {
        if (paused && !lost && !won)
        {
            paused = false;
            Time.timeScale = 1f;
            if (pausePanel != null)
                pausePanel.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void Retry()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void NextLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public bool IsPaused()
    {
        return paused;
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Game quit");
    }
}

