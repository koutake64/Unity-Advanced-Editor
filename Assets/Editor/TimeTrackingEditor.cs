using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class WorkTimeTracker : EditorWindow
{
    private DateTime startTime;                     // �쐬�J�n����
    private TimeSpan dailyWorkTime = TimeSpan.Zero; // �����̍�Ǝ���
    private TimeSpan totalWorkTime = TimeSpan.Zero; // �݌v��Ǝ���
    private string currentScene = "";               // ���݂̃V�[����
    private TimeSpan currentSceneWorkTime = TimeSpan.Zero; // ���݂̃V�[���̍�Ǝ���
    private DateTime lastUpdate;                           // �O��X�V����

    private Dictionary<string, TimeSpan> sceneWorkTimes = new Dictionary<string, TimeSpan>(); // �e�V�[���̍�Ǝ���

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
        //===== �O��̃f�[�^��ǂݍ��� =====
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

        // Scene���Ƃ̍�Ǝ��Ԃ�ǂݍ���
        LoadSceneWorkTimes();

        currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // ���݂̃V�[���̍�Ǝ��Ԃ��擾
        if (sceneWorkTimes.ContainsKey(currentScene))
        {
            currentSceneWorkTime = sceneWorkTimes[currentScene];
        }
        else
        {
            currentSceneWorkTime = TimeSpan.Zero;
        }
    }

    //===== �f�[�^�ۑ� =====
    private void OnDisable()
    {
        EditorPrefs.SetString(StartTimeKey, startTime.ToString());
        EditorPrefs.SetString(TotalWorkTimeKey, totalWorkTime.ToString());
        EditorPrefs.SetString(DailyWorkTimeKey, dailyWorkTime.ToString());
        EditorPrefs.SetString(LastUpdateKey, lastUpdate.ToString());

        //--- Scene���Ƃ̍�Ǝ��Ԃ�ۑ�
        SaveSceneWorkTimes();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("�쐬�J�n����  : ", startTime.ToString());
        EditorGUILayout.LabelField("�����̍�Ǝ���: ", FormatTime(dailyWorkTime));
        EditorGUILayout.LabelField("�݌v��Ǝ���  : ", FormatTime(totalWorkTime));
        EditorGUILayout.LabelField("���݂̃V�[��  : ", currentScene);
        EditorGUILayout.LabelField("�V�[�����Ƃ̍�Ǝ���: ", FormatTime(currentSceneWorkTime));

        if (GUILayout.Button("���Z�b�g"))
        {
            ResetTimes();
        }
    }

    private void Update()
    {
        var deltaTime = DateTime.Now - lastUpdate;
        lastUpdate = DateTime.Now;

        // ��Ǝ��Ԃ̍X�V
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

        // �ۑ�
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
