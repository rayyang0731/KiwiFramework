using System;

using Sirenix.OdinInspector;

using TMPro;

using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 文本状态数据
	/// </summary>
	[Serializable]
	public class LabelStateData
	{
		/// <summary>
		/// 状态名称
		/// </summary>
		[LabelText("状态名称"), LabelWidth(60), HorizontalGroup("Data"),
		 VerticalGroup("Data/Label"), PropertyTooltip("仅为了在Editor中备注状态名称,代码中无实际调用")]
		public string name;

		[LabelText("自定义文本"), HorizontalGroup("Data/Label/use_customText"), OnValueChanged(nameof(OnUseCustomTextChanged))]
		public bool use_customText;

		[HideLabel, HorizontalGroup("Data/Label/use_customText"), ShowIf(nameof(use_customText))]
		public string customText;

		[LabelText("使用i18n"), LabelWidth(60),
		 ToggleGroup("Data/Label/use_i18n", "国际化配置"),
		 OnValueChanged(nameof(OnUseI18NChanged))]
		public bool use_i18n;

		[LabelText("配置表 Key"),
		 ValueDropdown(nameof(GetTableKeys), FlattenTreeView = true, AppendNextDrawer = true),
		 ToggleGroup("Data/Label/use_i18n")]
		public string tableID;

		[LabelText("重载字体"), LabelWidth(60), HorizontalGroup("Data/Label/Font")]
		public bool overrideFont;

		/// <summary>
		/// 状态所用图片
		/// </summary>
		[HideLabel, HorizontalGroup("Data/Label/Font"), ShowIf(nameof(overrideFont))]
		public TMP_FontAsset font;

		/// <summary>
		/// 是否重载颜色
		/// </summary>
		[LabelText("重载颜色"), LabelWidth(60), HorizontalGroup("Data/Label/Color")]
		public bool overrideColor;

		/// <summary>
		/// 状态所用颜色
		/// </summary>
		[HideLabel, ShowIf(nameof(overrideColor)), HorizontalGroup("Data/Label/Color"),
		 PropertySpace(0, 20)]
		public Color color;

#if UNITY_EDITOR
		private void OnUseCustomTextChanged()
		{
			if (use_customText)
				use_i18n = false;
		}

		private void OnUseI18NChanged()
		{
			if (use_i18n)
				use_customText = false;
		}

		/// <summary>
		/// 获取指定多语言表中的全部 Key
		/// </summary>
		/// <returns></returns>
		internal ValueDropdownList<string> GetTableKeys() { return I18NSettings.GetKey2Values(); }
#endif

		/// <summary>
		/// 默认数据
		/// </summary>
		public static LabelStateData Default =>
			new()
			{
				name          = "State Name",
				use_i18n      = false,
				tableID       = string.Empty,
				overrideFont  = false,
				font          = null,
				overrideColor = false,
				color         = Color.black
			};
	}
}