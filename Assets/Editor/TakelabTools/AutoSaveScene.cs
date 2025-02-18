//_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
// AutoSaveScene.cs
// 一定間隔でSceneのバックアップ拡張エディタ
//
// Author:Take.
// Create:2024/10/15
// Ver:0.11
//
// Use:上部タブからTakelab/Auto Save Scene Settingsから設定
//
// Update:2024/10/15 基盤作成
//       :2024/10/15 日本語対応
//       :2024/10/20 ヘルプ用URL追加
//       :2024/11/01 適用時の表示方法変更
//       :2024/11/01 ヘルプの表示方法を変更
//       :2024/11/01 非操作時の処理を追加
//_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.SceneManagement;
using System.Globalization;

public class AutoSaveScene : EditorWindow
{
    private static bool autoSaveEnabled = false;                // セーブ判定
    private static float saveInterval = 10f;                    // セーブインターバル
    private static string backupFolder = "Assets/SceneBackUp";  // Assets/SceneBackUpに作成
    private float nextSaveTime;                                 // 時間計測
    private static float idleTimeLimit = 5 * 60f;               // 非操作時間
    private float lastActivityTime;                             // アクティブ時間の計測
    private bool isAutoSavePaused = false;                      // 操作判定

    // ===== 言語 =====
    private static bool isJapanese = CultureInfo.CurrentCulture.Name == "ja-JP";

    [MenuItem("TakelabTools/Auto Save Scene Settings")]
    public static void ShowWindow()
    {
        GetWindow<AutoSaveScene>(isJapanese ? "自動保存設定" : "Auto Save Settings");
    }

    void OnGUI()
    {
        GUILayout.Label(isJapanese ? "シーンの自動保存" : "Auto Save Scene", EditorStyles.boldLabel);

        autoSaveEnabled = EditorGUILayout.Toggle(isJapanese ? "自動保存を有効にする" : "Enable Auto Save", autoSaveEnabled);
        saveInterval = EditorGUILayout.FloatField(isJapanese ? "保存間隔 (分)" : "Save Interval (Minutes)", saveInterval);

        // 非操作時間の設定
        idleTimeLimit = EditorGUILayout.FloatField(isJapanese ? "非操作時間の設定 (分)" : "Idle Time Limit (Minutes)", idleTimeLimit / 60) * 60;

        if (GUILayout.Button(isJapanese ? "適用" : "Apply"))
        {
            if (autoSaveEnabled)
            {
                nextSaveTime = (float)EditorApplication.timeSinceStartup + saveInterval * 60;
                lastActivityTime = (float)EditorApplication.timeSinceStartup;
                EditorApplication.update += AutoSave;
            }
            else
            {
                EditorApplication.update -= AutoSave;
            }

            ShowNotification(new GUIContent("適用完了"));
        }

        if (GUILayout.Button(isJapanese ? "ヘルプを開く" : "Open Help"))
        {
            Application.OpenURL("https://github.com/koutake64/Unity-Advanced-Editor/wiki/AutoSaveScene");
        }
    }

    void AutoSave()
    {
        if (!autoSaveEnabled) return;

        // 現在の時間と最終操作時間との差をチェック
        if ((float)EditorApplication.timeSinceStartup - lastActivityTime >= idleTimeLimit)
        {
            if (!isAutoSavePaused)
            {
                isAutoSavePaused = true;
                Debug.Log(isJapanese ? "非操作時間が経過したため、自動保存が停止しました。" : "Auto-save paused due to inactivity.");
            }
            return;
        }

        // 非操作状態から再開された場合
        if (isAutoSavePaused)
        {
            isAutoSavePaused = false;
            Debug.Log(isJapanese ? "自動保存を再開します。" : "Auto-save resumed due to activity.");
            nextSaveTime = (float)EditorApplication.timeSinceStartup + saveInterval * 60;
        }

        if (EditorApplication.timeSinceStartup >= nextSaveTime)
        {
            SaveCurrentScene();
            nextSaveTime = (float)EditorApplication.timeSinceStartup + saveInterval * 60;
        }
    }

    void SaveCurrentScene()
    {
        if (!Directory.Exists(backupFolder))
        {
            Directory.CreateDirectory(backupFolder);
            AssetDatabase.Refresh();
        }

        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string backupPath = Path.Combine(backupFolder, $"{sceneName}_{timestamp}.unity");

        bool saveSuccessful = EditorSceneManager.SaveScene(currentScene, backupPath, true);

        if (saveSuccessful)
        {
            Debug.Log(isJapanese ? $"シーンを {backupPath} に保存しました。" : $"Scene saved to {backupPath}");
        }
        else
        {
            Debug.LogError(isJapanese ? "シーンの保存に失敗しました。" : "Scene saving failed.");
        }
    }

    // マウスの動きやクリックなどで最終操作時間を更新
    void OnInspectorUpdate()
    {
        lastActivityTime = (float)EditorApplication.timeSinceStartup;
        Repaint();
    }
}
