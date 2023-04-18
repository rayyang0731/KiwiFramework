using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace KiwiFramework.Editor
{
	internal partial class HierarchyExtension
	{
		/// <summary>
		/// Hierarchy 工具类
		/// </summary>
		public class Utility
		{
			/// <summary>
			/// 检测 GameObject 是否在 Hierarchy 窗口中展开
			/// </summary>
			public static bool IsExpanded(GameObject go) { return GetExpandedGameObjects().Contains(go); }

			/// <summary>
			/// 获得在Hierarchy 窗口中被展开的 GameObject 的列表
			/// </summary>
			public static List<GameObject> GetExpandedGameObjects()
			{
				var sceneHierarchy = GetSceneHierarchy();

				var methodInfo = sceneHierarchy
				                 .GetType()
				                 .GetMethod("GetExpandedGameObjects");

				var result = methodInfo?.Invoke(sceneHierarchy, Array.Empty<object>());

				return result as List<GameObject>;
			}

			/// <summary>
			/// 设置展开折叠
			/// </summary>
			public static void SetExpanded(GameObject go, bool expand)
			{
				var sceneHierarchy = GetSceneHierarchy();

				var methodInfo = sceneHierarchy.GetType()
				                               .GetMethod("ExpandTreeViewItem", BindingFlags.NonPublic | BindingFlags.Instance);

				methodInfo?.Invoke(sceneHierarchy, new object[] {go.GetInstanceID(), expand});
			}

			/// <summary>
			/// 递归设置展开折叠
			/// </summary>
			public static void SetExpandedRecursive(GameObject go, bool expand)
			{
				var sceneHierarchy = GetSceneHierarchy();

				var methodInfo = sceneHierarchy.GetType()
				                               .GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);

				methodInfo?.Invoke(sceneHierarchy, new object[] {go.GetInstanceID(), expand});
			}

			/// <summary>
			/// 获得 Hierarchy 窗口
			/// </summary>
			/// <returns></returns>
			private static object GetSceneHierarchy()
			{
				var window = GetHierarchyWindow();

				var sceneHierarchy = typeof(EditorWindow).Assembly
				                                         .GetType("UnityEditor.SceneHierarchyWindow")
				                                         .GetProperty("sceneHierarchy")
				                                         ?.GetValue(window);
				// BaseHierarchySort
				
				return sceneHierarchy;
			}

			/// <summary>
			/// 获取 Hierarchy 窗口
			/// </summary>
			/// <returns></returns>
			private static EditorWindow GetHierarchyWindow()
			{
				EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
				return EditorWindow.focusedWindow;
			}
		}
	}
}