using UnityEditor;

using UnityEngine;

namespace KiwiFramework.Editor
{
	/// <summary>
	/// Unity Hierarchy 窗口扩展
	/// </summary>
	[InitializeOnLoad]
	internal partial class HierarchyExtension
	{
		static HierarchyExtension() { EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemGUI; }

		private static Rect GetRect(Rect selectionRect, int width, int lastWidth)
		{
			var rect = new Rect(selectionRect);
			rect.x     += rect.width - (width + lastWidth);
			rect.width =  width;
			return rect;
		}

		private static void OnHierarchyWindowItemGUI(int instanceID, Rect selectionRect)
		{
			var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

			if (go == null) return;

			var lastWidth = 0;

			HierarchyRaycastSign.Draw(selectionRect, go, ref lastWidth);

			UIBatchDepthAnalyze.Draw(selectionRect, go, ref lastWidth);
		}
	}
}