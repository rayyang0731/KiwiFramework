using MonKey;

using UnityEditor;

namespace KiwiFramework.Editor
{
	internal partial class HierarchyExtension
	{
		[Command("Hierarchy 中 Raycast 开关")]
		public static void ShowRaycastToggle(bool value)
		{
			Setting.ShowRaycastToggle = value;
			EditorPrefs.SetBool(Setting.HierarchyShowRaycastToggle, value);
			EditorApplication.RepaintHierarchyWindow();
		}
		
		[Command("Hierarchy 中组件标识")]
		public static void ShowCompSign(bool value)
		{
			Setting.ShowCompSign = value;
			EditorPrefs.SetBool(Setting.HierarchyShowComponentSign, value);
			EditorApplication.RepaintHierarchyWindow();
		}

		public static class Setting
		{
			/// <summary>
			/// 显示组件标记
			/// </summary>
			internal static bool ShowCompSign = false;

			/// <summary>
			/// 显示 Raycast 开关
			/// </summary>
			internal static bool ShowRaycastToggle = false;

			public const string HierarchyShowComponentSign = "Hierarchy_Show_Component_Sign";
			public const string HierarchyShowRaycastToggle = "Hierarchy_Show_Raycast_Toggle";

			static Setting()
			{
				ShowCompSign      = EditorPrefs.GetBool(HierarchyShowComponentSign, false);
				ShowRaycastToggle = EditorPrefs.GetBool(HierarchyShowRaycastToggle, false);
			}
		}
	}
}