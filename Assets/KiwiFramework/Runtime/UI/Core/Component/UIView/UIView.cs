using KiwiFramework.Runtime.UnityExtend;

using Sirenix.OdinInspector;

using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	[HideMonoScript]
	[RequireComponent(typeof(CanvasGroup)),
	 AddComponentMenu("KiwiUI/UIView")]
	public sealed class UIView : UIContainer
	{
		/// <summary>
		/// 界面类型
		/// </summary>
		public enum VIEW_TYPE : byte
		{
			[LabelText("全屏界面")] FULL_SCREEN = 0,

			[LabelText("模态界面(透明遮罩)")] MODAL_TRANSPARENT = 1,

			[LabelText("模态界面(半透灰遮罩)")] MODAL_BLACK = 2
		}

		/// <summary>
		/// 界面逻辑类型
		/// </summary>
		public enum VIEW_LOGIC_TYPE : byte
		{
			[LabelText("可回退界面")] ROLLBACK = 0,
			[LabelText("普通界面")] NORMAL = 1,
		}

		#region 基础组件

		[ShowInInspector, BoxGroup("base", LabelText = "基础属性", CenterLabel = true, Order = int.MinValue), HideIf("@canvas == null"), ReadOnly]
		public Canvas canvas;

		[ShowInInspector, BoxGroup("base"), ReadOnly]
		public CanvasGroup canvasGroup;

		#endregion

		#region 设置

		[ShowInInspector, BoxGroup("设置", Order = -1, CenterLabel = true)]
		[LabelText("界面类型"), PropertyOrder(1)]
		[PropertyTooltip("选择[全屏界面]会关闭主游戏摄像机,反之则不会关闭.")]
		private VIEW_TYPE ViewType { get; set; } = VIEW_TYPE.FULL_SCREEN;

		[ShowInInspector, BoxGroup("设置")]
		[LabelText("是否可回退"), PropertyOrder(2)]
		public VIEW_LOGIC_TYPE ViewLogicType { get; set; } = VIEW_LOGIC_TYPE.ROLLBACK;

		[ShowInInspector, BoxGroup("设置")]
		[LabelText("动态界面", true), PropertyOrder(3)]
		public bool isDynamicView
		{
			get => _isDynamicView;
			set
			{
				if (_isDynamicView == value) return;
				_isDynamicView = value;
				if (value)
				{
					if (canvas == null)
						canvas = this.ForceGetComponent<Canvas>();
				}
				else
				{
					if (canvas == null) return;
#if UNITY_EDITOR
					DestroyImmediate(canvas, true);
#else
                    Destroy(canvas);
#endif
					canvas = null;
				}
			}
		}

		/// <summary>
		/// 是否是动态界面(是否会频繁重构的界面)
		/// </summary>
		[SerializeField, HideInInspector]
		private bool _isDynamicView;

		[ShowInInspector, BoxGroup("设置")]
		[LabelText("显示排序层级"), PropertyOrder(4), SuffixLabel("@GetSubCanvasName()"), PropertyRange(0, 300)]
		public int SortLayer
		{
			get => sortLayer;
			set => sortLayer = value;
		}

		[SerializeField, HideInInspector] private int sortLayer = 0;

		private string GetSubCanvasName()
		{
			// if (SortLayer >= ViewManager.TopCanvasSortOrder)
			// 	return "Top";
			// if (SortLayer >= ViewManager.ForwardCanvasSortOrder)
			return "Forward";
			return "Normal";
		}

		#endregion

		#region Tick 执行相关字段

		[BoxGroup("Tick", Order = -0.9f, CenterLabel = true)]

		#region 是否执行 Update 方法

		[ToggleGroup("ExecuteUpdate", "执行 Update 方法", GroupID = "Tick/Update"),
		 SerializeField]
		public bool ExecuteUpdate = true;

		[VerticalGroup("Tick/Update/Order"), LabelText("Update 执行顺序"), SerializeField]
		public int UpdateOrder = -1;

		#endregion

		#region 是否执行 LateUpdate 方法

		[ToggleGroup("ExecuteLateUpdate", "执行 LateUpdate 方法", GroupID = "Tick/LateUpdate"), SerializeField]
		public bool ExecuteLateUpdate = false;

		[VerticalGroup("Tick/LateUpdate/Order"), LabelText("LateUpdate 执行顺序"), SerializeField]
		public int LateUpdateOrder = -1;

		#endregion

		#region 是否执行 FixedUpdate 方法

		[ToggleGroup("ExecuteFixedUpdate", "执行 FixedUpdate 方法", GroupID = "Tick/FixedUpdate"), SerializeField]
		public bool ExecuteFixedUpdate = false;

		[VerticalGroup("Tick/FixedUpdate/Order"), LabelText("FixedUpdate 执行顺序"), SerializeField]
		public int FixedUpdateOrder = -1;

		#endregion

		#endregion

#if UNITY_EDITOR
		private void Reset()
		{
			if (canvasGroup == null)
				canvasGroup = GetComponent<CanvasGroup>();
		}
#endif
	}
}