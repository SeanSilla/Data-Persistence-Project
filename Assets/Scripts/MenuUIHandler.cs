using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuUIHandler : MonoBehaviour
{
    public TMP_InputField NameInput;
    public Text BestScoreText;

    private void Start()
    {
        if (DataManager.Instance != null && BestScoreText != null)
        {
            BestScoreText.text = $"Best Score : {DataManager.Instance.BestPlayerName} : {DataManager.Instance.BestScore}";
        }
    }

    public void StartNew()
    {
        if (DataManager.Instance != null)
        {
            string enteredName = NameInput != null ? NameInput.text.Trim() : "";
            DataManager.Instance.CurrentPlayerName = string.IsNullOrEmpty(enteredName) ? "Player" : enteredName;
        }

        SceneManager.LoadScene("main");
    }

    public void OpenHighScores()
    {
        SceneManager.LoadScene("HighScores");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}