using KiwiFramework.Runtime.UnityExtend;

using Sirenix.OdinInspector;

using UnityEditor.Animations;

using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	[HideMonoScript]
	[RequireComponent(typeof(CanvasGroup)),
	 AddComponentMenu("KiwiUI/UIView")]
	public sealed class UIView : UIContainer
	{
		public override bool ForceCallInCode => true;

		#region 基础组件

		[ShowInInspector, BoxGroup("base", LabelText = "基础属性", CenterLabel = true, Order = int.MinValue), HideIf("@canvas == null"), ReadOnly]
		public Canvas canvas;

		[ShowInInspector, BoxGroup("base"), ReadOnly]
		public CanvasGroup canvasGroup;

		#endregion

		#region View Config

		/// <summary>
		/// 界面层级
		/// </summary>
		[SerializeField, LabelText("界面层级"), BoxGroup("setting", LabelText = "设置", CenterLabel = true, Order = int.MinValue + 1),
		 SuffixLabel(SdfIconType.QuestionCircleFill),
		 PropertyTooltip("[ 底层 ] - 主界面之下要显示的内容\n\n" +
		                 "[ 主界面层 ] - 主界面所在层\n\n" +
		                 "[ 一级界面层 ] - 大部分全屏界面或者由主界面打开的界面所在层\n\n" +
		                 "[ 二级界面层 ] - 大部分半屏界面或者一级界面的子界面所在层\n\n" +
		                 "[ 三级界面层 ] - 大部分弹窗型小界面或者二级界面的子界面所在层\n\n" +
		                 "[ 弹窗界面层 ] - 大部分游戏逻辑的全局弹窗界面所在层\n\n" +
		                 "[ 引导界面层 ] - 游戏引导界面所在层\n\n" +
		                 "[ 加载界面层 ] - 游戏加载界面所在层\n\n" +
		                 "[ 系统界面层 ] - 系统级弹窗或者系统级界面所在层\n\n" +
		                 "[ 顶级界面层 ] - UI 界面的最高层级")]
		private ViewLayerType layer = ViewLayerType.Main;

		/// <summary>
		/// 界面模式
		/// </summary>
		[SerializeField, LabelText("界面模式"), BoxGroup("setting"), SuffixLabel(SdfIconType.QuestionCircleFill),
		 PropertyTooltip("[ 普通 ] - 单纯显示界面,界面不会进入界面回退栈.\n\n" +
		                 "[ 回退 ] - 打开的界面会进入界面回退栈,界面关闭会回退打开回退栈中栈顶的界面.\n\n" +
		                 "[ Solo ] - 独占显示界面,当界面打开会隐藏其他界面.")]
		private ViewMode mode = ViewMode.Normal;

		/// <summary>
		/// 是否为全屏界面
		/// </summary>
		[SerializeField, LabelText("全屏界面"), BoxGroup("setting"), SuffixLabel(SdfIconType.QuestionCircleFill),
		 PropertyTooltip("如果勾选,则界面打开时会暂时关闭摄像机")]
		private bool isFullScreen = true;

		/// <summary>
		/// 界面背景类型
		/// </summary>
		[SerializeField, LabelText("背景样式"), BoxGroup("setting"), SuffixLabel(SdfIconType.QuestionCircleFill),
		 PropertyTooltip("[ 无背景 ] - 界面不需要全屏背景,或者界面拥有自己的全屏背景\n\n" +
		                 "[ 模糊背景 ] - 界面打开时,添加一个有模糊效果的全屏背景.\n\n" +
		                 "[ 半透明背景 ] - 界面打开时,添加一个指定颜色的半透明全屏背景.\n\n" +
		                 "[ 透明背景 ] - 界面打开时,添加一个全透明全屏背景.\n\n")]
		private ViewBackgroundType background = ViewBackgroundType.None;

		/// <summary>
		/// 点击界面外关闭
		/// </summary>
		[SerializeField, Indent, LabelText("点击背景关闭界面"), BoxGroup("setting"),
		 HideIf(nameof(background), ViewBackgroundType.None)]
		private bool clickBackgroundClose;

		/// <summary>
		/// 界面销毁类型
		/// </summary>
		[SerializeField, Space, LabelText("销毁类型"), BoxGroup("setting"), SuffixLabel(SdfIconType.QuestionCircleFill),
		 PropertyTooltip("[ 立即删除 ] - 界面调用关闭时,立即删除界面对象\n\n" +
		                 "[ 延迟删除 ] - 界面调用关闭时,会先隐藏界面,转入缓存区,等待延迟时间结束,删除界面对象.\n\n" +
		                 "[ 常驻 ] - 界面调用关闭时,只会隐藏界面,永远不会被删除.\n\n")]
		private ViewDestroyType destroyType = ViewDestroyType.ImmediateDestroy;

		/// <summary>
		/// 延迟销毁时间
		/// </summary>
		[SerializeField, Indent, LabelText("延迟销毁时间"), BoxGroup("setting"),
		 ShowIf(nameof(destroyType), ViewDestroyType.DelayDestroy), SuffixLabel("秒", true)]
		private float delayDestroyTime = 15;

		/// <summary>
		/// 清空回退栈
		/// </summary>
		[SerializeField, Space, LabelText("清空回退栈"), BoxGroup("setting"), SuffixLabel(SdfIconType.QuestionCircleFill),
		 PropertyTooltip("如果勾选,则当界面打开时,会清空界面回退栈.")]
		private bool clearBackspaceStack;

		/// <summary>
		/// 屏幕外初始化
		/// </summary>
		[SerializeField, LabelText("屏幕外初始化"), BoxGroup("setting"), SuffixLabel(SdfIconType.QuestionCircleFill),
		 PropertyTooltip("创建界面时,是否在画面外进行实例化,下一帧再把界面放到原本的位置,可以防止出现Sprite后绑定出现白图的情况.")]
		private bool outOfScreenInit;

		/// <summary>
		/// 强制显示(忽略隐藏其他界面方法)
		/// </summary>
		[SerializeField, LabelText("强制显示"), BoxGroup("setting"), SuffixLabel(SdfIconType.QuestionCircleFill),
		 PropertyTooltip("如果勾选,即便打开 Solo 模式的界面或者代码中调用隐藏其他界面方法,也不会被隐藏.")]
		private bool holdShow;

		/// <summary>
		/// 是否是动态界面(是否会频繁重构的界面)
		/// </summary>
		[SerializeField, HideInInspector]
		private bool _isDynamicView;

		[ShowInInspector, LabelText("动态界面", true), BoxGroup("setting")]
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
		/// 界面动画类型
		/// </summary>
		[SerializeField, Space, LabelText("动画类型"), BoxGroup("setting"), SuffixLabel(SdfIconType.QuestionCircleFill),
		 PropertyTooltip("[ 无动画 ] - 不播放打开关闭动画.\n\n" +
		                 "[ 淡入淡出 ] - 以淡入方式播放打开动画,淡出方式播放关闭动画\n\n" +
		                 "[ 自定义 ] - 以 Animator 方式播放打开关闭动画"),
		 OnValueChanged(nameof(OnAnimationTypeChanged))]
		private ViewAnimationType animationType = ViewAnimationType.None;

		private void OnAnimationTypeChanged()
		{
			switch (animationType)
			{
				case ViewAnimationType.None:
				case ViewAnimationType.Fade:
					if (animator)
						DestroyImmediate(animator);
					break;
				case ViewAnimationType.Custom:
					animator = this.ForceGetComponent<Animator>();
					break;
			}
		}

		/// <summary>
		/// 打开淡入动画时间
		/// </summary>
		[SerializeField, Indent]
		[LabelText("打开淡入动画时间"), SuffixLabel("秒", true), BoxGroup("setting/淡入淡出动画", false, HideWhenChildrenAreInvisible = true),
		 ShowIf(nameof(animationType), ViewAnimationType.Fade)]
		private float openFadeInDuration = 0.5f;

		/// <summary>
		/// 关闭淡出动画时间
		/// </summary>
		[SerializeField, Indent]
		[LabelText("关闭淡出动画时间"), SuffixLabel("秒", true), BoxGroup("setting/淡入淡出动画"),
		 ShowIf(nameof(animationType), ViewAnimationType.Fade)]
		private float closeFadeOutDuration = 0.5f;

		/// <summary>
		/// 打开动画
		/// </summary>
		[SerializeField, Indent]
		[LabelText("打开动画"), BoxGroup("setting/自定义动画", false, HideWhenChildrenAreInvisible = true),
		 ShowIf(nameof(animationType), ViewAnimationType.Custom), ValueDropdown(nameof(GetAnimationList))]
		private string openCustomAnimation;

		/// <summary>
		/// 关闭动画
		/// </summary>
		[SerializeField, Indent]
		[LabelText("关闭动画"), BoxGroup("setting/自定义动画"),
		 ShowIf(nameof(animationType), ViewAnimationType.Custom), ValueDropdown(nameof(GetAnimationList))]
		private string closeCustomAnimation;

		[SerializeField, Indent]
		[BoxGroup("setting/自定义动画"), ReadOnly,
		 ShowIf(nameof(animationType), ViewAnimationType.Custom)]
		private Animator animator;

		private ValueDropdownList<string> GetAnimationList()
		{
			ValueDropdownList<string> list = new();

			if (animator == null)
				return list;

			var controller = animator.runtimeAnimatorController as AnimatorController;
			if (controller == null)
				return list;

			var stateMachine = controller.layers[0].stateMachine;
			foreach (var state in stateMachine.states)
			{
				list.Add(state.state.name);
			}

			return list;
		}

		/// <summary>
		/// 界面打开默认音效
		/// </summary>
		private const string CONST_DEFAULT_OPEN_SFX_KEY = "CommonViewOpen";

		/// <summary>
		/// 打开音效
		/// </summary>
		[SerializeField]
		[LabelText("打开音效"), FoldoutGroup("音效"), InlineButton("ResetOpenSFX", "重置")]
		private string openSFX = CONST_DEFAULT_OPEN_SFX_KEY;

		/// <summary>
		/// 界面关闭默认音效
		/// </summary>
		private const string CONST_DEFAULT_CLOSE_SFX_KEY = "CommonViewClose";

		/// <summary>
		/// 关闭音效
		/// </summary>
		[SerializeField]
		[LabelText("关闭音效"), FoldoutGroup("音效"), InlineButton("ResetCloseSFX", "重置")]
		private string closeSFX = CONST_DEFAULT_CLOSE_SFX_KEY;

		#endregion

		/// <summary>
		/// 界面层级
		/// </summary>
		public ViewLayerType Layer => layer;

		/// <summary>
		/// 界面模式
		/// </summary>
		public ViewMode Mode => mode;

		/// <summary>
		/// 是否为全屏界面
		/// </summary>
		public bool IsFullScreen => isFullScreen;

		/// <summary>
		/// 界面背景类型
		/// </summary>
		public ViewBackgroundType Background => background;

		/// <summary>
		/// 点击界面外关闭
		/// </summary>
		public bool ClickBackgroundClose => clickBackgroundClose;

		/// <summary>
		/// 界面销毁类型
		/// </summary>
		public ViewDestroyType DestroyType => destroyType;

		/// <summary>
		/// 延迟销毁时间
		/// </summary>
		public float DelayDestroyTime => delayDestroyTime;

		/// <summary>
		/// 点击界面外关闭
		/// </summary>
		public bool ClearBackspaceStack => clearBackspaceStack;

		/// <summary>
		/// 屏幕外初始化
		/// </summary>
		public bool OutOfScreenInit => outOfScreenInit;

		/// <summary>
		/// 强制显示(忽略隐藏其他界面方法)
		/// </summary>
		public bool HoldShow => holdShow;

		/// <summary>
		/// 界面动画类型
		/// </summary>
		public ViewAnimationType AnimationType => animationType;

		/// <summary>
		/// 打开淡入动画时间
		/// </summary>
		public float OpenFadeInDuration => openFadeInDuration;

		/// <summary>
		/// 关闭淡出动画时间
		/// </summary>
		public float CloseFadeOutDuration => closeFadeOutDuration;

		/// <summary>
		/// 打开动画
		/// </summary>
		public string OpenCustomAnimation => openCustomAnimation;

		/// <summary>
		/// 关闭动画
		/// </summary>
		public string CloseCustomAnimation => closeCustomAnimation;

		/// <summary>
		/// 界面动画组件
		/// </summary>
		public Animator Animator => animator;

		/// <summary>
		/// 打开音效
		/// </summary>
		public string OpenSFX
		{
			get => openSFX;
			set
			{
				if (string.IsNullOrEmpty(value) && openSFX == value)
					return;

				openSFX = value;
			}
		}

		/// <summary>
		/// 关闭音效
		/// </summary>
		public string CloseSFX
		{
			get => closeSFX;
			set
			{
				if (string.IsNullOrEmpty(value) && closeSFX == value)
					return;

				closeSFX = value;
			}
		}

		/// <summary>
		/// 重置打开音效
		/// </summary>
		private void ResetOpenSFX() => openSFX = CONST_DEFAULT_OPEN_SFX_KEY;

		/// <summary>
		/// 重置关闭音效
		/// </summary>
		private void ResetCloseSFX() => closeSFX = CONST_DEFAULT_CLOSE_SFX_KEY;

#if UNITY_EDITOR
		protected override void Reset()
		{
			base.Reset();

			canvasGroup = GetComponent<CanvasGroup>();
		}
#endif
	}
}