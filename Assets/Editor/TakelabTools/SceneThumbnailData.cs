using UnityEngine;

[CreateAssetMenu(fileName = "SceneThumbnailData", menuName = "Custom/SceneThumbnailData")]
public class SceneThumbnailData : ScriptableObject
{
	public Texture2D thumbnail; // カスタムサムネイル画像
	public string scenePath;    // 対応するシーンのパス
}
