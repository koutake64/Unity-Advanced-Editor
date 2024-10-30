using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class WorkTimeTracker : EditorWindow
{
    private DateTime startTime;                     // 作成開始時間
    private TimeSpan dailyWorkTime = TimeSpan.Zero; // 今日の作業時間
    private TimeSpan totalWorkTime = TimeSpan.Zero; // 累計作業時間
    private string currentScene = "";               // 現在のシーン名
    private TimeSpan currentSceneWorkTime = TimeSpan.Zero; // 現在のシーンの作業時間
    private DateTime lastUpdate;                           // 前回更新時間

    private Dictionary<string, TimeSpan> sceneWorkTimes = new Dictionary<string, TimeSpan>(); // 各シーンの作業時間

    private const string StartTimeKey =      "WorkTimeTracker_startTime";
    private const string TotalWorkTimeKey =  "WorkTimeTracker_totalWorkTime";
    private const string DailyWorkTimeKey =  "WorkTimeTracker_dailyWorkTime";
    private const string SceneWorkTimesKey = "WorkTimeTracker_sceneWorkTimes";
    private const string LastUpdateKey =     "WorkTimeTracker_lastUpdate";

    [MenuItem("TakelabTools/Work Time Tracker")]
    public static void ShowWindow()
    {
        GetWindow<WorkTimeTracker>("Work Time Tracker");
    }

    private void OnEnable()
    {
        //===== 前回のデータを読み込む =====
        if (EditorPrefs.HasKey(StartTimeKey))
            startTime = DateTime.Parse(EditorPrefs.GetString(StartTimeKey));
        else
            startTime = DateTime.Now;

        if (EditorPrefs.HasKey(TotalWorkTimeKey))
            totalWorkTime = TimeSpan.Parse(EditorPrefs.GetString(TotalWorkTimeKey));

        if (EditorPrefs.HasKey(DailyWorkTimeKey))
            dailyWorkTime = TimeSpan.Parse(EditorPrefs.GetString(DailyWorkTimeKey));

        if (EditorPrefs.HasKey(LastUpdateKey))
            lastUpdate = DateTime.Parse(EditorPrefs.GetString(LastUpdateKey));
        else
            lastUpdate = DateTime.Now;

        // Sceneごとの作業時間を読み込む
        LoadSceneWorkTimes();

        currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // 現在のシーンの作業時間を取得
        if (sceneWorkTimes.ContainsKey(currentScene))
        {
            currentSceneWorkTime = sceneWorkTimes[currentScene];
        }
        else
        {
            currentSceneWorkTime = TimeSpan.Zero;
        }
    }

    //===== データ保存 =====
    private void OnDisable()
    {
        EditorPrefs.SetString(StartTimeKey, startTime.ToString());
        EditorPrefs.SetString(TotalWorkTimeKey, totalWorkTime.ToString());
        EditorPrefs.SetString(DailyWorkTimeKey, dailyWorkTime.ToString());
        EditorPrefs.SetString(LastUpdateKey, lastUpdate.ToString());

        //--- Sceneごとの作業時間を保存
        SaveSceneWorkTimes();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("作成開始日時  : ", startTime.ToString());
        EditorGUILayout.LabelField("今日の作業時間: ", FormatTime(dailyWorkTime));
        EditorGUILayout.LabelField("累計作業時間  : ", FormatTime(totalWorkTime));
        EditorGUILayout.LabelField("現在のシーン  : ", currentScene);
        EditorGUILayout.LabelField("シーンごとの作業時間: ", FormatTime(currentSceneWorkTime));

        if (GUILayout.Button("リセット"))
        {
            ResetTimes();
        }
    }

    private void Update()
    {
        var deltaTime = DateTime.Now - lastUpdate;
        lastUpdate = DateTime.Now;

        // 作業時間の更新
        dailyWorkTime += deltaTime;
        totalWorkTime += deltaTime;

        currentSceneWorkTime += deltaTime;

        sceneWorkTimes[currentScene] = currentSceneWorkTime;

        Repaint();
    }

    private void ResetTimes()
    {
        dailyWorkTime = TimeSpan.Zero;
        totalWorkTime = TimeSpan.Zero;
        currentSceneWorkTime = TimeSpan.Zero;
        startTime = DateTime.Now;

        sceneWorkTimes.Clear();
    }

    private string FormatTime(TimeSpan timeSpan)
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}",
            timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }

    private void SaveSceneWorkTimes()
    {
        string sceneData = "";
        foreach (var kvp in sceneWorkTimes)
        {
            sceneData += kvp.Key + "|" + kvp.Value.ToString() + ";";
        }

        // 保存
        EditorPrefs.SetString(SceneWorkTimesKey, sceneData);
    }

    private void LoadSceneWorkTimes()
    {
        if (EditorPrefs.HasKey(SceneWorkTimesKey))
        {
            string sceneData = EditorPrefs.GetString(SceneWorkTimesKey);
            string[] sceneEntries = sceneData.Split(';');

            sceneWorkTimes.Clear();

            foreach (var entry in sceneEntries)
            {
                if (!string.IsNullOrEmpty(entry))
                {
                    string[] sceneInfo = entry.Split('|');
                    string sceneName = sceneInfo[0];
                    TimeSpan sceneTime = TimeSpan.Parse(sceneInfo[1]);
                    sceneWorkTimes[sceneName] = sceneTime;
                }
            }
        }
    }
}
