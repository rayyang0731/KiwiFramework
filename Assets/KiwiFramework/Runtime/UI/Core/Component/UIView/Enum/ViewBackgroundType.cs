using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 界面背景类型
	/// </summary>
	public enum ViewBackgroundType
	{
		/// <summary>
		/// 无背景或自身背景
		/// </summary>
		[InspectorName("无背景")]
		None = 1,

		/// <summary>
		/// 模糊背景
		/// </summary>
		[InspectorName("模糊背景")]
		Blur,

		/// <summary>
		/// 半透明背景
		/// </summary>
		[InspectorName("半透明背景")]
		SubTransparent,

		/// <summary>
		/// 透明背景
		/// </summary>
		[InspectorName("透明背景")]
		Transparent
	}
}