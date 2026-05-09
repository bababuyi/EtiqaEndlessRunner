using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource buttonClickSound;
    private Transform playerTransform; // Reference to the player's Transform

    void Start()
    {
        // Apply saved volume to button sounds
        buttonClickSound.volume = PlayerPrefs.GetFloat("Volume", 1f);
        buttonClickSound.mute = PlayerPrefs.GetInt("SoundEnabled", 1) == 0;
    }

    public void PlayGame()
    {
        PlaySound();
        SceneManager.LoadScene("Main Game");

        // Reset tiles when the game starts
        if (FindObjectOfType<TileManager>() != null)
        {
            FindObjectOfType<TileManager>().ResetTiles();
        }

        // Reset player position when starting the game
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerTransform.position = new Vector3(0, 1, 0);
        }
    }

    public void OpenSettings()
    {
        PlaySound();
        Debug.Log("Settings Opened!"); // Settings logic
    }

    public void QuitGame()
    {
        if (buttonClickSound != null && !buttonClickSound.mute)
        {
            buttonClickSound.Play();
        }

        StartCoroutine(QuitAfterSound()); // Quit after sound finishes
    }

    private IEnumerator QuitAfterSound()
    {
        if (buttonClickSound != null)
        {
            yield return new WaitForSeconds(buttonClickSound.clip.length); // Wait for sound to finish
        }

        Application.Quit(); // Quit game (works on Android & PC)

        // If testing in Unity Editor, stop play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void PlaySound()
    {
        if (buttonClickSound != null && !buttonClickSound.mute)
        {
            buttonClickSound.Play();
        }
    }
}
