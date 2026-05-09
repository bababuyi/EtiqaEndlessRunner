using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsMenubackup : MonoBehaviour

{
    public GameObject settingsPanel;
    public GameObject mainMenuButtons;
    public Toggle soundToggle;
    public Slider volumeSlider;
    public AudioSource backgroundMusic;
    public AudioSource[] soundEffects;

    public AudioSource uiAudioSource; // One AudioSource for all UI sounds
    public AudioClip uiSoundEffect; // Single sound effect for everything

    void Start()
    {
        // Load saved settings
        soundToggle.isOn = PlayerPrefs.GetInt("SoundEnabled", 1) == 1;
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);

        // Apply settings
        ApplySettings();

        // Add listeners
        soundToggle.onValueChanged.AddListener(delegate { ToggleSound(soundToggle.isOn); PlaySound(); });
        volumeSlider.onValueChanged.AddListener(delegate { ChangeVolume(volumeSlider.value); });

        // Detect when slider is released (not while dragging)
        EventTrigger trigger = volumeSlider.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((eventData) => PlaySound());
        trigger.triggers.Add(entry);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
       // backButton.SetActive(false);
        PlaySound(); // Play sound when opening settings
    }

    public void CloseSettings()
    {
        if (uiAudioSource != null && uiSoundEffect != null)
        {
            uiAudioSource.PlayOneShot(uiSoundEffect); // 🔊 Play sound first
        }

        // Switch menu immediately, sound keeps playing
        settingsPanel.SetActive(false);
      //  backButton.SetActive(true);
    }

    void ToggleSound(bool isSoundOn)
    {
        backgroundMusic.mute = !isSoundOn;

        foreach (AudioSource sfx in soundEffects)
        {
            sfx.mute = !isSoundOn;
        }

        PlayerPrefs.SetInt("SoundEnabled", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    void ChangeVolume(float volume)
    {
        backgroundMusic.volume = volume;

        foreach (AudioSource sfx in soundEffects)
        {
            sfx.volume = volume;
        }

        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }

    void ApplySettings()
    {
        bool isSoundOn = soundToggle.isOn;
        float volume = volumeSlider.value;

        backgroundMusic.mute = !isSoundOn;
        backgroundMusic.volume = volume;

        foreach (AudioSource sfx in soundEffects)
        {
            sfx.mute = !isSoundOn;
            sfx.volume = volume;
        }
    }

    void PlaySound()
    {
        if (uiAudioSource != null && uiSoundEffect != null)
        {
            uiAudioSource.PlayOneShot(uiSoundEffect);
        }
    }
}