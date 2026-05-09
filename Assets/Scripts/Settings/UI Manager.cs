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
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI coinsCollectedText;

    [Header("Pause Menu Panel")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Slider volumeSlider;

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        SubscribeToGameManager();
        InitialiseSettingsPanel();
        HideAllPanels();
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
        GameManager.Instance.OnGameRestarted += HideAllPanels;
    }

    private void UnsubscribeFromGameManager()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnScoreUpdated -= HandleScoreUpdated;
        GameManager.Instance.OnCoinsUpdated -= HandleCoinsUpdated;
        GameManager.Instance.OnGameOver -= HandleGameOver;
        GameManager.Instance.OnGamePaused -= HandleGamePaused;
        GameManager.Instance.OnGameResumed -= HandleGameResumed;
        GameManager.Instance.OnGameRestarted -= HideAllPanels;
    }

    #endregion

    #region HUD Handlers

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

    #endregion

    #region Game Over Panel

    private void HandleGameOver(int finalScore, int totalCoins)
    {
        gameOverPanel?.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + finalScore;

        if (coinsCollectedText != null)
            coinsCollectedText.text = "Coins Collected: " + totalCoins;
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
        pauseMenuPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
    }

    public void OnResumeButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        GameManager.Instance?.ResumeGame();
    }

    public void OnOpenSettingsFromPauseButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        pauseMenuPanel?.SetActive(false);
        settingsPanel?.SetActive(true);
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

    public void OnCloseSettingsButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        settingsPanel?.SetActive(false);

        bool returnToPause = SceneManager.GetActiveScene().name == "Main Game"
                          && GameManager.Instance.CurrentState == GameManager.GameState.Paused;

        if (returnToPause)
            pauseMenuPanel?.SetActive(true);
    }

    #endregion

    #region Utility

    private void HideAllPanels()
    {
        gameOverPanel?.SetActive(false);
        pauseMenuPanel?.SetActive(false);
        settingsPanel?.SetActive(false);
    }

    #endregion
}
}