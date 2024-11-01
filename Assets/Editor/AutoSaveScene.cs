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
//       :2024/11/01 �񑀍쎞�̏�����ǉ�
//_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.SceneManagement;
using System.Globalization;

public class AutoSaveScene : EditorWindow
{
    private static bool autoSaveEnabled = false;                // �Z�[�u����
    private static float saveInterval = 10f;                    // �Z�[�u�C���^�[�o��
    private static string backupFolder = "Assets/SceneBackUp";  // Assets/SceneBackUp�ɍ쐬
    private float nextSaveTime;                                 // ���Ԍv��
    private static float idleTimeLimit = 5 * 60f;               // �񑀍쎞��
    private float lastActivityTime;                             // �A�N�e�B�u���Ԃ̌v��
    private bool isAutoSavePaused = false;                      // ���씻��

    // ===== ���� =====
    private static bool isJapanese = CultureInfo.CurrentCulture.Name == "ja-JP";

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

        // �񑀍쎞�Ԃ̐ݒ�
        idleTimeLimit = EditorGUILayout.FloatField(isJapanese ? "�񑀍쎞�Ԃ̐ݒ� (��)" : "Idle Time Limit (Minutes)", idleTimeLimit / 60) * 60;

        if (GUILayout.Button(isJapanese ? "�K�p" : "Apply"))
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

            ShowNotification(new GUIContent("�K�p����"));
        }

        if (GUILayout.Button(isJapanese ? "�w���v���J��" : "Open Help"))
        {
            Application.OpenURL("https://github.com/koutake64/Unity-Advanced-Editor/wiki/AutoSaveScene");
        }
    }

    void AutoSave()
    {
        if (!autoSaveEnabled) return;

        // ���݂̎��ԂƍŏI���쎞�ԂƂ̍����`�F�b�N
        if ((float)EditorApplication.timeSinceStartup - lastActivityTime >= idleTimeLimit)
        {
            if (!isAutoSavePaused)
            {
                isAutoSavePaused = true;
                Debug.Log(isJapanese ? "�񑀍쎞�Ԃ��o�߂������߁A�����ۑ�����~���܂����B" : "Auto-save paused due to inactivity.");
            }
            return;
        }

        // �񑀍��Ԃ���ĊJ���ꂽ�ꍇ
        if (isAutoSavePaused)
        {
            isAutoSavePaused = false;
            Debug.Log(isJapanese ? "�����ۑ����ĊJ���܂��B" : "Auto-save resumed due to activity.");
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
            Debug.Log(isJapanese ? $"�V�[���� {backupPath} �ɕۑ����܂����B" : $"Scene saved to {backupPath}");
        }
        else
        {
            Debug.LogError(isJapanese ? "�V�[���̕ۑ��Ɏ��s���܂����B" : "Scene saving failed.");
        }
    }

    // �}�E�X�̓�����N���b�N�ȂǂōŏI���쎞�Ԃ��X�V
    void OnInspectorUpdate()
    {
        lastActivityTime = (float)EditorApplication.timeSinceStartup;
        Repaint();
    }
}
