using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Import TextMeshPro namespace

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public TileManager tileManager;
    private TextMeshProUGUI finalScoreText; // Automatically assigned at runtime
    private TextMeshProUGUI coinsCollectedText; // For total coins collected

    private static GameOverManager instance;
    private GameObject currentPlayer;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        gameOverUI.SetActive(false);

        if (tileManager == null)
        {
            tileManager = FindObjectOfType<TileManager>();
        }

        if (tileManager == null)
        {
            Debug.LogError("TileManager not found!");
        }

        // Find existing player
        currentPlayer = GameObject.FindGameObjectWithTag("Player");

        if (currentPlayer == null)
        {
            Debug.LogError("Player not found in the scene!");
        }
    }

    public void TriggerGameOver()
    {
        ScoreManager.Instance.SaveFinalScore(); // Store final score before showing UI
        Debug.Log("Game Over triggered!");
        gameOverUI.SetActive(true);

        // Now find the Score text after UI is enabled
        if (finalScoreText == null)
        {
            finalScoreText = gameOverUI.transform.Find("Score")?.GetComponent<TextMeshProUGUI>();
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + ScoreManager.FinalScore;
        }
        else
        {
            Debug.LogError("Text object named 'Score' not found inside Game Over UI!");
        }

        if (coinsCollectedText == null)
        {
            coinsCollectedText = gameOverUI.transform.Find("Coins Collected")?.GetComponent<TextMeshProUGUI>();
        }

        if (coinsCollectedText != null)
        {
            coinsCollectedText.text = "Coins Collected: " + CoinManager.Instance.GetTotalCoins();
        }
        else
        {
            Debug.LogError("Text object named 'Coins Collected' not found inside Game Over UI!");
        }

        Time.timeScale = 0f; // Pause the game

        if (GameSoundManager.instance != null)
        {
            GameSoundManager.instance.PlaySound(GameSoundManager.instance.loseSound);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume game

        // Reset tiles but keep the player
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (tileManager != null)
        {
            tileManager.ResetTiles();
        }

        // Reset player position
        if (currentPlayer == null)
        {
            currentPlayer = GameObject.FindGameObjectWithTag("Player");
        }

        if (currentPlayer != null)
        {
            currentPlayer.transform.position = new Vector3(0f, 1f, 0f);
        }

        gameOverUI.SetActive(false);
    }

    public void GoToMainMenu()
    {
        gameOverUI.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}