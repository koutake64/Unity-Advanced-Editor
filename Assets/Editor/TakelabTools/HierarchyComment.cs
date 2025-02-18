//using UnityEngine;
//using UnityEditor;

//[InitializeOnLoad]
//public class HierarchyCommentEditor
//{
//	static HierarchyCommentEditor()
//	{
//		EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
//	}

//	private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
//	{
//		GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

//		if (obj == null) return;

//		CommentData commentData = obj.GetComponent<CommentData>();

//		if (commentData != null && !string.IsNullOrEmpty(commentData.comment))
//		{
//			コメントアイコンの表示
//		   Rect iconRect = new Rect(selectionRect.xMax - 20, selectionRect.y, 20, selectionRect.height);
//			GUIContent iconContent = EditorGUIUtility.IconContent("console.infoicon");
//			GUI.Label(iconRect, iconContent);

//			コメントをホバー時に表示
//			if (iconRect.Contains(Event.current.mousePosition))
//			{
//				GUIContent tooltip = new GUIContent(commentData.comment);
//				EditorGUI.LabelField(new Rect(selectionRect.xMax + 5, selectionRect.y, 300, selectionRect.height), tooltip);
//			}
//		}

//		右クリックメニューを拡張
//		if (Event.current.type == EventType.ContextClick && selectionRect.Contains(Event.current.mousePosition))
//		{
//			ShowContextMenu(obj, commentData);
//			Event.current.Use();
//		}
//	}

//	private static void ShowContextMenu(GameObject obj, CommentData commentData)
//	{
//		GenericMenu menu = new GenericMenu();

//		既存の標準メニューと競合しないよう、カスタムメニューを追加
//		menu.AddSeparator(""); // 区切り線を追加

//		if (commentData != null)
//		{
//			menu.AddItem(new GUIContent("コメント/編集"), false, () =>
//			{
//				Selection.activeGameObject = obj;
//				CommentWindow.ShowWindow(obj);
//			});
//			menu.AddItem(new GUIContent("コメント/削除"), false, () =>
//			{
//				Undo.DestroyObjectImmediate(commentData);
//			});
//		}
//		else
//		{
//			menu.AddItem(new GUIContent("コメント/追加"), false, () =>
//			{
//				Undo.AddComponent<CommentData>(obj);
//			});
//		}

//		全ての既存メニューを標準処理に委譲
//		EditorUtility.DisplayPopupMenu(new Rect(Event.current.mousePosition, Vector2.zero), "GameObject", null);

//		menu.ShowAsContext();
//	}
//}

//public class CommentWindow : EditorWindow
//{
//	private GameObject targetObject;
//	private CommentData commentData;

//	public static void ShowWindow(GameObject obj)
//	{
//		CommentWindow window = GetWindow<CommentWindow>("コメント編集");
//		window.targetObject = obj;
//		window.commentData = obj.GetComponent<CommentData>();
//	}

//	private void OnGUI()
//	{
//		if (targetObject == null || commentData == null)
//		{
//			EditorGUILayout.LabelField("ターゲットオブジェクトがありません。");
//			return;
//		}

//		EditorGUILayout.LabelField("対象: " + targetObject.name, EditorStyles.boldLabel);
//		commentData.comment = EditorGUILayout.TextArea(commentData.comment);

//		if (GUILayout.Button("保存"))
//		{
//			EditorUtility.SetDirty(commentData);
//			Close();
//		}
//	}
//}
