using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[InitializeOnLoad]
public static class SetupBonusScenes
{
    static SetupBonusScenes()
    {
        EditorApplication.delayCall += RunIfNeeded;
    }

    private static void RunIfNeeded()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;

        bool missingScenes = !File.Exists("Assets/Scenes/HighScores.unity") || !File.Exists("Assets/Scenes/Settings.unity");
        bool missingMainMenuBtn = File.Exists("Assets/Scenes/main.unity") && !File.ReadAllText("Assets/Scenes/main.unity").Contains("BackToMenu");

        if (!missingScenes && !missingMainMenuBtn)
            return;

        if (missingScenes)
            RunSetup();
        else
            AddMenuButtonToMain();
    }

    [MenuItem("Tools/Setup Bonus Scenes")]
    public static void RunSetup()
    {
        CreateHighScoresScene();
        CreateSettingsScene();
        UpdateMenuScene();
        UpdateMainScene();
        UpdateBuildSettings();

        EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity", OpenSceneMode.Single);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[SetupBonusScenes] HighScores.unity, Settings.unity, Menu.unity, main.unity, and Build Settings are all configured!");
    }

    [MenuItem("Tools/Add Menu Button To Main")]
    public static void AddMenuButtonToMain()
    {
        UpdateMainScene();
        EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity", OpenSceneMode.Single);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[SetupBonusScenes] Added Menu button to main.unity!");
    }

    private static Canvas CreateSceneBase(out GameObject canvasObj)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var camObj = new GameObject("Main Camera");
        var cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.tag = "MainCamera";

        var esObj = new GameObject("EventSystem");
        esObj.AddComponent<EventSystem>();
        esObj.AddComponent<InputSystemUIInputModule>();

        canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        return canvas;
    }

    private static TextMeshProUGUI CreateTMPText(Transform parent, string name, string text, Vector2 anchoredPos, Vector2 size, float fontSize, TextAlignmentOptions align = TextAlignmentOptions.Center)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = align;
        tmp.color = Color.white;
        return tmp;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPos, Vector2 size)
    {
        var resources = new DefaultControls.Resources();
        var btnObj = DefaultControls.CreateButton(resources);
        btnObj.name = name;
        btnObj.transform.SetParent(parent, false);

        var rt = btnObj.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        var legacyText = btnObj.GetComponentInChildren<Text>();
        if (legacyText != null)
        {
            legacyText.text = label;
            legacyText.fontSize = 24;
            legacyText.color = Color.black;
        }

        return btnObj.GetComponent<Button>();
    }

    private static Slider CreateLabeledSlider(Transform parent, string name, float min, float max, float val, bool wholeNumbers, Vector2 pos, out TMP_Text label)
    {
        label = CreateTMPText(parent, name + "Label", name, pos + new Vector2(0, 45), new Vector2(500, 45), 30);

        var resources = new DefaultControls.Resources();
        var sliderObj = DefaultControls.CreateSlider(resources);
        sliderObj.name = name;
        sliderObj.transform.SetParent(parent, false);

        var rt = sliderObj.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(460, 35);

        var slider = sliderObj.GetComponent<Slider>();
        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = wholeNumbers;
        slider.value = val;
        return slider;
    }

    private static void CreateHighScoresScene()
    {
        CreateSceneBase(out var canvasObj);
        var parent = canvasObj.transform;

        CreateTMPText(parent, "TitleText", "HIGH SCORES", new Vector2(0, 320), new Vector2(700, 90), 56);
        var leaderboard = CreateTMPText(parent, "LeaderboardText", "No high scores yet!", new Vector2(0, 40), new Vector2(800, 400), 36);

        var handler = canvasObj.AddComponent<HighScoresUIHandler>();
        handler.LeaderboardText = leaderboard;

        var backBtn = CreateButton(parent, "Back Button", "Back", new Vector2(-140, -280), new Vector2(220, 60));
        UnityEventTools.AddPersistentListener(backBtn.onClick, handler.BackToMenu);

        var resetBtn = CreateButton(parent, "Reset Button", "Reset Scores", new Vector2(140, -280), new Vector2(220, 60));
        UnityEventTools.AddPersistentListener(resetBtn.onClick, handler.ResetScores);

        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/Scenes/HighScores.unity");
    }

    private static void CreateSettingsScene()
    {
        CreateSceneBase(out var canvasObj);
        var parent = canvasObj.transform;

        CreateTMPText(parent, "TitleText", "SETTINGS", new Vector2(0, 340), new Vector2(700, 90), 56);

        var handler = canvasObj.AddComponent<SettingsUIHandler>();

        handler.PaddleSpeedSlider = CreateLabeledSlider(parent, "PaddleSpeedSlider", 1f, 5f, 2f, false, new Vector2(0, 160), out handler.PaddleSpeedLabel);
        handler.BallSpeedSlider = CreateLabeledSlider(parent, "BallSpeedSlider", 2f, 6f, 3f, false, new Vector2(0, 20), out handler.BallSpeedLabel);
        handler.BrickRowsSlider = CreateLabeledSlider(parent, "BrickRowsSlider", 2f, 6f, 6f, true, new Vector2(0, -120), out handler.BrickRowsLabel);

        UnityEventTools.AddVoidPersistentListener(handler.PaddleSpeedSlider.onValueChanged, handler.UpdateLabels);
        UnityEventTools.AddVoidPersistentListener(handler.BallSpeedSlider.onValueChanged, handler.UpdateLabels);
        UnityEventTools.AddVoidPersistentListener(handler.BrickRowsSlider.onValueChanged, handler.UpdateLabels);

        var saveBtn = CreateButton(parent, "Save Button", "Save & Back", new Vector2(0, -280), new Vector2(260, 65));
        UnityEventTools.AddPersistentListener(saveBtn.onClick, handler.SaveAndBack);

        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/Scenes/Settings.unity");
    }

    private static void UpdateMenuScene()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity", OpenSceneMode.Single);
        var handler = Object.FindFirstObjectByType<MenuUIHandler>();
        var canvas = Object.FindFirstObjectByType<Canvas>();

        if (handler != null && canvas != null)
        {
            if (GameObject.Find("HighScores Button") == null)
            {
                var hsBtn = CreateButton(canvas.transform, "HighScores Button", "High Scores", new Vector2(-120, -210), new Vector2(200, 50));
                UnityEventTools.AddPersistentListener(hsBtn.onClick, handler.OpenHighScores);
            }

            if (GameObject.Find("Settings Button") == null)
            {
                var setBtn = CreateButton(canvas.transform, "Settings Button", "Settings", new Vector2(120, -210), new Vector2(200, 50));
                UnityEventTools.AddPersistentListener(setBtn.onClick, handler.OpenSettings);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }

    private static void UpdateMainScene()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/main.unity", OpenSceneMode.Single);
        var mainMgr = Object.FindFirstObjectByType<MainManager>();
        var canvas = Object.FindFirstObjectByType<Canvas>();

        if (mainMgr != null && canvas != null && GameObject.Find("Menu Button") == null)
        {
            var menuBtn = CreateButton(canvas.transform, "Menu Button", "Menu", Vector2.zero, new Vector2(120, 36));
            var rt = menuBtn.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-15, -15);

            var legacyText = menuBtn.GetComponentInChildren<Text>();
            if (legacyText != null) legacyText.fontSize = 18;

            UnityEventTools.AddPersistentListener(menuBtn.onClick, mainMgr.BackToMenu);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }

    private static void UpdateBuildSettings()
    {
        var scenes = new List<EditorBuildSettingsScene>
        {
            new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", false),
            new EditorBuildSettingsScene("Assets/Scenes/Menu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/main.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/HighScores.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Settings.unity", true)
        };
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
