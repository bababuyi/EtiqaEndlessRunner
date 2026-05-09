using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign your Pause Menu UI in Inspector
    public SettingsMenu settingsMenu; // Reference to your SettingsMenu script
    public AudioSource uiAudioSource; // AudioSource for UI sounds
    public AudioClip buttonClickSound; // Sound effect for button clicks

    private bool isPaused = false;

    void Start()
    {
        // Ensure that when entering the main game, the time scale is normal
        if (SceneManager.GetActiveScene().name == "Main Game")
        {
            Time.timeScale = 1f;
        }
    }

    void Update()
    {
        // Check if the game is in the "Main Game" scene and if the Escape key is pressed
        if (SceneManager.GetActiveScene().name == "Main Game" && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        PlaySound(); // Play button sound
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        PlaySound();
        TogglePause();
        pauseMenuUI.SetActive(false);
    }

    public void OpenSettings()
    {
        PlaySound();
        settingsMenu.OpenSettings();
        pauseMenuUI.SetActive(false);
    }

    public void CloseSettings()
    {
        PlaySound();
        settingsMenu.CloseSettings();
        pauseMenuUI.SetActive(true);
    }

    public void GoToMainMenu()
    {
        PlaySound();
        Time.timeScale = 1f; // Ensure time scale is normal when transitioning to main menu

        // Stop tile spawning before loading the new scene
        //FindObjectOfType<TileManager>()?.StopSpawning();

        pauseMenuUI.SetActive(false);
        SceneManager.LoadScene("Main Menu");
    }

    void PlaySound()
    {
        if (uiAudioSource != null && buttonClickSound != null)
        {
            uiAudioSource.PlayOneShot(buttonClickSound);
        }
    }

    // When we return to the Main Game scene, reset the paused state
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Reset when a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Game")
        {
            isPaused = false; // Reset paused state
            pauseMenuUI.SetActive(false); // Ensure the pause menu is hidden
            Time.timeScale = 1f; // Ensure time scale is normal when starting the game
        }
    }
}