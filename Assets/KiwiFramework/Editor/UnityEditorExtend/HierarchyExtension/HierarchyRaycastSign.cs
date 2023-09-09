using UnityEditor;

using UnityEngine;
using UnityEngine.UI;

namespace KiwiFramework.Editor
{
	internal partial class HierarchyExtension
	{
		/// <summary>
		/// 绘制 Raycast 开关
		/// </summary>
		internal static class HierarchyRaycastSign
		{
			/// <summary>
			/// 为 UI 绘制扩展
			/// </summary>
			public static void Draw(Rect selectionRect, GameObject go, ref int lastWidth)
			{
				if (!KiwiFramework.Editor.HierarchyExtension.Setting.ShowRaycastToggle) return;
				if (go.layer != LayerMask.NameToLayer("UI")) return;

				var graphic = go.GetComponent<Graphic>();
				if (graphic != null)
				{
					selectionRect =  GetRect(selectionRect, 10, lastWidth);
					lastWidth     += 10;

					var last = graphic.raycastTarget;
					graphic.raycastTarget = GUI.Toggle(selectionRect, graphic.raycastTarget, string.Empty);
					
					if (last != graphic.raycastTarget)
						EditorUtility.SetDirty(graphic);
				}
				else
				{
					lastWidth += 10;
				}

				lastWidth += 2;
			}
		}
	}
}