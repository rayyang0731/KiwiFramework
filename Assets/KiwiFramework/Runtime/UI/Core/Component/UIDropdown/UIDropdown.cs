using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace KiwiFramework.Runtime.UI
{
	[HideMonoScript]
	[RequireComponent(typeof(Dropdown))]
	public class UIDropdown : UIElement
	{
		public override bool ForceCallInCode => true;

		#region Dropdown 属性

		private Dropdown _dropdown;

		public Dropdown dropdown
		{
			get
			{
				if (_dropdown == null)
					_dropdown = GetComponent<Dropdown>();
				return _dropdown;
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
			get => dropdown.interactable;
			set
			{
				if (!canGray)
					dropdown.interactable = value;
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

			dropdown.interactable = state || invalidCanClick;

			SetGray(!state);
		}

		#endregion

		#region Value 属性

		public int value
		{
			get
			{
				if (dropdown == null)
				{
					Debug.LogError("dropdown 为 null ,但是理论上来说,不可能发生这种情况. 请检查具体发生原因.");
					return -1;
				}

				return dropdown.value;
			}
			set
			{
				if (dropdown == null)
					Debug.LogError("dropdown 为 null ,但是理论上来说,不可能发生这种情况. 请检查具体发生原因.");
				dropdown.value = value;
			}
		}

		#endregion

		#region 无效与置灰

		/// <summary>
		/// 是否可以使用按钮置灰状态
		/// </summary>
		[SerializeField]
		[LabelText("是否允许置灰"), ToggleGroup("canGray", ToggleGroupTitle = "无效置灰", Order = 8), PropertyOrder(6)]
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
				if (dropdown.image != null)
					UIEffectHelper.SetGray(dropdown.image, isGray);
			}
		}

		#endregion

		#region OnPlayArrowAnimation

		/// <summary>
		/// 自定义箭头动画事件
		/// </summary>
		[SerializeField, FoldoutGroup("Event", 11)]
		private Dropdown.DropdownStateEvent _onPlayArrowAnimation = new();

		/// <summary>
		/// 自定义箭头动画事件
		/// </summary>
		public Dropdown.DropdownStateEvent onPlayArrowAnimation => _onPlayArrowAnimation;

		private void OnPlayArrowAnimation(bool isOn) { onPlayArrowAnimation.Invoke(isOn); }

		#endregion

		#region OnValueChanged

		[SerializeField, FoldoutGroup("Event")]
		private Dropdown.DropdownEvent _onValueChanged = new();

		public Dropdown.DropdownEvent onValueChanged => _onValueChanged;

		private void OnValueChanged(int val) { onValueChanged.Invoke(val); }

		#endregion

		/// <summary>
		/// 添加选项(文本描述)
		/// </summary>
		/// <param name="labels">选项内容数组</param>
		public void AddOptions(string[] labels)
		{
			if (labels != null && labels.Length > 0)
				dropdown.AddOptions(labels);
		}

		/// <summary>
		/// 添加选项(图标显示)
		/// </summary>
		/// <param name="sprites"></param>
		public void AddOptions(Sprite[] sprites)
		{
			if (sprites != null && sprites.Length > 0)
				dropdown.AddOptions(sprites);
		}

		/// <summary>
		/// 添加选项(文本描述及图标)
		/// </summary>
		/// <param name="labels">选项的文本内容数组</param>
		/// <param name="sprites">选项的图标数组</param>
		public void AddOptions(string[] labels, Sprite[] sprites)
		{
			if (labels != null && sprites != null)
				dropdown.AddOptions(labels, sprites);
		}

		/// <summary>
		/// 添加多个选项
		/// </summary>
		/// <param name="options">要添加的选项数据的列表</param>
		public void AddOptions(List<Dropdown.OptionData> options)
		{
			if (options is {Count: > 0})
				dropdown.AddOptions(options);
		}

		/// <summary>
		/// 添加多个选项
		/// </summary>
		/// <param name="options">要添加的选项文本的列表</param>
		public void AddOptions(List<string> options)
		{
			if (options is {Count: > 0})
				dropdown.AddOptions(options);
		}

		/// <summary>
		/// 添加多个选项
		/// </summary>
		/// <param name="options">要添加的选项图片的列表</param>
		public void AddOptions(List<Sprite> options)
		{
			if (options != null && options.Count > 0)
				dropdown.AddOptions(options);
		}

		/// <summary>
		/// 清除选项列表中的全部选项
		/// </summary>
		public void ClearOptions() { dropdown.ClearOptions(); }

		protected override void OnAwake()
		{
			base.OnAwake();

			dropdown.onValueChanged.AddListener(OnValueChanged);
			dropdown.onPlayArrowAnimation.AddListener(OnPlayArrowAnimation);
		}
	}
}