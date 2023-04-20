using System.Collections.Generic;
using System.Linq;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 聚焦面板组件
	/// </summary>
	[HideMonoScript]
	[AddComponentMenu("KiwiUI/UIFocusBoard")]
	public class UIFocusBoard : UIElement, ILateTick
	{
		public override bool ForceCallInCode => true;

		#region Private Variables

		/// <summary>
		/// 当前焦点是否在面板内
		/// </summary>
		[ShowInInspector, BoxGroup, LabelText("当前焦点是否在面板内"), ToggleLeft,
		 ReadOnly, GUIColor("@_focusInBoard?UnityEngine.Color.green:UnityEngine.Color.red")]
		private bool _focusInBoard;

		/// <summary>
		/// 子面板
		/// </summary>
		[SerializeField]
		[ShowInInspector, LabelText("子面板列表")]
		private List<GameObject> _boardOfChildren = new();

		/// <summary>
		/// 聚焦响应的事件
		/// </summary>
		[SerializeField]
		private Button.ButtonClickedEvent _onFocusedEvent = new();

		/// <summary>
		/// 聚焦响应的事件
		/// </summary>
		public Button.ButtonClickedEvent onFocusedEvent => _onFocusedEvent;

		/// <summary>
		/// 失焦响应的事件
		/// </summary>
		[SerializeField]
		private Button.ButtonClickedEvent _onUnfocusedEvent = new();

		/// <summary>
		/// 失焦响应的事件
		/// </summary>
		public Button.ButtonClickedEvent onUnfocusedEvent => _onUnfocusedEvent;

		#endregion

		/// <summary>
		/// 当前焦点是否在面板内
		/// </summary>
		public bool FocusInBoard => _focusInBoard;

		/// <summary>
		/// 点击信息
		/// </summary>
		private readonly PointerEventData _curPointerEventData = new(UnityEngine.EventSystems.EventSystem.current);

		/// <summary>
		/// 点击时碰撞的信息
		/// </summary>
		private readonly List<RaycastResult> _raycastResults = new();

		#region Public Methods

		/// <summary>
		/// 聚焦
		/// </summary>
		public void Focus()
		{
			if (!_focusInBoard)
			{
				_onFocusedEvent.Invoke();
				_focusInBoard = true;
			}
		}

		/// <summary>
		/// 失焦
		/// </summary>
		public void Unfocus()
		{
			if (_focusInBoard)
			{
				_onUnfocusedEvent.Invoke();
				_focusInBoard = false;
			}
		}

		/// <summary>
		/// 添加子面板
		/// </summary>
		/// <param name="child">要添加的子面板</param>
		public void AddChildBoard(GameObject child)
		{
			if (child == null)
				return;

			if (!_boardOfChildren.Contains(child))
				_boardOfChildren.Add(child);
		}

		/// <summary>
		/// 添加子面板
		/// </summary>
		/// <param name="child">要添加的子面板</param>
		public void AddChildBoard(Component child)
		{
			if (child == null)
				return;

			AddChildBoard(child.gameObject);
		}

		/// <summary>
		/// 移除子面板
		/// </summary>
		/// <param name="child">要移除的子面板</param>
		public void RemoveChildBoard(GameObject child)
		{
			if (child == null)
				return;

			if (_boardOfChildren.Contains(child))
				_boardOfChildren.Remove(child);
		}

		/// <summary>
		/// 移除子面板
		/// </summary>
		/// <param name="child">要移除的子面板</param>
		public void RemoveChildBoard(Component child)
		{
			if (child == null)
				return;

			RemoveChildBoard(child.gameObject);
		}

		/// <summary>
		/// 清除全部子面板
		/// </summary>
		public void ClearChildBoard() { _boardOfChildren.Clear(); }

		#endregion

		protected override void OnEnable()
		{
			base.OnEnable();

			_focusInBoard = true;

			TickManager.Instance.Add(this);
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			TickManager.Instance.Remove(this);
		}

		bool IBaseTick.runInBackground => false;
		public int LateTickOrder => 0;

		public void OnLateTick(float deltaTime)
		{
			if (!_focusInBoard) return;

			var isPointerDown = Input.GetMouseButtonDown(0);

			if (!isPointerDown) return;

			_curPointerEventData.position = Input.mousePosition;

			UnityEngine.EventSystems.EventSystem.current.RaycastAll(_curPointerEventData, _raycastResults);

			foreach (var result in _raycastResults)
			{
				if (result.gameObject.Equals(gameObject))
				{
					// 如果点中的对象是面板自己,说明聚焦还在
					_focusInBoard = true;
					return;
				}

				if (result.gameObject.transform.IsChildOf(transform))
				{
					// 如果点中的对象是面板的子对象(不管这个子对象层级有多深),说明聚焦还在
					_focusInBoard = true;
					return;
				}

				if (_boardOfChildren is not {Count: > 0}) continue;
				if (!_boardOfChildren.Any(child => child.gameObject.Equals(result.gameObject))) continue;
				// 如果点中的对象是子面板的对象
				_focusInBoard = true;
				return;
			}

			_raycastResults.Clear();

			Unfocus();
		}
	}
}