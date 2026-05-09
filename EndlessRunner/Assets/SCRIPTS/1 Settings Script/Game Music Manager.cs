using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMusicManager : MonoBehaviour
{
    public static GameMusicManager instance;
    public AudioSource gameMusicAudioSource; // AudioSource for the background music
    public AudioClip mainGameMusic; // Background music for the Main Game scene

    void Awake()
    {
        // Check if an instance of GameMusicManager already exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameMusicManager when transitioning between scenes
        }
    }

    void Start()
    {
        // Start playing background music if in Main Game scene
        if (SceneManager.GetActiveScene().name == "Main Game")
        {
            PlayMusic();
        }
        else
        {
            StopMusic();
        }
    }

    void OnEnable()
    {
        // Subscribe to the sceneLoaded event to check for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check which scene has been loaded and enable/disable music accordingly
        if (scene.name == "Main Game")
        {
            PlayMusic(); // Play game music in Main Game
        }
        else if (scene.name == "Main Menu")
        {
            StopMusic(); // Stop game music in Main Menu
        }
    }

    // Play the background music
    void PlayMusic()
    {
        if (gameMusicAudioSource != null && !gameMusicAudioSource.isPlaying)
        {
            gameMusicAudioSource.clip = mainGameMusic;
            gameMusicAudioSource.Play();
        }
    }

    // Stop the background music
    void StopMusic()
    {
        if (gameMusicAudioSource != null && gameMusicAudioSource.isPlaying)
        {
            gameMusicAudioSource.Stop();
        }
    }
}
