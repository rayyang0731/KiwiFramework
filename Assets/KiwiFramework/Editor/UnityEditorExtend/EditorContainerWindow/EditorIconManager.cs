using System;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace GAEA.Editor
{
	public class EditorIconManager
	{
		public enum LabelIcon
		{
			Gray,
			Blue,
			Teal,
			Green,
			Yellow,
			Orange,
			Red,
			Purple
		}
		
		public enum ShapeIcon
		{
			CircleGray,
			CircleBlue,
			CircleTeal,
			CircleGreen,
			CircleYellow,
			CircleOrange,
			CircleRed,
			CirclePurple,
			DiamondGray,
			DiamondBlue,
			DiamondTeal,
			DiamondGreen,
			DiamondYellow,
			DiamondOrange,
			DiamondRed,
			DiamondPurple
		}
		
		private static MethodInfo setIconForObjectMethodInfo;

		public static void SetIcon(GameObject gameObject, LabelIcon labelIcon)
		{
			SetIcon(gameObject, $"sv_label_{(int) labelIcon}");
		}

		public static void SetIcon(GameObject gameObject, ShapeIcon shapeIcon)
		{
			SetIcon(gameObject, $"sv_icon_dot{(int) shapeIcon}_pix16_gizmo");
		}

		private static void SetIcon(GameObject gameObject, string contentName)
		{
			GUIContent iconContent = EditorGUIUtility.IconContent(contentName);
			SetIconForObject(gameObject, (Texture2D) iconContent.image);
		}

		public static void RemoveIcon(GameObject gameObject)
		{
			SetIconForObject(gameObject, null);
		}

		public static void SetIconForObject(GameObject obj, Texture2D icon)
		{
			if (setIconForObjectMethodInfo == null)
			{
				Type type = typeof(EditorGUIUtility);
				setIconForObjectMethodInfo =
					type.GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);
			}

			setIconForObjectMethodInfo.Invoke(null, new object[] {obj, icon});
		}
	}
}