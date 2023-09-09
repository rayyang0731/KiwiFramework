using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// UI文本代理脚本
	/// </summary>
	[RequireComponent(typeof(Slider)),
	 AddComponentMenu("KiwiUI/UISlider")]
	public partial class UISlider : UIElement, IPointerUpHandler
	{
		public override bool ForceCallInCode => true;

		/// <summary>
		/// 是否为可用状态
		/// </summary>
		[LabelText("是否可用"), ShowInInspector, PropertyOrder(-1)]
		public bool interactable
		{
			get => slider.interactable;
			set
			{
				if (!canGray)
					slider.interactable = value;
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

			slider.interactable = state || invalidCanClick;

			SetGray(!state);
		}

		private Slider _slider;

		public Slider slider
		{
			get
			{
				if (_slider == null)
					_slider = GetComponent<Slider>();
				return _slider;
			}
		}

		public float value
		{
			get => slider.value;
			set => slider.value = value;
		}

		public float minValue
		{
			get => slider.minValue;
			set => slider.minValue = value;
		}

		public float maxValue
		{
			get => slider.maxValue;
			set => slider.maxValue = value;
		}

		public float normalizedValue
		{
			get => slider.normalizedValue;
			set => slider.normalizedValue = value;
		}


		[SerializeField]
		[ShowInInspector, BoxGroup("滑条对象"), LabelText("文本")]
		private UILabel _textComp;

		public UILabel textComp => _textComp;

		/// <summary>
		/// 设置文本描述内容
		/// </summary>
		/// <param name="text">内容</param>
		public void SetText(string text)
		{
			if (_textComp != null)
				_textComp.text = text;
		}

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
				if (slider.image != null)
					UIEffectHelper.SetGray(slider.image, isGray);
			}
		}

		private Slider.SliderEvent _onValueChanged = new();

		public Slider.SliderEvent onValueChanged => _onValueChanged;

		private void OnValueChanged(float val) { onValueChanged.Invoke(val); }

		[SerializeField]
		[PropertyOrder(10), Space]
		private Slider.SliderEvent _onPointerUp = new();

		public Slider.SliderEvent onPointerUp => _onPointerUp;

		/// <summary>
		/// 设置滑动条的值(和文本描述)
		/// </summary>
		/// <param name="val">滑动条数值</param>
		/// <param name="text">描述内容</param>
		public void SetValue(float val, string text = null)
		{
			value = val;
			if (text != null)
				SetText(text);
		}

		protected override void OnAwake()
		{
			base.OnAwake();

			slider.onValueChanged.AddListener(OnValueChanged);
		}

		public void OnPointerUp(PointerEventData eventData) { onPointerUp.Invoke(value); }
	}

#if UNITY_EDITOR
	public partial class UISlider
	{
		[ShowInInspector, BoxGroup("滑条对象", CenterLabel = true, Order = 5), LabelText("背景")]
		private Image SliderBG
		{
			get => slider.image;
			set => slider.image = value;
		}

		[ShowInInspector, BoxGroup("滑条对象"), LabelText("填充"),
		 InfoBox("填充对象不得为 Null",
		         InfoMessageType.Error,
		         "@slider.fillRect == null")]
		private RectTransform SliderFillRect
		{
			get => slider.fillRect;
			set => slider.fillRect = value;
		}

		[ShowInInspector, BoxGroup("滑条对象"), LabelText("滑块")]
		private RectTransform SliderHandleRect
		{
			get => slider.handleRect;
			set => slider.handleRect = value;
		}

		protected internal override bool CheckValidate(ref string error)
		{
			if (SliderFillRect != null)
				return true;

			error = "slider.fillRect 为 null. slider 的 fillRect 字段必须有对象.";
			return false;
		}
	}
#endif
}