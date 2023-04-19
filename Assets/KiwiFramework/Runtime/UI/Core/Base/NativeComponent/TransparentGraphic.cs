using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;

namespace KiwiFramework.Runtime.UI
{
	[RequireComponent(typeof(CanvasRenderer))]
	[HideMonoScript, AddComponentMenu("KiwiUI/Native/TransparentGraphic")]
	public class TransparentGraphic : Graphic
	{
#if UNITY_EDITOR
		/// <summary>
		/// 非运行状态下
		/// </summary>
		/// <returns>红色半透</returns>
		[SerializeField]
		protected Color _previewColor = new(1, 0, 0, 0.3f);
#endif
		/// <summary>
		/// 在编辑器非运行模式下显示的颜色,运行时,变为全透明
		/// </summary>
		/// <value></value>
		public override Color color
		{
			get
			{
#if UNITY_EDITOR
				if (!UnityEditor.EditorApplication.isPlaying)
					return _previewColor;
#endif
				return Color.clear;
			}
		}

		/// <summary>
		/// 射线碰撞只能为true,不可以关闭
		/// </summary>
		public sealed override bool raycastTarget
		{
			get => true;
			set { }
		}

		protected TransparentGraphic() { useLegacyMeshGeneration = false; }


		public override void SetMaterialDirty()
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
				base.SetMaterialDirty();
#endif
		}

		public override void SetVerticesDirty()
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
				base.SetVerticesDirty();
#endif
		}


		protected sealed override void OnPopulateMesh(VertexHelper vh)
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
			{
				var r = GetPixelAdjustedRect();
				var v = new Vector4(
					r.x + raycastPadding.x,
					r.y + raycastPadding.y,
					r.x + r.width - raycastPadding.z,
					r.y + r.height - raycastPadding.w);

				Color32 color32 = color;

				vh.Clear();

				vh.AddVert(new Vector3(v.x, v.y), color32, Vector2.zero);
				vh.AddVert(new Vector3(v.x, v.w), color32, Vector2.up);
				vh.AddVert(new Vector3(v.z, v.w), color32, Vector2.one);
				vh.AddVert(new Vector3(v.z, v.y), color32, Vector2.right);

				vh.AddTriangle(0, 1, 2);
				vh.AddTriangle(2, 3, 0);
			}
			else
			{
				vh.Clear();
			}
#else
            vh.Clear();
#endif
		}
	}
}
