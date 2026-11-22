using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsUIHandler : MonoBehaviour
{
    public Slider PaddleSpeedSlider;
    public Slider BallSpeedSlider;
    public Slider BrickRowsSlider;

    public TMP_Text PaddleSpeedLabel;
    public TMP_Text BallSpeedLabel;
    public TMP_Text BrickRowsLabel;

    private void Start()
    {
        if (DataManager.Instance != null)
        {
            if (PaddleSpeedSlider != null) PaddleSpeedSlider.value = DataManager.Instance.PaddleSpeed;
            if (BallSpeedSlider != null) BallSpeedSlider.value = DataManager.Instance.BallSpeed;
            if (BrickRowsSlider != null) BrickRowsSlider.value = DataManager.Instance.BrickRows;
        }

        UpdateLabels();
    }

    public void UpdateLabels()
    {
        if (PaddleSpeedLabel != null && PaddleSpeedSlider != null)
            PaddleSpeedLabel.text = $"Paddle Speed: {PaddleSpeedSlider.value:F1}";

        if (BallSpeedLabel != null && BallSpeedSlider != null)
            BallSpeedLabel.text = $"Ball Max Speed: {BallSpeedSlider.value:F1}";

        if (BrickRowsLabel != null && BrickRowsSlider != null)
            BrickRowsLabel.text = $"Brick Rows: {(int)BrickRowsSlider.value}";
    }

    public void SaveAndBack()
    {
        if (DataManager.Instance != null)
        {
            if (PaddleSpeedSlider != null) DataManager.Instance.PaddleSpeed = PaddleSpeedSlider.value;
            if (BallSpeedSlider != null) DataManager.Instance.BallSpeed = BallSpeedSlider.value;
            if (BrickRowsSlider != null) DataManager.Instance.BrickRows = (int)BrickRowsSlider.value;

            DataManager.Instance.SaveDataToDisk();
        }

        SceneManager.LoadScene("Menu");
    }
}