using System;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 点击按钮组件
	/// </summary>
	[HideMonoScript, ExecuteInEditMode]
	[RequireComponent(typeof(Graphic)), RequireComponent(typeof(Button))]
	[AddComponentMenu("KiwiUI/UIButton")]
	public sealed partial class UIButton : UIElement,
	                                       IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
	                                       IPointerEnterHandler, IPointerExitHandler, ITick
	{
		public override bool ForceCallInCode => true;

		/// <summary>
		/// 按钮类型
		/// </summary>
		public enum BUTTON_TYPE : byte
		{
			/// <summary>
			/// 普通按钮
			/// </summary>
			[InspectorName("普通按钮")]
			NORMAL,

			/// <summary>
			/// 长按按钮
			/// </summary>
			[InspectorName("长按按钮")]
			LONG_PRESS,

			/// <summary>
			/// 重复响应按钮
			/// </summary>
			[InspectorName("重复响应按钮")]
			REPEAT,
		}

		/// <summary>
		/// 间隔时间类型
		/// </summary>
		private enum INTERVAL_TYPE : byte
		{
			/// <summary>
			/// 曲线类型
			/// </summary>
			[InspectorName("曲线类型")]
			CURVE,

			/// <summary>
			/// 固定时间
			/// </summary>
			[InspectorName("固定时间")]
			FIXED,
		}

		/// <summary>
		/// 点击类型
		/// </summary>
		public enum POINTER_TYPE : byte
		{
			/// <summary>
			/// 按下
			/// </summary>
			[InspectorName("按下")]
			DOWN,

			/// <summary>
			/// 抬起
			/// </summary>
			[InspectorName("抬起")]
			UP,

			/// <summary>
			/// 点击
			/// </summary>
			[InspectorName("点击")]
			CLICK,

			/// <summary>
			/// 重复响应
			/// </summary>
			[InspectorName("重复响应")]
			REPEAT_RESPONSE,
		}

		[Serializable]
		public class RepeatRespondEvent : UnityEvent<int>
		{
		}


		#region 可用状态

		/// <summary>
		/// 是否为可用状态
		/// </summary>
		[LabelText("是否可用"), ShowInInspector, PropertyOrder(-1)]
		public bool interactable
		{
			get => button.interactable;
			set
			{
				if (!canGray)
					button.interactable = value;
				else
					SetInteractable(value);
			}
		}

		/// <summary>
		/// 设置互动状态
		/// </summary>
		/// <param name="state">是否可以互动</param>
		private void SetInteractable(bool state)
		{
			if (!canGray)
				return;

			button.interactable = state || invalidCanClick;

			SetGray(!state);
		}

		#endregion

		#region Button 属性

		[SerializeField, HideInInspector] private Button _button;

		public Button button
		{
			get
			{
				if (_button == null)
					_button = GetComponent<Button>();
				return _button;
			}
		}

		#endregion

		#region Button Text 属性

		[SerializeField]
		[LabelText("按钮文字"), BoxGroup("可用对象", Order = 5, CenterLabel = true),
		 InfoBox("自动获取只保证在有限规则下可以获取到,获取不到则需要手动拖拽指定,并且只获取字段为null的,已经指定对象的不会执行获取操作.", SdfIconType.MegaphoneFill)]
		private UILabel _buttonText;

		/// <summary>
		/// 按钮文字
		/// </summary>
		public UILabel buttonText
		{
			get => _buttonText;
			set => _buttonText = value;
		}

		#endregion

		#region Button Image 属性

		[SerializeField]
		[LabelText("按钮图片"), BoxGroup("可用对象")]
		private UIImage _buttonImage;

		/// <summary>
		/// 按钮图片
		/// </summary>
		public UIImage buttonImage
		{
			get
			{
				if (_buttonImage == null)
					TryGetComponent(out _buttonImage);

				return _buttonImage;
			}
		}

		#endregion

		#region 按钮类型

		[SerializeField]
		[LabelText("按钮类型"), FoldoutGroup("按钮行为设置", Order = 5)]
		private BUTTON_TYPE _buttonType = BUTTON_TYPE.NORMAL;

		/// <summary>
		/// 设置按钮类型
		/// </summary>
		/// <param name="buttonType">要设置的按钮类型</param>
		public void SetButtonType(BUTTON_TYPE buttonType) => _buttonType = buttonType;

		/// <summary>
		/// 设置按钮类型
		/// </summary>
		/// <param name="buttonType">要设置的按钮类型</param>
		public void SetButtonType(int buttonType)
		{
			var type = (BUTTON_TYPE) buttonType;
			switch (type)
			{
				case BUTTON_TYPE.NORMAL:
				case BUTTON_TYPE.LONG_PRESS:
				case BUTTON_TYPE.REPEAT:
					_buttonType = type;
					break;
				default:
					Debug.LogError($"{this} 设置按钮类型失败,要设置的按钮类型的值越界 [{buttonType.ToString()}]");
					break;
			}
		}

		#region 长按按钮

		[Serializable]
		private class LongPressButton
		{
			[SerializeField]
			[LabelText("长按响应时长"), SuffixLabel("秒", true)]
			internal float longPressDuration = 2;

			[SerializeField]
			[LabelText("忽略Timescale")]
			internal bool ignoreTimescale = true;

			/// <summary>
			/// 按下的时间点
			/// </summary>
			internal float LastRespondTime;

			/// <summary>
			/// 响应事件
			/// </summary>
			[SerializeField]
			internal Button.ButtonClickedEvent _onLongPress = new();

			/// <summary>
			/// 已经被触发过
			/// </summary>
			internal bool IsTriggered;
		}

		[SerializeField]
		[FoldoutGroup("按钮行为设置"), HideLabel,
		 ShowIf(nameof(_buttonType), BUTTON_TYPE.LONG_PRESS)]
		private LongPressButton longPressButton = new();

		/// <summary>
		/// 长按响应事件
		/// </summary>
		public Button.ButtonClickedEvent onLongPress
		{
			get => longPressButton?._onLongPress;
			set
			{
				if (longPressButton != null)
					longPressButton._onLongPress = value;
			}
		}

		/// <summary>
		/// 设置长按响应时长
		/// </summary>
		/// <param name="val">要按住多长时间后响应事件</param>
		public void SetLongPressDuration(float val)
		{
			longPressButton                   ??= new LongPressButton();
			longPressButton.longPressDuration =   val;
		}

		/// <summary>
		/// 当长按按钮响应按钮按下
		/// </summary>
		private void OnLongPressPointerDown() { longPressButton.LastRespondTime = longPressButton.ignoreTimescale ? Time.realtimeSinceStartup : Time.time; }

		/// <summary>
		/// LongPress OnPointerClick 要执行的内容
		/// </summary>
		private void OnLongPressPointerClick() { longPressButton.IsTriggered = false; }

		/// <summary>
		/// LongPress OnDisable 要执行的内容
		/// </summary>
		private void OnLongPressDisable()
		{
			longPressButton.LastRespondTime = 0;
			longPressButton.IsTriggered     = false;
		}

		/// <summary>
		/// LongPress Update 要执行的内容
		/// </summary>
		private void OnLongPressUpdate()
		{
			if (!_isPointerDown || !_isPointerInside || longPressButton == null) return;

			var curTime = longPressButton.ignoreTimescale ? Time.realtimeSinceStartup : Time.time;

			var deltaTime = curTime - longPressButton.LastRespondTime;
			if (!(deltaTime >= longPressButton.longPressDuration)) return;

			_isPointerDown   = false;
			_isPointerInside = false;

			longPressButton.LastRespondTime = 0;

			if (useButtonScale)
				uiScaleHelper.Out();

			longPressButton._onLongPress?.Invoke();
			longPressButton.IsTriggered = true;
		}

		#endregion

		#region 重复响应按钮

		[Serializable]
		private class RepeatButton
		{
			/// <summary>
			/// 使用曲线控制响应间隔时间
			/// </summary>
			[SerializeField]
			[LabelText("间隔时间类型")]
			internal INTERVAL_TYPE _intervalType = INTERVAL_TYPE.CURVE;

			/// <summary>
			/// 响应间隔时间曲线
			/// </summary>
			[SerializeField]
			[LabelText("时间曲线"), BoxGroup("curve/Box"),
			 ShowIfGroup("curve", Condition = nameof(_intervalType), Value = INTERVAL_TYPE.CURVE),
			 InfoBox("Y轴的值代表间隔时间,X轴的值仅代表长度,不影响间隔时间的值.")]
			internal AnimationCurve _intervalCurve = new(new Keyframe(0, 0.5f), new Keyframe(1, 0.5f));

			/// <summary>
			/// 曲线拆分节点数量
			/// </summary>
			[SerializeField]
			[LabelText("时间节点数量"), BoxGroup("curve/Box", ShowLabel = false)]
			[MinValue(3)]
			[ShowIfGroup("curve", Condition = nameof(_intervalType), Value = INTERVAL_TYPE.CURVE)]
			internal int _CurveNodeCount = 5;

			/// <summary>
			/// 固定响应间隔时间
			/// </summary>
			[SerializeField]
			[LabelText("固定间隔时间"), BoxGroup("fixed/Box", ShowLabel = false), SuffixLabel("秒", true)]
			[ShowIfGroup("fixed", Condition = nameof(_intervalType), Value = INTERVAL_TYPE.FIXED)]
			internal float _fixedInterval = 0.2f;

			[SerializeField]
			[LabelText("忽略TimeScale")]
			internal bool ignoreTimeScale = true;

			/// <summary>
			/// 按下的时候立即执行
			/// </summary>
			[SerializeField]
			[LabelText("立即执行"), PropertyTooltip("按下按钮时,是否立即执行响应时间,如果不进行勾选,事件将会在第一个时间节点到达时调用")]
			internal bool _immediateDo = true;

			/// <summary>
			/// 当前调用次数
			/// </summary>
			internal int _curRepeatCount;

			/// <summary>
			/// 按下的时间点
			/// </summary>
			internal float _lastRespondTime;

			/// <summary>
			/// 响应事件
			/// </summary>
			[SerializeField]
			internal RepeatRespondEvent _onRepeatRespond = new();

			/// <summary>
			/// 间隔时间变化最大时长
			/// </summary>
			internal float TotalLength => _intervalCurve.keys[_intervalCurve.length - 1].time;

			/// <summary>
			/// 调用间隔时间
			/// </summary>
			internal float Interval => _intervalType == INTERVAL_TYPE.FIXED ? _fixedInterval : _intervalCurve.Evaluate((TotalLength / _CurveNodeCount) * _curRepeatCount);

			/// <summary>
			/// 已经被触发过
			/// </summary>
			internal bool IsTriggered;
		}

		[SerializeField]
		[FoldoutGroup("按钮行为设置"), HideLabel, ShowIf(nameof(_buttonType), BUTTON_TYPE.REPEAT)]
		private RepeatButton repeatButton = new();

		/// <summary>
		/// 重复响应按钮事件
		/// </summary>
		public RepeatRespondEvent onRepeatRespond
		{
			get => repeatButton?._onRepeatRespond;
			set
			{
				if (repeatButton != null)
					repeatButton._onRepeatRespond = value;
			}
		}

		/// <summary>
		/// 设置固定间隔时间(并且会自动把间隔时间改为使用固定间隔时间)
		/// </summary>
		/// <param name="val">间隔时间</param>
		public void SetFixInterval(float val)
		{
			repeatButton                ??= new RepeatButton();
			repeatButton._intervalType  =   INTERVAL_TYPE.FIXED;
			repeatButton._fixedInterval =   val;
		}

		/// <summary>
		/// 设置响应时间曲线
		/// </summary>
		/// <param name="keys">曲线上的时间点数组,请保持与values的个数一致</param>
		/// <param name="values">曲线上每个时间点的值得数组,请保持与keys的个数一致</param>
		/// <param name="count">变化阶段次数</param>
		public void SetCurveInterval(float[] keys, float[] values, int count)
		{
			if (keys is not {Length: > 0} || values is not {Length: > 0} || count <= 1)
			{
				Debug.LogError("要添加的曲线数据有问题,改用默认固定间隔时间");
				SetFixInterval(0.2f);
				return;
			}

			repeatButton                 ??= new RepeatButton();
			repeatButton._intervalType   =   INTERVAL_TYPE.CURVE;
			repeatButton._CurveNodeCount =   count;
			repeatButton._intervalCurve  =   new AnimationCurve();

			var minCount = Mathf.Min(keys.Length, values.Length);

			for (var i = 0; i < minCount; i++)
			{
				repeatButton._intervalCurve.AddKey(keys[i], values[i]);
			}
		}

		/// <summary>
		/// 设置响应时间曲线
		/// </summary>
		/// <param name="curve">响应曲线</param>
		/// <param name="count">变化阶段次数</param>
		public void SetCurveInterval(AnimationCurve curve, int count)
		{
			if (curve == null || curve.keys.Length <= 0 || count <= 1)
			{
				Debug.LogError("要添加的曲线数据有问题,改用默认固定间隔时间");
				SetFixInterval(0.2f);
				return;
			}

			repeatButton                 ??= new RepeatButton();
			repeatButton._intervalType   =   INTERVAL_TYPE.CURVE;
			repeatButton._CurveNodeCount =   count;
			repeatButton._intervalCurve  =   curve;
		}

		/// <summary>
		/// RepeatButton OnPointerDown 要执行的内容
		/// </summary>
		private void OnRepeatButtonPointerDown()
		{
			repeatButton._lastRespondTime = repeatButton.ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;
			repeatButton._curRepeatCount  = 0;

			if (!repeatButton._immediateDo || repeatButton._onRepeatRespond == null) return;

			repeatButton._onRepeatRespond.Invoke(repeatButton._curRepeatCount);
			if (usePlaySound)
				soundHelper.Play(POINTER_TYPE.REPEAT_RESPONSE);
		}

		/// <summary>
		/// RepeatButton OnPointerUp 要执行的内容
		/// </summary>
		private void OnRepeatButtonPointerClick() { repeatButton.IsTriggered = false; }

		/// <summary>
		/// RepeatButton OnDisable 要执行的内容
		/// </summary>
		private void OnRepeatButtonDisable()
		{
			repeatButton._lastRespondTime = 0;
			repeatButton.IsTriggered      = false;
		}

		/// <summary>
		/// RepeatButton Update 要执行的内容
		/// </summary>
		private void OnRepeatButtonUpdate()
		{
			if (!_isPointerDown || !_isPointerInside) return;

			var curTime = repeatButton.ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;

			var deltaTime = curTime - repeatButton._lastRespondTime;

			if (!(deltaTime >= repeatButton.Interval)) return;

			repeatButton._lastRespondTime = repeatButton.ignoreTimeScale ? Time.realtimeSinceStartup : Time.time;

			if (repeatButton._intervalType == INTERVAL_TYPE.FIXED)
				repeatButton._curRepeatCount = ++repeatButton._curRepeatCount;
			else
				repeatButton._curRepeatCount = Mathf.Min(++repeatButton._curRepeatCount, repeatButton._CurveNodeCount);

			if (repeatButton._onRepeatRespond == null) return;

			repeatButton._onRepeatRespond.Invoke(repeatButton._curRepeatCount);
			repeatButton.IsTriggered = true;
			if (usePlaySound)
				soundHelper.Play(POINTER_TYPE.REPEAT_RESPONSE);
		}

		#endregion

		#endregion

		#region 防连点

		[SerializeField]
		[LabelText("允许点击"), DisplayAsString, ToggleGroup("preventContinuousClick", Order = 6)]
		private bool _canClick = true;

		private bool CanClick
		{
			get => _canClick;
			set
			{
				button.interactable = value;
				_canClick           = value;
			}
		}

		/// <summary>
		/// 是否防止连续点击
		/// </summary>
		[SerializeField]
		[LabelText("是否防连点"), ToggleGroup("preventContinuousClick", ToggleGroupTitle = "防止连续点击")]
		private bool preventContinuousClick;

		/// <summary>
		/// 防止连续点击时间
		/// </summary>
		[SerializeField]
		[LabelText("防连点时间"), ToggleGroup("preventContinuousClick"), SuffixLabel("秒", true)]
		private float preventContinuousTime = 1f;

		/// <summary>
		/// 防止连续点击时间
		/// </summary>
		public float PreventContinuousTime
		{
			get => preventContinuousTime;
			set => preventContinuousTime = value;
		}

		/// <summary>
		/// 防止连续点击是否置灰
		/// </summary>
		[SerializeField]
		[LabelText("防连点是否置灰"), ToggleGroup("preventContinuousClick")]
		private bool preventContinuousWillGray;

		/// <summary>
		/// 防止连点进行中点击事件
		/// </summary>
		[SerializeField]
		[ToggleGroup("preventContinuousClick")]
		private Button.ButtonClickedEvent _onPreventContinuousClick = new();

		/// <summary>
		/// 防止连点的计时器 GUID
		/// </summary>
		private int _preventContinuousClickTimerID = -1;

		/// <summary>
		/// 防止连点进行中点击事件
		/// </summary>
		public Button.ButtonClickedEvent onPreventContinuousClick
		{
			get => _onPreventContinuousClick;
			set => _onPreventContinuousClick = value;
		}

		/// <summary>
		/// 防止连点已结束事件
		/// </summary>
		[SerializeField]
		[ToggleGroup("preventContinuousClick")]
		private Button.ButtonClickedEvent _onPreventContinuousCompleted = new();

		/// <summary>
		/// 防止连点已结束事件
		/// </summary>
		public Button.ButtonClickedEvent onPreventContinuousCompleted
		{
			get => _onPreventContinuousCompleted;
			set => _onPreventContinuousCompleted = value;
		}

		/// <summary>
		/// 中止防连点
		/// </summary>
		public void AbortPreventContinuous()
		{
			var timer = TimerContainer.Instance.GetTimer(_preventContinuousClickTimerID);
			timer?.Stop(true);
		}

		#endregion

		#region 无效与置灰

		/// <summary>
		/// 是否可以使用按钮置灰状态
		/// </summary>
		[SerializeField]
		[LabelText("是否允许置灰"), ToggleGroup("canGray", ToggleGroupTitle = "无效置灰", Order = 8)]
		private bool canGray = true;

		/// <summary>
		/// 置灰状态是否影响子物体
		/// </summary>
		[SerializeField]
		[LabelText("是否影响子物体"), ToggleGroup("canGray")]
		private bool grayAffectChild = true;

		/// <summary>
		/// 无效状态下是否可点击
		/// </summary>
		[SerializeField]
		[LabelText("无效是否可点击"), ToggleGroup("canGray")]
		private bool invalidCanClick;

		/// <summary>
		/// 设置置灰状态
		/// </summary>
		/// <param name="isGray">是否置灰</param>
		private void SetGray(bool isGray)
		{
			if (grayAffectChild)
				UIEffectHelper.SetGrayRecursion(gameObject, isGray);
			else
			{
				if (button.image != null)
					UIEffectHelper.SetGray(button.image, isGray);
			}
		}

		#endregion

		#region 音效

		[SerializeField]
		[ToggleGroup("usePlaySound", ToggleGroupTitle = "音效", Order = 10)]
		private bool usePlaySound = true;

		[SerializeField]
		[ToggleGroup("usePlaySound"), HideLabel]
		private UISoundHelper soundHelper;

		#endregion

		#region 缩放

		/// <summary>
		/// 按钮点击是否应用缩放
		/// </summary>
		[SerializeField]
		[ToggleGroup("useButtonScale", ToggleGroupTitle = "缩放", Order = 11)]
		private bool useButtonScale;

		[SerializeField]
		[ToggleGroup("useButtonScale"), HideLabel]
		private UIScaleHelper uiScaleHelper;

		#endregion

		#region 事件

		/// <summary>
		/// 按钮被按下事件
		/// </summary>
		[SerializeField]
		[FoldoutGroup("Event", Order = 11)]
		private Button.ButtonClickedEvent _onPointerDown = new();

		/// <summary>
		/// 按钮被按下事件
		/// </summary>
		public Button.ButtonClickedEvent onPointerDown
		{
			get => _onPointerDown;
			set => _onPointerDown = value;
		}

		/// <summary>
		/// 按钮抬起事件
		/// </summary>
		[SerializeField]
		[FoldoutGroup("Event")]
		private Button.ButtonClickedEvent _onPointerUp = new();

		/// <summary>
		/// 按钮抬起事件
		/// </summary>
		public Button.ButtonClickedEvent onPointerUp
		{
			get => _onPointerUp;
			set => _onPointerUp = value;
		}

		/// <summary>
		/// 按钮点击事件
		/// </summary>
		[SerializeField]
		[FoldoutGroup("Event")]
		private Button.ButtonClickedEvent _onPointerClick = new();

		/// <summary>
		/// 按钮点击事件
		/// </summary>
		public Button.ButtonClickedEvent onPointerClick
		{
			get => _onPointerClick;
			set => _onPointerClick = value;
		}

		/// <summary>
		/// 按钮双击事件
		/// </summary>
		[SerializeField]
		[FoldoutGroup("Event")]
		private Button.ButtonClickedEvent _onPointerDoubleClick = new();

		/// <summary>
		/// 按钮双击事件
		/// </summary>
		public Button.ButtonClickedEvent onPointerDoubleClick
		{
			get => _onPointerDoubleClick;
			set => _onPointerDoubleClick = value;
		}

		#endregion

		#region 设置状态

		private IMultiState[] multiStateChildren;

		/// <summary>
		/// 修改状态
		/// </summary>
		/// <param name="targetState">状态索引</param>
		/// <param name="force">是否强制设置状态索引,可缺省,默认为false</param>
		[Button(
			 DisplayParameters = true,
			 Expanded = true,
			 Name = "设置状态",
			 Style = ButtonStyle.CompactBox),
		 BoxGroup("Debug", CenterLabel = true, Order = 12)]
		public void SetState(int targetState, bool force = false)
		{
			multiStateChildren ??= transform.GetComponentsInChildren<IMultiState>(true);

			foreach (var multiStateChild in multiStateChildren)
			{
				if (multiStateChild.AffectedByParent)
					multiStateChild.SetState(targetState, force);
			}

#if UNITY_EDITOR
			if (Application.isPlaying) return;

			multiStateChildren = null;
#endif
		}

		#endregion

		/// <summary>
		/// 是否点在按钮上
		/// </summary>
		private bool _isPointerInside;

		/// <summary>
		/// 是否已经按下
		/// </summary>
		private bool _isPointerDown;

		protected override void OnAwake()
		{
			base.OnAwake();

			_canClick = true;
		}

		protected override void OnStart()
		{
			base.OnStart();

			uiScaleHelper.Init();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			if (Application.isPlaying)
				TickManager.Instance.Add(this);

			if (useButtonScale)
				uiScaleHelper.Out();
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			if (Application.isPlaying)
				TickManager.Instance.Remove(this);

			_isPointerDown   = false;
			_isPointerInside = false;

			switch (_buttonType)
			{
				case BUTTON_TYPE.LONG_PRESS:
					OnLongPressDisable();
					break;
				case BUTTON_TYPE.REPEAT:
					OnRepeatButtonDisable();
					break;
			}
		}

		protected override void OnDestroyed()
		{
			if (preventContinuousClick && _preventContinuousClickTimerID > 0)
			{
				var timer = TimerContainer.Instance.GetTimer(_preventContinuousClickTimerID);
				timer?.Stop(true);
			}

			onPointerClick?.RemoveAllListeners();
			onPointerDown?.RemoveAllListeners();
			onPointerUp?.RemoveAllListeners();
			onPointerDoubleClick?.RemoveAllListeners();
			onLongPress?.RemoveAllListeners();
			onRepeatRespond?.RemoveAllListeners();
			onPreventContinuousClick?.RemoveAllListeners();
			onPreventContinuousCompleted?.RemoveAllListeners();

			uiScaleHelper.Stop();
			uiScaleHelper = null;

			base.OnDestroyed();
		}

		bool IBaseTick.runInBackground => false;

		public int TickOrder => 0;

		public void OnTick(float deltaTime)
		{
			switch (_buttonType)
			{
				case BUTTON_TYPE.LONG_PRESS:
					OnLongPressUpdate();
					break;
				case BUTTON_TYPE.REPEAT:
					OnRepeatButtonUpdate();
					break;
			}
		}

		/// <summary>
		/// 当按钮按下执行按下事件
		/// </summary>
		private void PointerDown()
		{
			if (!button.isActiveAndEnabled || !button.IsInteractable())
				return;

			_isPointerInside = true;

			if (useButtonScale)
				uiScaleHelper.In();
			if (usePlaySound)
				soundHelper.Play(POINTER_TYPE.DOWN);

			_onPointerDown?.Invoke();
		}

		/// <summary>
		/// 当按钮抬起执行抬起事件
		/// </summary>
		private void PointerUp()
		{
			if (!button.isActiveAndEnabled || !button.IsInteractable())
				return;

			if (useButtonScale)
				uiScaleHelper.Out();

			if (preventContinuousClick && !CanClick)
				return;

			if (usePlaySound)
				soundHelper.Play(POINTER_TYPE.UP);

			_onPointerUp?.Invoke();
		}

		#region PointerDown

		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			_isPointerDown   = true;
			_isPointerInside = true;

			switch (_buttonType)
			{
				case BUTTON_TYPE.LONG_PRESS:
					OnLongPressPointerDown();
					break;
				case BUTTON_TYPE.REPEAT:
					OnRepeatButtonPointerDown();
					break;
			}

			PointerDown();
		}

		#endregion

		#region PointerUp

		public void OnPointerUp(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			_isPointerDown = false;

			PointerUp();
		}

		#endregion

		#region PointerClick

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			if (preventContinuousClick)
			{
				if (!CanClick)
				{
					_onPreventContinuousClick?.Invoke();
					return;
				}

				Timer timer = null;

				if (_preventContinuousClickTimerID > 0)
				{
					timer = TimerContainer.Instance.GetTimer(_preventContinuousClickTimerID);
					timer?.Stop();
				}

				timer = Timer.Create(preventContinuousTime, callback =>
				{
					CanClick = true;
					_onPreventContinuousCompleted?.Invoke();
					if (preventContinuousWillGray)
						SetInteractable(true);
				}).AddStartCallback(callback =>
				{
					CanClick = false;
					if (preventContinuousWillGray)
						SetInteractable(false);
					if (useButtonScale)
						uiScaleHelper.Out();
					button.onClick?.Invoke();
				});
				timer.Startup();

				_preventContinuousClickTimerID = timer.ID;
			}

			if (longPressButton is {IsTriggered: false} && repeatButton is {IsTriggered: false})
			{
				switch (eventData.clickCount)
				{
					case 1:
						// 单击
						_onPointerClick?.Invoke();
						break;
					case 2:
						// 双击
						_onPointerDoubleClick?.Invoke();
						break;
				}
			}
			else
			{
				switch (_buttonType)
				{
					case BUTTON_TYPE.LONG_PRESS:
						OnLongPressPointerClick();
						break;
					case BUTTON_TYPE.REPEAT:
						OnRepeatButtonPointerClick();
						break;
				}
			}

			if (usePlaySound)
				soundHelper.Play(POINTER_TYPE.CLICK);
		}

		#endregion

		#region PointerEnter&Exit

		public void OnPointerEnter(PointerEventData eventData) { _isPointerInside = true; }

		public void OnPointerExit(PointerEventData eventData) { _isPointerInside = false; }

		#endregion
	}

#if UNITY_EDITOR
	public sealed partial class UIButton
	{
		protected override void Reset()
		{
			base.Reset();

			usePlaySound   = true;
			useButtonScale = true;
			uiScaleHelper  = new UIScaleHelper(rectTransform);
			soundHelper    = new UISoundHelper();

			GetProperty();

			if (button.image == null)
				button.transition = Selectable.Transition.None;
		}

		protected internal override bool CheckValidate(ref string error)
		{
			if (uiScaleHelper?.rectTransform == null)
				uiScaleHelper = new UIScaleHelper(rectTransform);

			return true;
		}

		#region 获得按钮的可用对象

		[LabelText("按钮文字"),
		 BoxGroup("可用对象", Order = 5),
		 Button("自动获取"), GUIColor(0, 1, 0),
		 DisableIf(
			 "@buttonText && (button.image || (button.transition != Selectable.Transition.ColorTint && button.transition != Selectable.Transition.SpriteSwap))")]
		private void GetProperty()
		{
			var children = transform.GetComponentsInChildren<RectTransform>();
			foreach (var child in children)
			{
				if (child.GetComponent<UILabel>())
				{
					if (buttonText == null)
						buttonText = child.GetComponent<UILabel>();
				}
				else if (child.GetComponent<RichImage>())
				{
					if (button.image == null && button.transition is Selectable.Transition.ColorTint or Selectable.Transition.SpriteSwap)
						button.image = child.GetComponent<RichImage>();
				}
			}

			if (_buttonImage == null)
				button.image.TryGetComponent(out _buttonImage);
		}

		#endregion
	}
#endif
}