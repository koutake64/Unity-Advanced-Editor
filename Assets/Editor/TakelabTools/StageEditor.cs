using UnityEditor;
using UnityEngine;
using System.IO;

public class StageEditor : EditorWindow
{
	private int width = 5;
	private int height = 5;
	private int[,] stageData;

	[MenuItem("TakelabTools/Stage Editor")]
	public static void ShowWindow()
	{
		GetWindow<StageEditor>("Stage Editor");
	}

	private void OnGUI()
	{
		width = EditorGUILayout.IntField("Width", width);
		height = EditorGUILayout.IntField("Height", height);

		if (stageData == null || stageData.GetLength(0) != width || stageData.GetLength(1) != height)
			stageData = new int[width, height];

		for (int y = 0; y < height; y++)
		{
			EditorGUILayout.BeginHorizontal();
			for (int x = 0; x < width; x++)
			{
				stageData[x, y] = EditorGUILayout.IntField(stageData[x, y]);
			}
			EditorGUILayout.EndHorizontal();
		}

		if (GUILayout.Button("Export CSV"))
		{
			SaveCSV();
		}
	}

	private void SaveCSV()
	{
		string path = EditorUtility.SaveFilePanel("Save CSV", "", "stage.csv", "csv");
		using (StreamWriter writer = new StreamWriter(path))
		{
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					writer.Write(stageData[x, y]);
					if (x < width - 1) writer.Write(",");
				}
				writer.WriteLine();
			}
		}
		Debug.Log("CSV Exported: " + path);
	}
}
