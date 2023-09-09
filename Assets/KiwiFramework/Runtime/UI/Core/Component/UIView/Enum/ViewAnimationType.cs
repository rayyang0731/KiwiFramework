using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 界面动画类型
	/// </summary>
	public enum ViewAnimationType
	{
		/// <summary>
		/// 无动画
		/// </summary>
		[InspectorName("无动画")]
		None = 1,

		/// <summary>
		/// 淡入淡出
		/// </summary>
		[InspectorName("淡入淡出")]
		Fade,

		/// <summary>
		/// 自定义
		/// </summary>
		[InspectorName("自定义")]
		Custom
	}
}