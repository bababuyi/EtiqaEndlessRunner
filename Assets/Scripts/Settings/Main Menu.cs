using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestScoreText;

    private void Start()
    {
        if (bestScoreText != null)
            bestScoreText.text = "Best: " + PlayerPrefs.GetInt("HighScore", 0);
    }

    #region Button Callbacks

    public void OnPlayButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        SceneManager.LoadScene("Main Game");
    }

    public void OnSettingsButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        UIManager.Instance?.OnOpenSettingsButton();
    }

    public void OnQuitButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        StartCoroutine(QuitAfterDelay());
    }

    #endregion

    #region Quit

    private IEnumerator QuitAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion
}