using System;

using KiwiFramework.Runtime.UnityExtend;
using KiwiFramework.Runtime.Utility;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace KiwiFramework.Runtime.UI
{
	[HideMonoScript]
	[AddComponentMenu("KiwiUI/UIDrag")]
	[RequireComponent(typeof(TransparentGraphic))]
	public class UIDrag : UIElement, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
	{
		[Serializable]
		public class DragClickedEvent : UnityEvent<Vector2>
		{
		}

		/// <summary>
		/// 拖拽是否可用
		/// </summary>
		[SerializeField, LabelText("是否可用")]
		private bool _interactable = true;

		/// <summary>
		/// 是否可以水平拖拽
		/// </summary>
		[SerializeField, LabelText("水平拖动")]
		private bool _canHorizontal = true;

		/// <summary>
		/// 是否可以垂直拖拽
		/// </summary>
		[SerializeField, LabelText("垂直拖动")]
		private bool _canVertical = true;

		/// <summary>
		/// 是否按住拖拽物
		/// </summary>
		private bool _hisOnDragObj = false;

		/// <summary>
		/// 是否控制拖拽范围
		/// </summary>
		[SerializeField, LabelText("是否可以超出拖拽区域"), InlineButton("CanOutOfAreaTips", SdfIconType.QuestionCircleFill, "")]
		private bool _canOutOfArea = true;

#if UNITY_EDITOR
		private void CanOutOfAreaTips() { UnityEditor.EditorUtility.DisplayDialog("拖拽区域说明", "本对象大小即为可拖拽区域,在Scene界面显示为蓝色区域.", "OK"); }
#endif

		/// <summary>
		/// 拖拽对象
		/// </summary>
		[SerializeField, LabelText("拖拽物对象"),
		 InfoBox("必须制定拖拽物,并且保证拖拽物为本对象子物体!", InfoMessageType.Error, "@dragObj == null")]
		private RectTransform dragObj;

		/// <summary>
		/// 点击按下位置
		/// </summary>
		private Vector2 _pointerDownPos;

		/// <summary>
		/// 点击按下时拖拽对象位置
		/// </summary>
		private Vector2 _objDownPos;

		/// <summary>
		/// 是否正在拖拽中
		/// </summary>
		private bool _isDragging = false;

		/// <summary>
		/// 拖拽物极限范围(x-left,y,-top,z-right,w-bottom)
		/// </summary>
		private Vector4 _maxminArea;

		/// <summary>
		/// 是否需要按在拖拽物上才可以拖动
		/// </summary>
		[SerializeField, LabelText("是否需要按住拖拽物"),
		 InfoBox("勾选了此选项,请保证拖拽物可以被射线检测!", SdfIconType.MegaphoneFill, "needHitObj")]
		public bool needHitObj = false;

		/// <summary>
		/// 拖拽结束事件
		/// </summary>
		[SerializeField, BoxGroup("Event")]
		public DragClickedEvent onDragEndEvent = new();

		/// <summary>
		/// 点击拖拽物事件
		/// </summary>
		[SerializeField, BoxGroup("Event")]
		public DragClickedEvent onDragClickEvent = new();

		public bool interactable
		{
			get => _interactable;
			set
			{
				if (!SetPropertyUtility.SetStruct(ref _interactable, value))
					return;

				if (!_interactable)
				{
					_isDragging     = false;
					_pointerDownPos = Vector2.zero;
					_objDownPos     = Vector2.zero;
				}
			}
		}

		/// <summary>
		/// 是否可以水平拖拽
		/// </summary>
		public bool Horizontal
		{
			get => _canHorizontal;
			set => SetPropertyUtility.SetStruct(ref _canHorizontal, value);
		}

		/// <summary>
		/// 是否可以垂直拖拽
		/// </summary>
		public bool Vertical
		{
			get => _canVertical;
			set => SetPropertyUtility.SetStruct(ref _canVertical, value);
		}

		/// <summary>
		/// 是否控制拖拽范围
		/// </summary>
		public bool CanOutOfArea
		{
			get => _canOutOfArea;
			set
			{
				if (!SetPropertyUtility.SetStruct(ref _canOutOfArea, value))
					return;

				if (!_canOutOfArea)
					CalMaxminArea();
			}
		}

		/// <summary>
		/// 拖拽物极限范围(x-left,y,-top,z-right,w-bottom)
		/// </summary>
		public Vector4 MaxminArea => _maxminArea;

		/// <summary>
		/// 限制拖拽对象在可拖拽范围内
		/// </summary>
		/// <param name="pos">拖拽对象当前目标坐标</param>
		private void ClampToArea(ref Vector2 pos)
		{
			pos.x = Mathf.Clamp(pos.x, _maxminArea.x, _maxminArea.z);
			pos.y = Mathf.Clamp(pos.y, _maxminArea.y, _maxminArea.w);
		}

		/// <summary>
		/// 计算拖拽物极限范围
		/// </summary>
		public void CalMaxminArea(float x = 0, float y = 0)
		{
			if (dragObj == null) return;
			Vector2 baseArea    = rectTransform.GetSize();
			Vector2 dragObjSize = dragObj.GetSize();
			if (x != 0 || y != 0)
				dragObjSize = new Vector2(x, y);

			if (dragObj.IsAnchorHorizontal(AnchorHorizontal.Left))
			{
				_maxminArea.x = (baseArea.x > dragObjSize.x) ? 0 : (baseArea.x - dragObjSize.x);
				_maxminArea.z = (baseArea.x > dragObjSize.x) ? (baseArea.x - dragObjSize.x) : 0;
			}
			else if (dragObj.IsAnchorHorizontal(AnchorHorizontal.Center))
			{
				_maxminArea.x = (baseArea.x > dragObjSize.x)
					? ((dragObjSize.x - baseArea.x) * 0.5f)
					: ((baseArea.x - dragObjSize.x) * 0.5f);
				_maxminArea.z = (baseArea.x > dragObjSize.x)
					? ((baseArea.x - dragObjSize.x) * 0.5f)
					: ((dragObjSize.x - baseArea.x) * 0.5f);
			}
			else if (dragObj.IsAnchorHorizontal(AnchorHorizontal.Right))
			{
				_maxminArea.x = (baseArea.x > dragObjSize.x) ? (dragObjSize.x - baseArea.x) : 0;
				_maxminArea.z = (baseArea.x > dragObjSize.x) ? 0 : (dragObjSize.x - baseArea.x);
			}

			if (dragObj.IsAnchorVertical(AnchorVertical.Top))
			{
				_maxminArea.y = (baseArea.y > dragObjSize.y) ? (dragObjSize.y - baseArea.y) : 0;
				_maxminArea.w = (baseArea.y > dragObjSize.y) ? 0 : (dragObjSize.y - baseArea.y);
			}
			else if (dragObj.IsAnchorVertical(AnchorVertical.Middle))
			{
				_maxminArea.y = (baseArea.y > dragObjSize.y)
					? ((dragObjSize.y - baseArea.y) * 0.5f)
					: ((baseArea.y - dragObjSize.y) * 0.5f);
				_maxminArea.w = (baseArea.y > dragObjSize.y)
					? ((baseArea.y - dragObjSize.y) * 0.5f)
					: ((dragObjSize.y - baseArea.y) * 0.5f);
			}
			else if (dragObj.IsAnchorVertical(AnchorVertical.Bottom))
			{
				_maxminArea.y = (baseArea.y > dragObjSize.y) ? 0 : (baseArea.y - dragObjSize.y);
				_maxminArea.w = (baseArea.y > dragObjSize.y) ? (baseArea.y - dragObjSize.y) : 0;
			}
		}

		protected override void OnAwake()
		{
			base.OnAwake();
#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				if (dragObj == null)
				{
					Debug.LogException(new Exception("拖拽对象为空"), gameObject);
				}
			}
#endif
			if (!_canOutOfArea)
				CalMaxminArea();
		}

		protected override void OnDestroyed()
		{
			onDragEndEvent.RemoveAllListeners();
			onDragClickEvent.RemoveAllListeners();
			base.OnDestroyed();
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!interactable || dragObj == null) return;

			_isDragging = true;

			_objDownPos = dragObj.anchoredPosition;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				this.rectTransform(),
				eventData.position,
				eventData.pressEventCamera,
				out _pointerDownPos
			);

			if (needHitObj)
			{
				var pointerTarget = eventData.pointerCurrentRaycast.gameObject;
				_hisOnDragObj = pointerTarget == dragObj.gameObject || pointerTarget.GetComponentsInParent<UIDrag>() != null;
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!interactable || dragObj == null) return;
			if (needHitObj && !_hisOnDragObj) return;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				this.rectTransform(),
				eventData.position,
				eventData.pressEventCamera,
				out var localPointerPos
			);

			var targetPos = _objDownPos + localPointerPos - _pointerDownPos;

			if (!_canHorizontal)
				targetPos.x = _objDownPos.x;
			if (!_canVertical)
				targetPos.y = _objDownPos.y;

			if (!_canOutOfArea)
				ClampToArea(ref targetPos);

			dragObj.anchoredPosition = targetPos;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (!interactable || dragObj == null) return;

			_isDragging = false;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				this.rectTransform(),
				eventData.position,
				eventData.pressEventCamera,
				out var pos
			);

			onDragEndEvent?.Invoke(pos);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (!interactable || dragObj == null || _isDragging) return;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				this.rectTransform(),
				eventData.position,
				eventData.pressEventCamera,
				out var pos
			);

			onDragClickEvent?.Invoke(pos);
		}
	}
}