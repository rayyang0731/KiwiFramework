// using System;
//
// using KiwiFramework.Runtime;
//
// using Sirenix.OdinInspector;
//
// using UnityEngine;
//
// namespace KiwiFramework.UI
// {
// 	/// <summary>
// 	/// 图片状态数据
// 	/// </summary>
// 	[Serializable]
// 	public class RawImageStateData
// 	{
// 		/// <summary>
// 		/// 状态名称
// 		/// </summary>
// 		[LabelText("状态名称"), VerticalGroup("Data/Label")]
// 		[PropertyTooltip("仅为了在Editor中备注状态名称,代码中无实际调用")]
// 		public string name;
//
// 		/// <summary>
// 		/// 状态所用图片名称
// 		/// </summary>
// 		[LabelText("Texture 名称"), VerticalGroup("Data/Label"), DisplayAsString]
// 		public string textureName;
//
// #if UNITY_EDITOR
// 		[SerializeField, DisplayAsString]
// 		internal string textureGUID;
//
// 		/// <summary>
// 		/// 预览图
// 		/// </summary>
// 		[NonSerialized, HideLabel, PreviewField, HorizontalGroup("Data", Width = 60),
// 		 OnInspectorInit(nameof(OnTextureInspectorInit)), OnValueChanged(nameof(OnTextureChanged))]
// 		public Texture texture;
//
// 		/// <summary>
// 		/// 当 Texture 字段初始化时调用
// 		/// </summary>
// 		private void OnTextureInspectorInit()
// 		{
// 			if (string.IsNullOrEmpty(textureGUID))
// 				return;
//
// 			texture = KiwiAssets.Editor.LoadByGUID<Texture>(textureGUID);
// 		}
//
// 		/// <summary>
// 		/// 当 Texture 字段发生改变时调用
// 		/// </summary>
// 		private void OnTextureChanged()
// 		{
// 			if (texture == null)
// 			{
// 				textureName = string.Empty;
// 				textureGUID = string.Empty;
// 				return;
// 			}
//
// 			textureName = texture.name;
// 			textureGUID = KiwiAssets.Editor.GetGUID(texture);
// 		}
// #endif
//
// 		/// <summary>
// 		/// 是否重载颜色
// 		/// </summary>
// 		[LabelText("重载颜色"), PropertyTooltip("Override color"), HorizontalGroup("Color"), PropertySpace]
// 		public bool overrideColor;
//
// 		/// <summary>
// 		/// 状态所用颜色
// 		/// </summary>
// 		[HideLabel, ShowIf("overrideColor"), HorizontalGroup("Color"), PropertySpace]
// 		public Color color;
//
// 		/// <summary>
// 		/// 是否自动设置为图片原大小
// 		/// </summary>
// 		[LabelText("原图大小"), PropertyTooltip("Native size")]
// 		public bool autoUseNativeSize;
//
// 		/// <summary>
// 		/// 是否重载尺寸
// 		/// </summary>
// 		[LabelText("重载尺寸"), PropertyTooltip("Override size"),
// 		 HorizontalGroup("Size"), DisableIf("autoUseNativeSize"),]
// 		public bool overrideSize;
//
// 		/// <summary>
// 		/// Sprite的显示尺寸
// 		/// </summary>
// 		[HideLabel, LabelWidth(60),
// 		 HorizontalGroup("Size"), ShowIf("overrideSize")]
// 		public Vector2 size;
//
// 		/// <summary>
// 		/// 默认数据
// 		/// </summary>
// 		public static RawImageStateData Default =>
// 			new()
// 			{
// 				name              = "State Name",
// 				textureName       = string.Empty,
// 				overrideColor     = false,
// 				color             = Color.white,
// 				autoUseNativeSize = false,
// 				overrideSize      = false,
// 				size              = new Vector2(100, 100)
// 			};
// 	}
// }
