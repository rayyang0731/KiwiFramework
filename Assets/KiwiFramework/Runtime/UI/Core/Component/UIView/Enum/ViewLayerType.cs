using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// UI 界面层级类型
	/// </summary>
	public enum ViewLayerType
	{
		/// <summary>
		/// 底层
		/// </summary>
		[InspectorName("底层")]
		Bottom = 0,

		/// <summary>
		/// 主界面层
		/// </summary>
		[InspectorName("主界面层")]
		Main,

		/// <summary>
		/// 一级界面层
		/// </summary>
		[InspectorName("一级界面层")]
		FirstPanel,

		/// <summary>
		/// 二级界面层
		/// </summary>
		[InspectorName("二级界面层")]
		SecondPanel,

		/// <summary>
		/// 三级界面层
		/// </summary>
		[InspectorName("三级界面层")]
		ThirdPanel,

		/// <summary>
		/// 弹窗界面层
		/// </summary>
		[InspectorName("弹窗界面层")]
		Popup,

		/// <summary>
		/// 引导界面层
		/// </summary>
		[InspectorName("引导界面层")]
		Guide,

		/// <summary>
		/// 加载界面层
		/// </summary>
		[InspectorName("加载界面层")]
		Loading,

		/// <summary>
		/// 系统弹窗界面层
		/// </summary>
		[InspectorName("系统界面层")]
		System,

		/// <summary>
		/// 顶级界面层
		/// </summary>
		[InspectorName("顶级界面层")]
		Top,
	}
}