using KiwiFramework.Runtime.UnityExtend;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;

namespace KiwiFramework.Runtime.UI
{
	/// <summary>
	/// 下拉对象
	/// </summary>
	[RequireComponent(typeof(UIToggle))]
	[AddComponentMenu("KiwiUI/Native/DropdownItem")]
	public class DropdownItem : UIBehaviour, IPointerEnterHandler, ICancelHandler
	{
		/// <summary>
		/// 下拉对象显示的文字组件
		/// </summary>
		[SerializeField]
		private TMP_Text _text;

		/// <summary>
		/// 下拉对象被选中时的图片组件
		/// </summary>
		[SerializeField]
		private RichImage _image;

		/// <summary>
		/// 下拉对象的 RectTransform 组件
		/// </summary>
		[SerializeField]
		private RectTransform _rectTransform;

		[SerializeField] private UIToggle toggleProxyProxy;

		/// <summary>
		/// 下拉对象显示的文字组件
		/// </summary>
		public TMP_Text text
		{
			get => _text;
			set => _text = value;
		}

		/// <summary>
		/// 下拉对象被选中时的图片组件
		/// </summary>
		public RichImage image
		{
			get => _image;
			set => _image = value;
		}

		/// <summary>
		/// 下拉对象的 RectTransform 组件
		/// </summary>
		public RectTransform rectTransform => _rectTransform;

		public UIToggle toggleProxy
		{
			get => toggleProxyProxy;
			set => toggleProxyProxy = value;
		}

		protected override void Awake()
		{
			base.Awake();

			_rectTransform = this.rectTransform();

			toggleProxyProxy = GetComponent<UIToggle>();
		}

		public void OnPointerEnter(PointerEventData eventData) { UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(gameObject); }

		public void OnCancel(BaseEventData eventData)
		{
			var dropdown = GetComponentInParent<Dropdown>();
			if (dropdown)
				dropdown.Hide();
		}
	}
}