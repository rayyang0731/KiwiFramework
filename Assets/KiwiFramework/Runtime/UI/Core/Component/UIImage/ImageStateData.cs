using System;

using Sirenix.OdinInspector;

using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 图片状态数据
	/// </summary>
	[Serializable]
	public class ImageStateData
	{
		/// <summary>
		/// 状态名称
		/// </summary>
		[LabelText("状态名称"), VerticalGroup("Data/Label")]
		[PropertyTooltip("仅为了在Editor中备注状态名称,代码中无实际调用")]
		public string name;

		/// <summary>
		/// 状态所用图片名称
		/// </summary>
		[LabelText("Sprite名称"), DisplayAsString, VerticalGroup("Data/Label")]
		public string spriteName;

#if UNITY_EDITOR
		[SerializeField, DisplayAsString, VerticalGroup("Data/Label")]
		internal string spriteGUID;

		/// <summary>
		/// 预览图
		/// </summary>
		[NonSerialized, HideLabel, PreviewField, HorizontalGroup("Data", Width = 60),
		 OnInspectorInit(nameof(OnSpriteInspectorInit)), OnValueChanged(nameof(OnSpriteChanged))]
		internal Sprite sprite;

		/// <summary>
		/// 当 Sprite 字段初始化时调用
		/// </summary>
		private void OnSpriteInspectorInit()
		{
			if (string.IsNullOrEmpty(spriteGUID))
				return;

			sprite = AssetLoader.Editor.LoadByGUID<Sprite>(spriteGUID);
		}

		/// <summary>
		/// 当 Sprite 字段发生改变时调用
		/// </summary>
		private void OnSpriteChanged()
		{
			if (sprite == null)
			{
				spriteName = string.Empty;
				spriteGUID = string.Empty;
				return;
			}

			spriteName = sprite.name;
			spriteGUID = AssetLoader.Editor.GetGUID(sprite);
		}
#endif

		/// <summary>
		/// 是否重载颜色
		/// </summary>
		[LabelText("重载颜色"), PropertyTooltip("Override color"), HorizontalGroup("Data/Label/Color")]
		public bool overrideColor;

		/// <summary>
		/// 状态所用颜色
		/// </summary>
		[HideLabel, ShowIf(nameof(overrideColor)), HorizontalGroup("Data/Label/Color")]
		public Color color;

		/// <summary>
		/// 是否自动设置为图片原大小
		/// </summary>
		[LabelText("原图大小"), PropertyTooltip("Native size"), VerticalGroup("Data/Label")]
		public bool autoUseNativeSize;

		/// <summary>
		/// 是否重载尺寸
		/// </summary>
		[LabelText("重载尺寸"), PropertyTooltip("Override size"), HorizontalGroup("Data/Label/Size"),
		 DisableIf(nameof(autoUseNativeSize)),]
		public bool overrideSize;

		/// <summary>
		/// Sprite的显示尺寸
		/// </summary>
		[HideLabel, LabelWidth(60), HorizontalGroup("Data/Label/Size"), ShowIf(nameof(overrideSize))]
		public Vector2 size;

		/// <summary>
		/// 默认数据
		/// </summary>
		public static ImageStateData Default =>
			new()
			{
				name              = "State Name",
				spriteName        = string.Empty,
				overrideColor     = false,
				color             = Color.white,
				autoUseNativeSize = false,
				overrideSize      = false,
				size              = new Vector2(100, 100)
			};
	}
}