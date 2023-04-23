using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 界面销毁类型
	/// </summary>
	public enum ViewDestroyType
	{
		/// <summary>
		/// 立即删除
		/// </summary>
		[InspectorName("立即删除")]
		ImmediateDestroy = 1,

		/// <summary>
		/// 延迟删除
		/// </summary>
		[InspectorName("延迟删除")]
		DelayDestroy,

		/// <summary>
		/// 常驻(不删除)
		/// </summary>
		[InspectorName("常驻")]
		Permanent,
	}
}