using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 界面模式
	/// </summary>
	public enum ViewMode
	{
		/// <summary>
		/// 单纯显示界面(不支持界面回退操作)
		/// </summary>
		[InspectorName("普通")]
		Normal = 1,

		/// <summary>
		/// 支持界面回退操作
		/// </summary>
		[InspectorName("回退")]
		Backspace,

		/// <summary>
		/// 隐藏其他界面,单独显示本界面(不支持界面回退操作)
		/// </summary>
		[InspectorName("Solo")]
		Solo,
	}
}