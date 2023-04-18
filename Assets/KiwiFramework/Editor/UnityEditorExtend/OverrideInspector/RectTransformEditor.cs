using KiwiFramework.Runtime;
using KiwiFramework.Runtime.UnityExtend;

using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

using UnityEditor;

using UnityEngine;

namespace KiwiFramework.Editor
{
	/// <summary>
	/// 扩展 RectTransform 编辑器面板,添加重置位置,旋转,缩放的按钮和位置,宽高取整的按钮
	/// </summary>
	[CustomEditor(typeof(RectTransform), true)]
	public class RectTransformEditor : DecoratorEditor
	{
		public RectTransformEditor() : base("RectTransformEditor") { }

		private static bool _otherPropertyFoldOut;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space();

			var rt = target as RectTransform;

			_otherPropertyFoldOut = EditorGUILayout.Foldout(_otherPropertyFoldOut, "Other Property");
			if (_otherPropertyFoldOut)
			{
				Undo.RecordObject(target, "RectTransform Extra Property");
				if (!ReferenceEquals(rt, null))
				{
					GUIHelper.PushIndentLevel(1);

					rt.position           = SirenixEditorFields.Vector3Field("世界坐标", rt.position);
					rt.localPosition      = SirenixEditorFields.Vector3Field("相对坐标", rt.localPosition);
					rt.anchoredPosition   = SirenixEditorFields.Vector2Field("Anchored Position", rt.anchoredPosition);
					rt.anchoredPosition3D = SirenixEditorFields.Vector3Field("Anchored Position 3D", rt.anchoredPosition3D);
					rt.rotation           = SirenixEditorFields.QuaternionField("Rotation(Quaternion)", rt.rotation);
					rt.SetSize(SirenixEditorFields.Vector2Field("尺寸", rt.GetSize()));

					GUIHelper.PopIndentLevel();
				}
			}

			SirenixEditorGUI.BeginIndentedHorizontal("Box");
			GUILayout.Label(EditorIcons.Refresh.Active, GUILayoutOptions.Width(18).Height(18));
			ResetPos();
			ResetRot();
			ResetScale();
			ResetAll();
			SirenixEditorGUI.EndIndentedHorizontal();
			Rounding();
		}

		private void ResetPos()
		{
			GUIHelper.PushColor(Color.cyan);
			if (!GUILayout.Button("重置 AnchoredPos")) return;
			var rt                                = target as RectTransform;
			if (rt != null) rt.anchoredPosition3D = Vector3.zero;
			GUIHelper.PopColor();
		}

		private void ResetRot()
		{
			GUIHelper.PushColor(Color.green);
			if (!GUILayout.Button("重置 Rotation")) return;
			var rt                              = target as RectTransform;
			if (rt != null) rt.localEulerAngles = Vector3.zero;
			GUIHelper.PopColor();
		}

		private void ResetScale()
		{
			GUIHelper.PushColor(Color.yellow);
			if (!GUILayout.Button("重置 Scale")) return;
			var rt                        = target as RectTransform;
			if (rt != null) rt.localScale = Vector3.one;
			GUIHelper.PopColor();
		}

		private void ResetAll()
		{
			GUIHelper.PushColor(Color.red);
			if (!GUILayout.Button("All")) return;
			var rt = target as RectTransform;
			if (rt != null)
			{
				rt.anchoredPosition3D = Vector3.zero;
				rt.localEulerAngles   = Vector3.zero;
				rt.localScale         = Vector3.one;
			}

			GUIHelper.PopColor();
		}

		private void Rounding()
		{
			GUIHelper.PushColor(new Color(1, 0.5f, 0f));
			if (!GUILayout.Button("全部取整")) return;
			var rt = target as RectTransform;
			if (rt != null)
			{
				var pos = Vector3Int.RoundToInt(rt.anchoredPosition3D);
				rt.anchoredPosition3D = pos;
				var size = Vector2Int.RoundToInt(rt.rect.size);
				rt.SetSize(size);
			}

			GUIHelper.PopColor();
		}
	}
}