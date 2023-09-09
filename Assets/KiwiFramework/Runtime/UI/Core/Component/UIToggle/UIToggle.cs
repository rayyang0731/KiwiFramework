using System.Collections.Generic;

using KiwiFramework.Runtime.UnityExtend;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 开关组件
	/// </summary>
	[HideMonoScript]
	[RequireComponent(typeof(Toggle))]
	[AddComponentMenu("KiwiUI/UIToggle")]
	public partial class UIToggle : UIElement, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		public override bool ForceCallInCode => true;

		/// <summary>
		/// 开关效果类型
		/// </summary>
		private enum TOGGLE_TYPE
		{
			/// <summary>
			/// Unity Toggle 类型
			/// </summary>
			[InspectorName("Unity Toggle")]
			NATIVE,

			/// <summary>
			/// 切换对象类型
			/// </summary>
			[InspectorName("切换对象")]
			SWITCH_OBJECTS,

			/// <summary>
			/// 切换状态类型
			/// </summary>
			[InspectorName("切换状态")]
			SWITCH_STATUE,
		}

		#region Toggle 属性

		private Toggle _native;

		public Toggle Native
		{
			get
			{
				if (_native == null)
					_native = GetComponent<Toggle>();
				return _native;
			}
		}

		#endregion

		#region 可用状态

		/// <summary>
		/// 是否为可用状态
		/// </summary>
		[LabelText("是否可用"), ShowInInspector, PropertyOrder(-1)]
		public bool interactable
		{
			get => Native.interactable;
			set
			{
				if (!canGray)
					Native.interactable = value;
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

			Native.interactable = state || invalidCanClick;

			SetGray(!state);
		}

		#endregion

		#region isOn 属性

		[ShowInInspector]
		public bool isOn
		{
			get => Native != null && Native.isOn;
			set
			{
				if (Native != null)
				{
					Native.isOn = value;

#if UNITY_EDITOR
					if (!Application.isPlaying)
					{
						OnValueChanged(value);
						UnityEditor.EditorUtility.SetDirty(gameObject);
					}
#endif
				}
			}
		}

		#endregion

		#region ToggleGroup

		[SerializeField, HideInInspector] private ToggleGroup _group;

		[LabelText("开关组"), ShowInInspector]
		public ToggleGroup group
		{
			get => _group;
			set
			{
				_group       = value;
				Native.group = _group != null ? _group : null;
			}
		}

		#endregion

		#region 开关效果

		[SerializeField]
		[LabelText("效果类型"), PropertyOrder(2), BoxGroup("开关属性", CenterLabel = true)]
		private TOGGLE_TYPE _toggleType = TOGGLE_TYPE.NATIVE;

		/// <summary>
		/// 多状态图片组件
		/// </summary>
		[SerializeField]
		[Indent, LabelText("状态图对象"),
		 ShowIf(nameof(_toggleType), TOGGLE_TYPE.SWITCH_STATUE),
		 PropertyOrder(2),
		 BoxGroup("开关属性")]
		private UIImage[] _images;

		/// <summary>
		/// 多状态文字组件
		/// </summary>
		[SerializeField]
		[Indent, LabelText("状态图文字"),
		 ShowIf(nameof(_toggleType), TOGGLE_TYPE.SWITCH_STATUE),
		 PropertyOrder(2),
		 BoxGroup("开关属性")]
		private UILabel[] _tmps;

		/// <summary>
		/// 开关的ON对象
		/// </summary>
		[SerializeField]
		[Indent, LabelText("ON 对象"),
		 ShowIf(nameof(_toggleType), TOGGLE_TYPE.SWITCH_OBJECTS),
		 PropertyOrder(2),
		 BoxGroup("开关属性")]
		private GameObject[] _on;

		/// <summary>
		/// 开关的OFF对象
		/// </summary>
		[SerializeField]
		[Indent, LabelText("OFF 对象"),
		 ShowIf(nameof(_toggleType), TOGGLE_TYPE.SWITCH_OBJECTS),
		 PropertyOrder(2),
		 BoxGroup("开关属性")]
		private GameObject[] _off;

		#endregion

		#region 音效

		/// <summary>
		/// 按钮点击是否播放音效
		/// </summary>
		[SerializeField]
		[ToggleGroup("usePlaySound", ToggleGroupTitle = "音效", Order = 9),
		 PropertyOrder(7)]
		private bool usePlaySound = true;

		[SerializeField]
		[ToggleGroup("usePlaySound"), HideLabel]
		private UISoundHelper soundHelper;

		#endregion

		#region 无效与置灰

		/// <summary>
		/// 是否可以使用按钮置灰状态
		/// </summary>
		[SerializeField]
		[LabelText("是否允许置灰")]
		[ToggleGroup("canGray", ToggleGroupTitle = "无效置灰", Order = 8),
		 PropertyOrder(6)]
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
				if (Native.image != null)
					UIEffectHelper.SetGray(Native.image, isGray);
			}
		}

		#endregion

		#region OnValueChanged 事件

		/// <summary>
		/// 值发生改变事件
		/// </summary>
		[SerializeField]
		[FoldoutGroup("Event", Order = 11)]
		private Toggle.ToggleEvent _onValueChanged = new();

		/// <summary>
		/// 值发生改变事件
		/// </summary>
		public Toggle.ToggleEvent onValueChanged => _onValueChanged;

		#endregion

		/// <summary>
		/// 设置 isOn 不调用 ValueChange
		/// </summary>
		/// <param name="value"></param>
		public void SetIsOnWithoutNotify(bool value) { Native.SetIsOnWithoutNotify(value); }

		private void PointerDown()
		{
			if (!Native.IsActive() || !Native.IsInteractable())
				return;

			UISystemProfilerApi.AddMarker("UIToggleProxy.onPointerDown", this);

			if (usePlaySound)
				soundHelper.Play(UIButton.POINTER_TYPE.DOWN);
		}

		private void PointerUp()
		{
			if (!Native.IsActive())
				return;

			UISystemProfilerApi.AddMarker("UIToggleProxy.onPointerUp", this);

			if (usePlaySound)
				soundHelper.Play(UIButton.POINTER_TYPE.UP);
		}

		private void OnValueChanged(bool value)
		{
			switch (_toggleType)
			{
				case TOGGLE_TYPE.SWITCH_STATUE:
				{
					if (_images != null && _images.Length > 0)
					{
						foreach (var image in _images)
						{
							image.SetState(value ? 1 : 0);
						}
					}

					if (_tmps != null && _tmps.Length > 0)
					{
						foreach (var tmp in _tmps)
						{
							tmp.SetState(value ? 1 : 0);
						}
					}

					break;
				}
				case TOGGLE_TYPE.SWITCH_OBJECTS:
					if (_on != null && _on.Length > 0)
					{
						foreach (var go in _on)
						{
							go.SetActive(value);
						}
					}

					if (_off != null && _off.Length > 0)
					{
						foreach (var go in _off)
						{
							go.SetActive(!value);
						}
					}

					break;
				case TOGGLE_TYPE.NATIVE:
				default:
					break;
			}

			_onValueChanged?.Invoke(value);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			PointerDown();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			PointerUp();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			if (usePlaySound)
				soundHelper.Play(UIButton.POINTER_TYPE.CLICK);
		}

		protected override void OnAwake()
		{
			base.OnAwake();

			if (group != null)
				Native.group = group;

			OnValueChanged(Native.isOn);

			Native.onValueChanged.AddListener(OnValueChanged);
		}
	}

#if UNITY_EDITOR
	public partial class UIToggle
	{
		protected override void OnValidate()
		{
			base.OnValidate();

			soundHelper ??= new UISoundHelper();

			switch (_toggleType)
			{
				case TOGGLE_TYPE.NATIVE:
					if (Native != null)
						Native.toggleTransition = Toggle.ToggleTransition.Fade;

					_images = null;
					_tmps   = null;

					_off = null;
					_on  = null;

					break;
				case TOGGLE_TYPE.SWITCH_STATUE:
				{
					if (Native != null)
						Native.toggleTransition = Toggle.ToggleTransition.None;

					_off = null;
					_on  = null;

					var stateNames = new[] {"OFF", "ON"};

					_images = transform.GetComponentsInChildren<UIImage>(true);
					foreach (var image in _images)
					{
						if (image.StateCount < 2)
						{
							while (image.StateCount < 2)
							{
								image.AddState(ImageStateData.Default);
							}
						}
						else if (image.StateCount > 2)
						{
							while (image.StateCount > 2)
							{
								image.RemoveState(image.StateCount - 1);
							}
						}

						for (var i = 0; i < 2; i++)
						{
							if (!image.GetState(i, out var data)) continue;

							data.name = stateNames[i];
							image.ChangeStateData(i, data);
						}
					}

					_tmps = transform.GetComponentsInChildren<UILabel>(true);
					foreach (var tmp in _tmps)
					{
						if (tmp.StateCount < 2)
						{
							while (tmp.StateCount < 2)
							{
								tmp.AddState(LabelStateData.Default);
							}
						}
						else if (tmp.StateCount > 2)
						{
							while (tmp.StateCount > 2)
							{
								tmp.RemoveState(tmp.StateCount - 1);
							}
						}

						for (var i = 0; i < 2; i++)
						{
							if (!tmp.GetState(i, out var data)) continue;

							data.name = stateNames[i];
							tmp.ChangeStateData(i, data);
						}
					}

					break;
				}
				case TOGGLE_TYPE.SWITCH_OBJECTS:
				{
					if (Native != null)
						Native.toggleTransition = Toggle.ToggleTransition.None;

					_images = null;
					_tmps   = null;

					if (_off == null || _off.Length < 1)
					{
						var offGos = new List<GameObject>();
						foreach (Transform child in transform)
						{
							if (child.name.ToLower().Contains("off"))
							{
								offGos.Add(child.gameObject);
							}
						}

						if (offGos.Count > 0)
						{
							_off = offGos.ToArray();
						}
						else
						{
							var go = new GameObject("OFF", typeof(RectTransform));
							go.rectTransform().pivot = this.rectTransform().pivot;
							go.rectTransform().SetParentAndPos(transform, Vector3.zero);
							go.rectTransform().pivot      = Vector2.one * 0.5f;
							go.rectTransform().localScale = Vector3.one;

							_off = new[] {go};
						}
					}

					if (_on == null || _on.Length < 1)
					{
						var onGos = new List<GameObject>();
						foreach (Transform child in transform)
						{
							if (child.name.ToLower().Contains("on"))
							{
								onGos.Add(child.gameObject);
							}
						}

						if (onGos.Count > 0)
						{
							_on = onGos.ToArray();
						}
						else
						{
							var go = new GameObject("ON", typeof(RectTransform));
							go.rectTransform().pivot = this.rectTransform().pivot;
							go.rectTransform().SetParentAndPos(transform, Vector3.zero);
							go.rectTransform().pivot      = Vector2.one * 0.5f;
							go.rectTransform().localScale = Vector3.one;

							_on = new[] {go};
						}
					}

					break;
				}
			}
		}

		protected override void Reset()
		{
			base.Reset();

			usePlaySound = true;
			soundHelper  = new UISoundHelper();
		}
	}
#endif
}