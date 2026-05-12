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
        AudioManager.Instance?.PlayButtonClick();
        SceneManager.LoadScene("Main Game");
        //if (FindFirstObjectByType<TileManager>() != null)
            //FindFirstObjectByType<TileManager>().ResetTiles();
    }

    public void QuitGame()
    {
        AudioManager.Instance?.PlayButtonClick();
        StartCoroutine(QuitAfterDelay());
    }

    private IEnumerator QuitAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenSettings()
    {
        PlaySound();
        Debug.Log("Settings Opened!"); // Settings logic
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
