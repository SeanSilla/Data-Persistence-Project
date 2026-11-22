using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoresUIHandler : MonoBehaviour
{
    public TMP_Text LeaderboardText;

    private void Start()
    {
        RefreshLeaderboard();
    }

    public void RefreshLeaderboard()
    {
        if (LeaderboardText == null) return;

        if (DataManager.Instance == null || DataManager.Instance.HighScores.Count == 0)
        {
            LeaderboardText.text = "No high scores yet!";
            return;
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < DataManager.Instance.HighScores.Count; i++)
        {
            var entry = DataManager.Instance.HighScores[i];
            sb.AppendLine($"{i + 1}. {entry.PlayerName} - {entry.Score}");
        }

        LeaderboardText.text = sb.ToString();
    }

    public void ResetScores()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.ResetHighScores();
            RefreshLeaderboard();
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}