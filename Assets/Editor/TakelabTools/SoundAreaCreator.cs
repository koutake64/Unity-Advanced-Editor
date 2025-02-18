using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SoundAreaCreator : EditorWindow
{
    private string areaName = "NewSoundArea";
    private AudioClip areaClip;
    private List<SoundSourceChange.SoundLayer> soundLayers = new List<SoundSourceChange.SoundLayer>();

    [MenuItem("Tools/Sound Source Manager")]
    public static void ShowWindow()
    {
        GetWindow<SoundAreaCreator>("Sound Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sound Area Settings", EditorStyles.boldLabel);
        areaName = EditorGUILayout.TextField("Area Name", areaName);
        areaClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", areaClip, typeof(AudioClip), false);

        if (GUILayout.Button("Create Sound Area"))
        {
            CreateSoundArea();
        }
    }

    private void CreateSoundArea()
    {
        GameObject newArea = new GameObject(areaName);
        BoxCollider collider = newArea.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        // Nullチェックを追加
        if (soundLayers == null)
        {
            soundLayers = new List<SoundSourceChange.SoundLayer>();
        }

        // 通常のスクリプトとして SoundSourceChange をアタッチ
        SoundSourceChange soundChanger = newArea.AddComponent<SoundSourceChange>();
        soundChanger.soundLayers = new List<SoundSourceChange.SoundLayer>(soundLayers); // コピー

        Selection.activeGameObject = newArea;
    }
}
