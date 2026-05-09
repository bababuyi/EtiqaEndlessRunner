using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundManager : MonoBehaviour
{
    public static GameSoundManager instance; // Singleton for easy access
    public AudioSource audioSource; // The single AudioSource

    public AudioClip buttonClickSound;
    public AudioClip coinCollectSound;
    public AudioClip flyPowerUpSound;
    public AudioClip invincibilityPowerUpSound;
    public AudioClip doubleCoinPowerUpSound;
    public AudioClip highJumpPowerUpSound;
    public AudioClip loseSound; 
    public AudioClip powerUpSound;
    public AudioClip hitSound;

    void Awake()
    {
        // Ensure only one instance of GameSoundManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep it across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}