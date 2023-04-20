using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using KiwiFramework.Runtime.UnityExtend;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 可穿透点击遮罩
	/// </summary>
	[HideMonoScript]
	[AddComponentMenu("KiwiUI/UIBackground")]
	public class UIBackground : UIElement, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
	{
		/// <summary>
		/// 遮罩类型
		/// </summary>
		private enum MASK_TYPE
		{
			/// <summary>
			/// 有色半透
			/// </summary>
			[InspectorName("有色半透")]
			COLOR,

			/// <summary>
			/// 背景虚化
			/// </summary>
			[InspectorName("模糊")]
			BLUR,

			/// <summary>
			/// 全透
			/// </summary>
			[InspectorName("全透明")]
			TRANSPARENT,
		}
		
		private static Sprite _defaultSprite = null;

		private static Sprite DefaultSprite
		{
			get
			{
				if (_defaultSprite != null) return _defaultSprite;

				var tex2D = Texture2D.whiteTexture;
				tex2D.name          = "kiwi.UIBackground";
				_defaultSprite      = Sprite.Create(tex2D, new Rect(0, 0, 4, 4), Vector2.zero);
				_defaultSprite.name = "Kiwi.Sprite.UIBackground";

				return _defaultSprite;
			}
		}


		/// <summary>
		/// 有色遮罩时的颜色
		/// </summary>
		private static readonly Color MaskColor = new(0, 0, 0, 0.5f);

		/// <summary>
		/// 遮罩类型
		/// </summary>
		private MASK_TYPE _maskType = MASK_TYPE.COLOR;

		/// <summary>
		/// 是否可以点击
		/// </summary>
		[SerializeField, HideInInspector]
		private bool _enableClick = false;

		/// <summary>
		/// 是否可以点击
		/// </summary>
		[ShowInInspector, ToggleGroup("enableClick", ToggleGroupTitle = "是否可以点击")]
		private bool enableClick
		{
			get => _enableClick;
			set
			{
				var graphic = GetComponent<Graphic>();
				graphic.raycastTarget = value;
				_enableClick          = value;
			}
		}

		/// <summary>
		/// 是否可以穿透
		/// </summary>
		[SerializeField, LabelText("是否可以穿透"), BoxGroup("enableClick/Box", ShowLabel = false)]
		private bool enablePass = false;

		/// <summary>
		/// 点击事件
		/// </summary>
		[SerializeField, BoxGroup("enableClick/Box")]
		private Button.ButtonClickedEvent _onButtonClick = new();

		/// <summary>
		/// 按下事件
		/// </summary>
		[SerializeField, BoxGroup("enableClick/Box")]
		private Button.ButtonClickedEvent _onButtonDown = new();

		/// <summary>
		/// 抬起事件
		/// </summary>
		[SerializeField, BoxGroup("enableClick/Box")]
		private Button.ButtonClickedEvent _onButtonUp = new();

		/// <summary>
		/// 遮罩类型
		/// </summary>
		[ShowInInspector, LabelText("遮罩类型"), EnumPaging, PropertyOrder(-1)]
		private MASK_TYPE maskType
		{
			get => _maskType;
			set
			{
				if (value == MASK_TYPE.TRANSPARENT)
				{
					var graphic = GetComponent<RichImage>();
					if (graphic != null)
						DestroyImmediate(graphic);

					gameObject.ForceGetComponent<TransparentGraphic>();
				}
				else
				{
					var transGraphic = GetComponent<TransparentGraphic>();
					if (transGraphic != null)
						DestroyImmediate(transGraphic);

					var graphic = gameObject.ForceGetComponent<RichImage>();

					UIEffectHelper.SetBlur(graphic, value == MASK_TYPE.BLUR);

					if (value == MASK_TYPE.COLOR)
					{
						graphic.sprite = DefaultSprite;
						graphic.color  = MaskColor;
					}
					else if (value == MASK_TYPE.BLUR)
					{
						graphic.color = Color.white;
					}
				}

				_maskType = value;
			}
		}

		private static void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> callback)
			where T : IEventSystemHandler
		{
			var results = new List<RaycastResult>();

			UnityEngine.EventSystems.EventSystem.current.RaycastAll(data, results);

			var current = data.pointerCurrentRaycast.gameObject;
			if (current == null)
				return;

			foreach (var result in results.Where(result => current != result.gameObject))
			{
				ExecuteEvents.Execute(result.gameObject, data, callback);
				break;
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (!enableClick)
				return;

			_onButtonClick?.Invoke();

			if (enablePass)
				PassEvent(eventData, ExecuteEvents.pointerClickHandler);
		}

		public void AddPointerClickEvent([NotNull] UnityAction pointerEvent)
		{
			if (pointerEvent == null) throw new ArgumentNullException(nameof(pointerEvent));
			_onButtonClick.AddListener(pointerEvent);
		}

		public void RemovePointerClickEvent([NotNull] UnityAction pointerEvent)
		{
			if (pointerEvent == null) throw new ArgumentNullException(nameof(pointerEvent));
			_onButtonClick.RemoveListener(pointerEvent);
		}

		public void ClearPointerClickEvent() { _onButtonClick.RemoveAllListeners(); }

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!enableClick)
				return;

			_onButtonDown?.Invoke();

			if (enablePass)
				PassEvent(eventData, ExecuteEvents.pointerDownHandler);
		}

		public void AddPointerDownEvent([NotNull] UnityAction pointerEvent)
		{
			if (pointerEvent == null) throw new ArgumentNullException(nameof(pointerEvent));
			_onButtonDown.AddListener(pointerEvent);
		}

		public void RemovePointerDownEvent([NotNull] UnityAction pointerEvent)
		{
			if (pointerEvent == null) throw new ArgumentNullException(nameof(pointerEvent));
			_onButtonDown.RemoveListener(pointerEvent);
		}

		public void ClearPointerDownEvent() { _onButtonDown.RemoveAllListeners(); }

		public void OnPointerUp(PointerEventData eventData)
		{
			if (!enableClick)
				return;

			_onButtonUp?.Invoke();

			if (enablePass)
				PassEvent(eventData, ExecuteEvents.pointerUpHandler);
		}

		public void AddPointerUpEvent([NotNull] UnityAction pointerEvent)
		{
			if (pointerEvent == null) throw new ArgumentNullException(nameof(pointerEvent));
			_onButtonUp.AddListener(pointerEvent);
		}

		public void RemovePointerUpEvent([NotNull] UnityAction pointerEvent)
		{
			if (pointerEvent == null) throw new ArgumentNullException(nameof(pointerEvent));
			_onButtonUp.RemoveListener(pointerEvent);
		}

		public void ClearPointerUpEvent() { _onButtonUp.RemoveAllListeners(); }

#if UNITY_EDITOR

		protected override void Reset()
		{
			base.Reset();

			maskType = MASK_TYPE.COLOR;
		}
#endif
	}
}