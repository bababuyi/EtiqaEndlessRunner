using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    [Header("Manager References")]
    public UIManager uiManager;
    public AudioManager audioManager;

    public static GameManager Instance { get; private set; }

    public event Action<int> OnScoreUpdated;
    public event Action<int> OnCoinsUpdated;
    public event Action<int, int> OnGameOver;
    public event Action OnGamePaused;
    public event Action OnGameResumed;
    public event Action OnGameRestarted;

    public enum GameState { Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Playing;

    [Header("Score")]
    [SerializeField] private float scoreMultiplier = 1f;

    public int CurrentScore { get; private set; }
    public static int FinalScore { get; private set; }

    private Transform playerTransform;
    private Vector3 lastPlayerPosition;

    public int TotalCoins { get; private set; }

    private bool doublePointsActive;
    private float doublePointsEndTime;

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadCoins();
        CachePlayer();
    }

    private void Update()
    {
        if (CurrentState != GameState.Playing) return;

        TrackScore();
        TickDoublePoints();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region Scene

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Main Game") return;

        CurrentState = GameState.Playing;
        CurrentScore = 0;
        doublePointsActive = false;
        CachePlayer();
    }

    private void CachePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
            lastPlayerPosition = playerTransform.position;
        }
        else
        {
            Debug.LogWarning("GameManager: No Player found in scene.");
        }
    }

    #endregion

    #region Score

    private void TrackScore()
    {
        if (TileManager.Instance == null) return;

        float scoreGain = TileManager.Instance.WorldSpeed * scoreMultiplier * Time.deltaTime;
        int updated = CurrentScore + Mathf.RoundToInt(scoreGain);

        if (updated != CurrentScore)
        {
            CurrentScore = updated;
            OnScoreUpdated?.Invoke(CurrentScore);
        }
    }

    private void CommitFinalScore() => FinalScore = CurrentScore;

    #endregion

    #region Coins

    public void AddCoins(int amount)
    {
        int final = doublePointsActive ? amount * 2 : amount;
        TotalCoins += final;
        SaveCoins();
        OnCoinsUpdated?.Invoke(TotalCoins);
    }

    public bool SpendCoins(int amount)
    {
        if (TotalCoins < amount) return false;

        TotalCoins -= amount;
        SaveCoins();
        OnCoinsUpdated?.Invoke(TotalCoins);
        return true;
    }

    private void LoadCoins() =>
        TotalCoins = PlayerPrefs.GetInt("TotalCoins", 0);

    private void SaveCoins()
    {
        PlayerPrefs.SetInt("TotalCoins", TotalCoins);
        PlayerPrefs.Save();
    }

    #endregion

    #region Power-Ups

    public void ActivateDoublePoints(float duration)
    {
        doublePointsActive = true;
        doublePointsEndTime = Time.time + duration;
    }

    public bool IsDoublePointsActive() => doublePointsActive;

    private void TickDoublePoints()
    {
        if (doublePointsActive && Time.time > doublePointsEndTime)
            doublePointsActive = false;
    }

    #endregion

    #region Game State

    public void TriggerGameOver()
    {
        if (CurrentState == GameState.GameOver) return;

        CurrentState = GameState.GameOver;
        CommitFinalScore();
        Time.timeScale = 0f;
        OnGameOver?.Invoke(FinalScore, TotalCoins);
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.Paused;
        Time.timeScale = 0f;
        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused) return;

        CurrentState = GameState.Playing;
        Time.timeScale = 1f;
        OnGameResumed?.Invoke();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        CurrentState = GameState.Playing;
        OnGameRestarted?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    #endregion
}