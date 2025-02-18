using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneAsset))]
public class SceneThumbnailEditor : Editor
{
	private SceneThumbnailData thumbnailData;

	public override void OnInspectorGUI()
	{
		SceneAsset sceneAsset = (SceneAsset)target;
		string scenePath = AssetDatabase.GetAssetPath(sceneAsset);

		// サムネイルデータの検索
		if (thumbnailData == null || thumbnailData.scenePath != scenePath)
		{
			thumbnailData = FindThumbnailData(scenePath);
		}

		// サムネイルが存在すれば表示
		if (thumbnailData != null && thumbnailData.thumbnail != null)
		{
			GUILayout.Label("Custom Thumbnail:");
			GUILayout.Label(thumbnailData.thumbnail, GUILayout.Width(128), GUILayout.Height(128));
		}
		else
		{
			GUILayout.Label("No custom thumbnail found.");
		}

		// デフォルトのインスペクターを表示
		base.OnInspectorGUI();
	}

	private SceneThumbnailData FindThumbnailData(string scenePath)
	{
		// サムネイルデータを全てロードし、対応するシーンを探す
		var assets = AssetDatabase.FindAssets("t:SceneThumbnailData");
		foreach (string guid in assets)
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			SceneThumbnailData data = AssetDatabase.LoadAssetAtPath<SceneThumbnailData>(path);
			if (data != null && data.scenePath == scenePath)
			{
				return data;
			}
		}
		return null;
	}
}
