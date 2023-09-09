using System;

using Sirenix.OdinInspector;

using TMPro;

using UnityEngine;
using UnityEngine.Events;

namespace KiwiFramework.Runtime.UI
{
	[HideMonoScript]
	[RequireComponent(typeof(TMP_InputField))]
	[AddComponentMenu("KiwiUI/UIInputField")]
	public partial class UIInputField : UIElement
	{
		public override bool ForceCallInCode => true;

		/// <summary>
		/// 输入有效性检测事件
		/// </summary>
		[Serializable]
		public class InputValidateEvent : UnityEvent<string, int, char>
		{
		}

		#region InputField 属性

		[SerializeField, HideInInspector] private TMP_InputField _native;

		public TMP_InputField Native
		{
			get
			{
				if (_native == null)
					_native = GetComponent<TMP_InputField>();
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
			set => Native.interactable = value;
		}

		#endregion

		#region 是否检测输入有效性

		/// <summary>
		/// 是否检测输入有效性
		/// </summary>
		[LabelText("是否检测输入有效性"), SerializeField]
		private bool _checkValidate = false;

		/// <summary>
		/// 是否检测输入有效性
		/// </summary>
		public bool checkValidate
		{
			get => _checkValidate;
			set
			{
				_checkValidate = value;
				if (_checkValidate)
					Native.onValidateInput += OnValidateInput;
				else
					Native.onValidateInput -= OnValidateInput;
			}
		}

		#endregion

		#region text 属性

		public string text
		{
			get => Native == null ? string.Empty : Native.text;
			set
			{
				if (Native == null) return;

				Native.text = value;
			}
		}

		#endregion

		#region OnValueChange 事件

		/// <summary>
		/// 值发生改变事件
		/// </summary>
		[SerializeField]
		[FoldoutGroup("Event", Order = 11)]
		private TMP_InputField.OnChangeEvent _onValueChange = new();

		public TMP_InputField.OnChangeEvent onValueChange
		{
			get => _onValueChange;
			set => _onValueChange = value;
		}

		#endregion

		#region OnEndEdit 事件

		/// <summary>
		/// 当输入法点击提交或点击其他地方执行该事件
		/// </summary>
		[SerializeField]
		[FoldoutGroup("Event", Order = 11)]
		private TMP_InputField.SubmitEvent _onEndEdit = new();

		/// <summary>
		/// 当输入法点击提交或点击其他地方执行该事件
		/// </summary>
		public TMP_InputField.SubmitEvent onEndEdit
		{
			get => _onEndEdit;
			set => _onEndEdit = value;
		}

		#endregion

		#region OnSubmit 事件

		/// <summary>
		/// 当输入法点击提交按钮事件
		/// </summary>
		[SerializeField]
		[FoldoutGroup("Event", Order = 11)]
		private TMP_InputField.SubmitEvent _onSubmit = new();

		/// <summary>
		/// 当输入法点击提交按钮事件
		/// </summary>
		public TMP_InputField.SubmitEvent onSubmit
		{
			get => _onSubmit;
			set => _onSubmit = value;
		}

		#endregion

		#region OnSelect 事件

		/// <summary>
		/// 当被选中时的事件
		/// </summary>
		[SerializeField]
		[FoldoutGroup("Event", Order = 11)]
		private TMP_InputField.SelectionEvent _onSelect = new();

		/// <summary>
		/// 当被选中时的事件
		/// </summary>
		public TMP_InputField.SelectionEvent onSelect
		{
			get => _onSelect;
			set => _onSelect = value;
		}

		#endregion

		#region OnDeselect 事件

		/// <summary>
		/// 当取消选中时的事件
		/// </summary>
		[SerializeField]
		[FoldoutGroup("Event", Order = 11)]
		private TMP_InputField.SelectionEvent _onDeselect = new();

		/// <summary>
		/// 当取消选中时的事件
		/// </summary>
		public TMP_InputField.SelectionEvent onDeselect
		{
			get => _onDeselect;
			set => _onDeselect = value;
		}

		#endregion

		#region onValidateInput 事件

		/// <summary>
		/// 检测输入有效性事件
		/// </summary>
		public Func<string, int, char, char> onValidateInput { get; set; }

		/// <summary>
		/// 当检测输入有效性
		/// </summary>
		/// <param name="beforeInputContent">在此次输入之前,输入框中的内容</param>
		/// <param name="charIndex">当前输入 char 的索引位置</param>
		/// <param name="addedChar">此次添加的 char</param>
		/// <returns></returns>
		private char OnValidateInput(string beforeInputContent, int charIndex, char addedChar)
		{
			return onValidateInput?.Invoke(beforeInputContent, charIndex, addedChar) ?? addedChar;
		}

		#endregion

		#region OnTouchScreenKeyboardStatusChanged 事件

		/// <summary>
		/// 当屏幕键盘状态发生变化时的事件
		/// </summary>
		[SerializeField]
		[FoldoutGroup("Event", Order = 11)]
		private TMP_InputField.TouchScreenKeyboardEvent _onTouchScreenKeyboardStatusChanged = new();

		/// <summary>
		/// 当屏幕键盘状态发生变化时的事件
		/// </summary>
		public TMP_InputField.TouchScreenKeyboardEvent onTouchScreenKeyboardStatusChanged
		{
			get => _onTouchScreenKeyboardStatusChanged;
			set => _onTouchScreenKeyboardStatusChanged = value;
		}

		#endregion

		protected override void OnAwake()
		{
			base.OnAwake();

			Native.onValueChanged                     = onValueChange;
			Native.onEndEdit                          = onEndEdit;
			Native.onSubmit                           = onSubmit;
			Native.onSelect                           = onSelect;
			Native.onDeselect                         = onDeselect;
			Native.onTouchScreenKeyboardStatusChanged = onTouchScreenKeyboardStatusChanged;

			if (checkValidate)
				Native.onValidateInput += OnValidateInput;
		}

		protected override void OnDestroyed()
		{
			Native.onValueChanged.RemoveAllListeners();
			Native.onEndEdit.RemoveAllListeners();
			Native.onSubmit.RemoveAllListeners();
			Native.onSelect.RemoveAllListeners();
			Native.onDeselect.RemoveAllListeners();
			Native.onTouchScreenKeyboardStatusChanged.RemoveAllListeners();

			Native.onValidateInput = null;

			base.OnDestroyed();
		}
	}
}