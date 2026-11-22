using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public string CurrentPlayerName = "Player";

    public List<HighScoreEntry> HighScores = new List<HighScoreEntry>();
    public float PaddleSpeed = 2.0f;
    public float BallSpeed = 3.0f;
    public int BrickRows = 6;

    public string BestPlayerName => HighScores.Count > 0 ? HighScores[0].PlayerName : "None";
    public int BestScore => HighScores.Count > 0 ? HighScores[0].Score : 0;

    private string SaveFilePath => Application.persistentDataPath + "/savefile.json";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadDataFromDisk();
    }

    [System.Serializable]
    public class HighScoreEntry
    {
        public string PlayerName;
        public int Score;
    }

    [System.Serializable]
    class SaveData
    {
        public List<HighScoreEntry> HighScores = new List<HighScoreEntry>();
        public float PaddleSpeed = 2.0f;
        public float BallSpeed = 3.0f;
        public int BrickRows = 6;
    }

    public void AddHighScore(string playerName, int score)
    {
        if (score <= 0) return;

        HighScores.Add(new HighScoreEntry
        {
            PlayerName = string.IsNullOrEmpty(playerName) ? "Player" : playerName,
            Score = score
        });

        HighScores.Sort((a, b) => b.Score.CompareTo(a.Score));

        if (HighScores.Count > 5)
        {
            HighScores.RemoveRange(5, HighScores.Count - 5);
        }

        SaveDataToDisk();
    }

    public void ResetHighScores()
    {
        HighScores.Clear();
        SaveDataToDisk();
    }

    public void SaveDataToDisk()
    {
        SaveData data = new SaveData
        {
            HighScores = HighScores,
            PaddleSpeed = PaddleSpeed,
            BallSpeed = BallSpeed,
            BrickRows = BrickRows
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(SaveFilePath, json);
    }

    public void LoadDataFromDisk()
    {
        if (File.Exists(SaveFilePath))
        {
            string json = File.ReadAllText(SaveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            if (data != null)
            {
                HighScores = data.HighScores ?? new List<HighScoreEntry>();
                PaddleSpeed = data.PaddleSpeed > 0 ? data.PaddleSpeed : 2.0f;
                BallSpeed = data.BallSpeed > 0 ? data.BallSpeed : 3.0f;
                BrickRows = data.BrickRows > 0 ? data.BrickRows : 6;
            }
        }
    }
}