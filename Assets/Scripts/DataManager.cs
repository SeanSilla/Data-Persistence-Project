using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public string CurrentPlayerName = "Player";
    public string BestPlayerName = "None";
    public int BestScore = 0;

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
        LoadBestScore();
    }

    [System.Serializable]
    class SaveData
    {
        public string BestPlayerName;
        public int BestScore;
    }

    public void SaveBestScore()
    {
        SaveData data = new SaveData
        {
            BestPlayerName = BestPlayerName,
            BestScore = BestScore
        };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(SaveFilePath, json);
    }
    public void LoadBestScore()
    {
        if (File.Exists(SaveFilePath))
        {
            string json = File.ReadAllText(SaveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            BestPlayerName = data.BestPlayerName;
            BestScore = data.BestScore;
        }
    }
}