using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-50)]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip coinCollectSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip flyPowerUpSound;
    [SerializeField] private AudioClip invincibilityPowerUpSound;
    [SerializeField] private AudioClip doubleCoinPowerUpSound;
    [SerializeField] private AudioClip highJumpPowerUpSound;

    private float currentVolume = 1f;
    private bool isMuted = false;
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
        LoadAudioSettings();
        SubscribeToGameManager();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnDestroy() => UnsubscribeFromGameManager();

    #endregion

    #region GameManager Event Wiring

    private void SubscribeToGameManager()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnGameOver += OnGameOverHandler;
    }

    private void UnsubscribeFromGameManager()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnGameOver -= OnGameOverHandler;
    }

    private void OnGameOverHandler(int score, int coins) => PlayLose();

    #endregion

    #region Music

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip clip = scene.name == "Main Game" ? gameplayMusic : mainMenuMusic;
        PlayMusic(clip);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic() => musicSource?.Stop();

    #endregion

    #region SFX — Named Public API

    public void PlayButtonClick() => PlaySFX(buttonClickSound);
    public void PlayCoinCollect() => PlaySFX(coinCollectSound);
    public void PlayHit() => PlaySFX(hitSound);
    public void PlayLose() => PlaySFX(loseSound);
    public void PlayFlyPowerUp() => PlaySFX(flyPowerUpSound);
    public void PlayInvincibility() => PlaySFX(invincibilityPowerUpSound);
    public void PlayDoubleCoin() => PlaySFX(doubleCoinPowerUpSound);
    public void PlayHighJump() => PlaySFX(highJumpPowerUpSound);

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null || isMuted) return;

        sfxSource.PlayOneShot(clip);
    }

    #endregion

    #region Volume Control

    public void SetVolume(float volume)
    {
        currentVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
    }

    public void SetMuted(bool muted)
    {
        isMuted = muted;
        ApplyAudioSettings();
    }

    private void ApplyAudioSettings()
    {
        if (musicSource != null)
        {
            musicSource.volume = isMuted ? 0f : currentVolume;
            musicSource.mute = isMuted;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = isMuted ? 0f : currentVolume;
            sfxSource.mute = isMuted;
        }
    }

    private void LoadAudioSettings()
    {
        currentVolume = PlayerPrefs.GetFloat("Volume", 1f);
        isMuted = PlayerPrefs.GetInt("SoundEnabled", 1) == 0;
        ApplyAudioSettings();
    }

    #endregion
}