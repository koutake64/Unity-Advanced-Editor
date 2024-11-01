//_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
// AutoSaveScene.cs
// ���Ԋu��Scene�̃o�b�N�A�b�v�g���G�f�B�^
//
// Author:Take.
// Create:2024/10/15
// Ver:0.11
//
// Use:�㕔�^�u����Takelab/Auto Save Scene Settings����ݒ�
//
// Update:2024/10/15 ��Ս쐬
//       :2024/10/15 ���{��Ή�
//       :2024/10/20 �w���v�pURL�ǉ�
//       :2024/11/01 �K�p���̕\�����@�ύX
//       :2024/11/01 �w���v�̕\�����@��ύX
//_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.SceneManagement;
using System.Globalization;

public class AutoSaveScene : EditorWindow
{
    private static bool autoSaveEnabled = false;
    private static float saveInterval = 10f; // �f�t�H���g10��
    private static string backupFolder = "Assets/SceneBackUp"; // Unity�̃A�Z�b�g�t�H���_���ɍ쐬
    private float nextSaveTime;

    // ===== ����֘A =====
    private static bool isJapanese = CultureInfo.CurrentCulture.Name == "ja-JP";    // ���{��


    [MenuItem("TakelabTools/Auto Save Scene Settings")]
    public static void ShowWindow()
    {
        GetWindow<AutoSaveScene>(isJapanese ? "�����ۑ��ݒ�" : "Auto Save Settings");
    }

    void OnGUI()
    {
        GUILayout.Label(isJapanese ? "�V�[���̎����ۑ�" : "Auto Save Scene", EditorStyles.boldLabel);

        autoSaveEnabled = EditorGUILayout.Toggle(isJapanese ? "�����ۑ���L���ɂ���" : "Enable Auto Save", autoSaveEnabled);
        saveInterval = EditorGUILayout.FloatField(isJapanese ? "�ۑ��Ԋu (��)" : "Save Interval (Minutes)", saveInterval);

        if (GUILayout.Button(isJapanese ? "�K�p" : "Apply"))
        {
            if (autoSaveEnabled)
            {
                nextSaveTime = (float)EditorApplication.timeSinceStartup + saveInterval * 60;
                EditorApplication.update += AutoSave;
            }
            else
            {
                EditorApplication.update -= AutoSave;
            }

            ShowNotification(new GUIContent("�K�p����"));
        }

        //--- �w���v�{�^��
        if (GUILayout.Button(isJapanese ? "�w���v���J��" : "Open Help"))
        {
            Application.OpenURL("https://github.com/koutake64/Unity-Advanced-Editor/wiki/AutoSaveScene");
        }
    }

    void AutoSave()
    {
        if (!autoSaveEnabled) return;

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

        // �V�[����ۑ�
        bool saveSuccessful = EditorSceneManager.SaveScene(currentScene, backupPath, true);

        if (saveSuccessful)
        {
            Debug.Log(isJapanese ? $"�V�[���� {backupPath} �ɕۑ�����܂����B" : $"Scene saved to {backupPath}");
        }
        else
        {
            Debug.LogError(isJapanese ? "�V�[���̕ۑ��Ɏ��s���܂����B" : "Scene saving failed.");
        }
    }

}