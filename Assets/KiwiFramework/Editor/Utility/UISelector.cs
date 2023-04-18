using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace KiwiFramework.Editor.Utility
{
	[InitializeOnLoad]
	public static class UISelector
	{
		static UISelector() { SceneView.duringSceneGui += OnSceneGUI; }

		private static void OnSceneGUI(SceneView sceneView)
		{
			var current = Event.current;
			if (current is {button: 1, type: EventType.MouseUp})
			{
				current.Use();
				// 当前屏幕坐标，左上角是（0，0）右下角（camera.pixelWidth，camera.pixelHeight）
				var mousePosition = Event.current.mousePosition;
				// Retina 屏幕需要拉伸值
				var mult = EditorGUIUtility.pixelsPerPoint;
				// 转换成摄像机可接受的屏幕坐标，左下角是（0，0，0）右上角是（camera.pixelWidth，camera.pixelHeight，0）
				mousePosition.y =  sceneView.camera.pixelHeight - mousePosition.y * mult;
				mousePosition.x *= mult;
				var scenes = GetAllScenes();
				var groups = scenes
				             .Where(m => m.isLoaded)
				             .SelectMany(m => m.GetRootGameObjects())
				             .Where(m => m.activeInHierarchy)
				             .SelectMany(m => m.GetComponentsInChildren<RectTransform>())
				             .Where(m => RectTransformUtility.RectangleContainsScreenPoint(m, mousePosition, sceneView.camera))
				             .GroupBy(m => m.gameObject.scene.name)
				             .ToArray();
				var sceneCount = scenes.Count(m => m.isLoaded);
				var gc         = new GenericMenu();
				var dic        = new Dictionary<string, int>();
				foreach (var group in groups)
				{
					foreach (var rt in group)
					{
						var name              = rt.name;
						var sceneName         = rt.gameObject.scene.name;
						var nameWithSceneName = sceneName + "/" + name;
						var isContains        = dic.ContainsKey(nameWithSceneName);
						var text              = sceneCount <= 1 ? name : nameWithSceneName;
						if (isContains)
						{
							var count = dic[nameWithSceneName]++;
							text += " [" + count + "]";
						}

						var content = new GUIContent(text);
						gc.AddItem(content, false, () =>
						{
							Selection.activeTransform = rt;
							EditorGUIUtility.PingObject(rt.gameObject);
						});
						if (!isContains)
						{
							dic.Add(nameWithSceneName, 1);
						}
					}
				}

				gc.ShowAsContext();
			}
		}

		private static IEnumerable<Scene> GetAllScenes()
		{
			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				yield return SceneManager.GetSceneAt(i);
			}

			var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			if (prefabStage != null)
			{
				yield return prefabStage.scene;
			}
		}
	}
}