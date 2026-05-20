using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

[DefaultExecutionOrder(-50)]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalDistanceText;
    [SerializeField] private TextMeshProUGUI coinsCollectedText;

    [Header("Pause Menu Panel")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Slider volumeSlider;

    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject newBestLabel;

    #region Unity Lifecycle

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SubscribeToGameManager();
        InitialiseSettingsPanel();
        HideAllPanels();

        if (SceneManager.GetActiveScene().name == "Main Game")
            ResetHUD();

        int best = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
            highScoreText.text = "Best: " + best;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            HandleEscapeKey();
    }

    private void OnDestroy() => UnsubscribeFromGameManager();

    #endregion

    #region GameManager Event Wiring

    private void SubscribeToGameManager()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnScoreUpdated += HandleScoreUpdated;
        GameManager.Instance.OnCoinsUpdated += HandleCoinsUpdated;
        GameManager.Instance.OnGameOver += HandleGameOver;
        GameManager.Instance.OnGamePaused += HandleGamePaused;
        GameManager.Instance.OnGameResumed += HandleGameResumed;
        GameManager.Instance.OnGameRestarted += HandleGameRestarted;
        GameManager.Instance.OnDistanceUpdated += HandleDistanceUpdated;
        GameManager.Instance.OnHealthChanged += HandleHealthChanged;
    }

    private void UnsubscribeFromGameManager()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnScoreUpdated -= HandleScoreUpdated;
        GameManager.Instance.OnCoinsUpdated -= HandleCoinsUpdated;
        GameManager.Instance.OnGameOver -= HandleGameOver;
        GameManager.Instance.OnGamePaused -= HandleGamePaused;
        GameManager.Instance.OnGameResumed -= HandleGameResumed;
        GameManager.Instance.OnGameRestarted -= HandleGameRestarted;
        GameManager.Instance.OnDistanceUpdated -= HandleDistanceUpdated;
        GameManager.Instance.OnHealthChanged -= HandleHealthChanged;
    }

    #endregion

    #region HUD

    private void HandleScoreUpdated(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void HandleCoinsUpdated(int coins)
    {
        if (coinText != null)
            coinText.text = "Coins: " + coins;
    }

    private void HandleDistanceUpdated(float metres)
    {
        if (distanceText != null)
            distanceText.text = Mathf.FloorToInt(metres) + "m";
    }

    private void HandleHealthChanged(int current, int max)
    {
        if (hpText != null)
            hpText.text = "HP: " + new string('♥', current).PadRight(max, '♡');
    }

    private void ResetHUD()
    {
        if (hudPanel != null) hudPanel.SetActive(true);

        if (scoreText != null) scoreText.text = "Score: 0";
        if (coinText != null) coinText.text = "Coins: 0";
        if (distanceText != null) distanceText.text = "0m";
        if (hpText != null) hpText.text = "HP: --";
    }

    #endregion

    #region Game Over Panel

    private void HandleGameOver(int finalScore, int totalCoins)
    {
        bool isNewBest = finalScore >= GameManager.Instance.HighScore;
        if (newBestLabel != null) newBestLabel.SetActive(isNewBest);

        hudPanel?.SetActive(false);
        gameOverPanel?.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + finalScore;

        if (finalDistanceText != null)
            finalDistanceText.text = "Distance: " + Mathf.FloorToInt(GameManager.Instance.DistanceMetres) + "m";

        if (coinsCollectedText != null)
            coinsCollectedText.text = "Coins Collected: " + totalCoins;

        if (highScoreText != null)
            highScoreText.text = "Best: " + GameManager.Instance.HighScore;
    }

    public void OnRestartButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        GameManager.Instance?.RestartGame();
    }

    public void OnMainMenuFromGameOverButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        GameManager.Instance?.GoToMainMenu();
    }

    #endregion

    #region Pause Menu

    private void HandleEscapeKey()
    {
        if (SceneManager.GetActiveScene().name != "Main Game") return;
        if (GameManager.Instance.CurrentState == GameManager.GameState.GameOver) return;

        TogglePause();
    }

    private void TogglePause()
    {
        AudioManager.Instance?.PlayButtonClick();

        if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
            GameManager.Instance.PauseGame();
        else if (GameManager.Instance.CurrentState == GameManager.GameState.Paused)
            GameManager.Instance.ResumeGame();
    }

    private void HandleGamePaused()
    {
        pauseMenuPanel?.SetActive(true);
        settingsPanel?.SetActive(false);
    }

    private void HandleGameResumed()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        settingsPanel?.SetActive(false);
    }

    public void OnResumeButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        GameManager.Instance?.ResumeGame();
    }

    public void OnOpenSettingsButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false); // hide main menu
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void OnCloseSettingsButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        if (settingsPanel != null) settingsPanel.SetActive(false);

        bool returnToPause = SceneManager.GetActiveScene().name == "Main Game"
                          && GameManager.Instance != null
                          && GameManager.Instance.CurrentState == GameManager.GameState.Paused;

        if (returnToPause)
        {
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        }
        else
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true); // restore main menu
        }
    }

    public void OnMainMenuFromPauseButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        GameManager.Instance?.GoToMainMenu();
    }

    #endregion

    #region Settings Panel

    private void InitialiseSettingsPanel()
    {
        if (soundToggle == null || volumeSlider == null) return;

        bool savedSound = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);

        soundToggle.SetIsOnWithoutNotify(savedSound);
        volumeSlider.SetValueWithoutNotify(savedVolume);

        soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);

        EventTrigger trigger = volumeSlider.gameObject.GetComponent<EventTrigger>()
                            ?? volumeSlider.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerUp = new EventTrigger.Entry
        { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener(_ => AudioManager.Instance?.PlayButtonClick());
        trigger.triggers.Add(pointerUp);

        AudioManager.Instance?.SetMuted(!savedSound);
        AudioManager.Instance?.SetVolume(savedVolume);
    }

    private void OnSoundToggleChanged(bool isOn)
    {
        AudioManager.Instance?.SetMuted(!isOn);
        PlayerPrefs.SetInt("SoundEnabled", isOn ? 1 : 0);
        PlayerPrefs.Save();
        if (isOn) AudioManager.Instance?.PlayButtonClick();
    }

    private void OnVolumeSliderChanged(float value)
    {
        AudioManager.Instance?.SetVolume(value);
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
    }

    #endregion

    #region Utility

    private void HandleGameRestarted()
    {
        HideAllPanels();
        ResetHUD();
    }

    private void HideAllPanels()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    #endregion
}