using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;

namespace KiwiFramework.Runtime.UI
{
	[HideMonoScript]
	[RequireComponent(typeof(ToggleGroup)),
	AddComponentMenu("KiwiUI/UIToggleGroup")]
	public class UIToggleGroup : UIElement
	{
		public override bool ForceCallInCode => true;

		#region ToggleGroup 属性

		private ToggleGroup _native;

		public ToggleGroup Native
		{
			get
			{
				if (_native == null)
					_native = GetComponent<ToggleGroup>();
				return _native;
			}
		}

		#endregion

		#region AllowSwitchOff 属性

		[SerializeField, HideInInspector] private bool _allowSwitchOff;

		[LabelText("是否允许全部为 OFF"), ShowInInspector]
		public bool allowSwitchOff
		{
			get => _allowSwitchOff;
			set
			{
				_allowSwitchOff       = value;
				Native.allowSwitchOff = _allowSwitchOff;
			}
		}

		#endregion

		protected override void OnAwake()
		{
			base.OnAwake();

			Native.allowSwitchOff = allowSwitchOff;
		}

		/// <summary>
		/// 通知指定 Toggle 勾选
		/// </summary>
		/// <param name="toggle">要通知的 toggle 对象</param>
		/// <param name="sendCallback">是否需要 toggle 执行 OnValueChanged 事件</param>
		public void NotifyToggleOn(Toggle toggle, bool sendCallback = true)
		{
			if (Native != null && toggle != null)
				Native.NotifyToggleOn(toggle, sendCallback);
		}

		/// <summary>
		/// 通知指定 Toggle 勾选
		/// </summary>
		/// <param name="proxy">要通知的 toggleProxy 对象</param>
		/// <param name="sendCallback">是否需要 toggle 执行 OnValueChanged 事件</param>
		public void NotifyToggleOn(UIToggle proxy, bool sendCallback = true)
		{
			if (Native != null && proxy != null && proxy.Native != null)
				Native.NotifyToggleOn(proxy.Native, sendCallback);
		}

		/// <summary>
		/// 添加 Toggle 到组中
		/// </summary>
		/// <param name="proxy">要添加的 ToggleProxy 对象</param>
		public void RegisterToggle(UIToggle proxy)
		{
			if (Native != null && proxy != null)
			{
				proxy.group = Native;
				Native.RegisterToggle(proxy.Native);
			}
		}

		/// <summary>
		/// 移除 Toggle
		/// </summary>
		/// <param name="proxy">要移除的 ToggleProxy</param>
		public void UnregisterToggle(UIToggle proxy)
		{
			if (Native != null && proxy != null)
			{
				proxy.group = null;
				Native.UnregisterToggle(proxy.Native);
			}
		}

		/// <summary>
		/// 确认有效状态
		/// </summary>
		public void EnsureValidState()
		{
			if (Native != null)
				Native.EnsureValidState();
		}

		/// <summary>
		/// 是否有任意一个 Toggle 的状态为 On
		/// </summary>
		/// <returns></returns>
		public bool AnyTogglesOn() { return Native.AnyTogglesOn(); }

		/// <summary>
		/// 设置全部 Toggle 为 Off
		/// </summary>
		/// <param name="sendCallback">是否需要 toggle 执行 OnValueChanged 事件</param>
		public void SetAllTogglesOff(bool sendCallback = true) { Native.SetAllTogglesOff(sendCallback); }
	}
}