using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

using KiwiFramework.Runtime.UnityExtend;

using Sirenix.OdinInspector;

using TMPro;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KiwiFramework.Runtime.UI
{
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("KiwiUI/Native/Dropdown")]
	public class Dropdown : Selectable, IPointerClickHandler, ISubmitHandler, ICancelHandler
	{
		/// <summary>
		/// 下拉列表展开收起状态变化时的事件
		/// </summary>
		[Serializable]
		public class DropdownStateEvent : UnityEvent<bool>
		{
		}

		/// <summary>
		/// 下拉列表选项发生变化时的事件
		/// </summary>
		[Serializable]
		public class DropdownEvent : UnityEvent<int>
		{
		}

		/// <summary>
		/// 下拉列表中选项的文本和图片的数据类
		/// </summary>
		[Serializable]
		public class OptionData
		{
			/// <summary>
			/// 要显示的文本
			/// </summary>
			[SerializeField]
			private string _text;

			/// <summary>
			/// 要显示的图片
			/// </summary>
			[SerializeField]
			private Sprite _image;

			/// <summary>
			/// 要显示的文本
			/// </summary>
			public string text
			{
				get => _text;
				set => _text = value;
			}

			/// <summary>
			/// 要显示的图片
			/// </summary>
			public Sprite image
			{
				get => _image;
				set => _image = value;
			}

			public OptionData() { }

			public OptionData(string text) { this.text = text; }

			public OptionData(Sprite image) { this.image = image; }

			public OptionData(string text, Sprite image)
			{
				this.text  = text;
				this.image = image;
			}
		}

		/// <summary>
		/// 下拉列表全部选项的数据列表类
		/// </summary>
		[Serializable, HideLabel]
		public class OptionDataList
		{
			[SerializeField] private List<OptionData> _options;

			/// <summary>
			/// 下拉列表的选项列表
			/// </summary>
			public List<OptionData> options
			{
				get => _options;
				set => _options = value;
			}

			public OptionDataList() { options = new List<OptionData>(); }
		}

		/// <summary>
		/// 箭头动画类型
		/// </summary>
		public enum DROPDOWN_ARROW_ANIMATION_TYPE
		{
			/// <summary>
			/// 无效果
			/// </summary>
			[InspectorName("无动画")]
			NONE,

			/// <summary>
			/// 修改 Sprite
			/// </summary>
			[InspectorName("切换图片")]
			CHANGE_SPRITE,

			/// <summary>
			/// 翻转类型
			/// </summary>
			[InspectorName("翻转图片")]
			FLIP,

			/// <summary>
			/// 自定义
			/// </summary>
			[InspectorName("自定义")]
			CUSTOM,
		}

		/// <summary>
		/// 用于创建下拉餐单的模板对象
		/// </summary>
		[SerializeField]
		private RectTransform _template;

		/// <summary>
		/// 用于创建下拉餐单的模板对象
		/// </summary>
		public RectTransform template
		{
			get => _template;
			set
			{
				_template = value;
				RefreshShownValue();
			}
		}

		/// <summary>
		/// 当前下拉菜单要显示的标题对象
		/// </summary>
		[SerializeField]
		private TMP_Text _captionText;

		/// <summary>
		/// 当前下拉菜单要显示的标题对象
		/// </summary>
		public TMP_Text captionText
		{
			get => _captionText;
			set
			{
				_captionText = value;
				RefreshShownValue();
			}
		}

		/// <summary>
		/// 当前选中选项要表现的图片
		/// </summary>
		[SerializeField]
		private UnityEngine.UI.Image _captionImage;

		/// <summary>
		/// 当选中选项要表现的图片
		/// </summary>
		public UnityEngine.UI.Image captionImage
		{
			get => _captionImage;
			set
			{
				_captionImage = value;
				RefreshShownValue();
			}
		}

		/// <summary>
		/// 当选项列表中没有选项数据时显示的图片
		/// </summary>
		[SerializeField]
		private GameObject _noDataObj;

		/// <summary>
		/// 当选项列表中没有选项数据时显示的图片
		/// </summary>
		public GameObject NoDataObj
		{
			get => _noDataObj;
			set
			{
				_noDataObj = value;
				RefreshShownValue();
			}
		}

		/// <summary>
		/// 下拉列表最大高度
		/// </summary>
		[SerializeField]
		private float _listMaxHeight = 300;

		/// <summary>
		/// 选项间隔
		/// </summary>
		[SerializeField]
		private float _itemSpace = 10;

		/// <summary>
		/// 垂直排版组件
		/// </summary>
		private VerticalLayoutGroup _verticalLayout;

		/// <summary>
		/// 模板中选项要显示文字的文本组件
		/// </summary>
		[SerializeField]
		private TMP_Text _itemText;

		/// <summary>
		/// 模板中选项要显示文字的文本组件
		/// </summary>
		public TMP_Text itemText
		{
			get => _itemText;
			set
			{
				_itemText = value;
				RefreshShownValue();
			}
		}

		/// <summary>
		/// 模板中选项要显示选中图片的图片组件
		/// </summary>
		[SerializeField]
		private RichImage _itemImage;

		/// <summary>
		/// 模板中选项要显示选中图片的图片组件
		/// </summary>
		public RichImage itemImage
		{
			get => _itemImage;
			set
			{
				_itemImage = value;
				RefreshShownValue();
			}
		}

		/// <summary>
		/// 选前选中的选项的 Index
		/// </summary>
		[SerializeField]
		private int _value;

		/// <summary>
		/// 下拉列表全部选项的数据列表类
		/// </summary>
		[SerializeField]
		private OptionDataList _options = new OptionDataList();

		/// <summary>
		/// 下拉列表全部选项的数据列表类
		/// </summary>
		public List<OptionData> options
		{
			get => _options.options;
			set
			{
				_options.options = value;
				RefreshShownValue();
			}
		}

		/// <summary>
		/// 响应下拉列表选项发生变化事件
		/// </summary>
		[Space]
		[SerializeField]
		private DropdownEvent _onValueChanged = new DropdownEvent();

		/// <summary>
		/// 响应下拉列表选项发生变化事件
		/// </summary>
		public DropdownEvent onValueChanged
		{
			get { return _onValueChanged; }
			set { _onValueChanged = value; }
		}

		/// <summary>
		/// 下拉框出现和消失的时间间隔 
		/// </summary>
		[SerializeField]
		private float _alphaFadeSpeed = 0.15f;

		/// <summary>
		/// 下拉框出现和消失的时间间隔 
		/// </summary>
		public float alphaFadeSpeed
		{
			get => _alphaFadeSpeed;
			set => _alphaFadeSpeed = value;
		}

		/// <summary>
		/// 箭头动画类型
		/// </summary>
		[SerializeField]
		private DROPDOWN_ARROW_ANIMATION_TYPE arrowAnimationType;

		/// <summary>
		/// 箭头对象
		/// </summary>
		[SerializeField]
		private RichImage arrow;

		/// <summary>
		/// 箭头显示状态图标
		/// </summary>
		[SerializeField]
		private Sprite showStateArrowIcon;

		/// <summary>
		/// 箭头隐藏状态图标
		/// </summary>
		[SerializeField]
		private Sprite hideStateArrowIcon;

		/// <summary>
		/// 箭头翻转类型(显示)
		/// </summary>
		[SerializeField]
		private FlipType arrowFlipType;

		/// <summary>
		/// 自定义箭头动画事件
		/// </summary>
		[SerializeField]
		public DropdownStateEvent onPlayArrowAnimation;

		/// <summary>
		/// 当前显示的下拉菜单对象
		/// </summary>
		private GameObject _dropdown;

		/// <summary>
		/// 当前被创建出的全部下拉对象
		/// </summary>
		private readonly List<DropdownItem> _items = new List<DropdownItem>();

		/// <summary>
		/// 下拉菜单动画 Tweener
		/// </summary>
		private Tweener _alphaTweener;

		/// <summary>
		/// 下拉菜单的模板是否无效
		/// </summary>
		private bool _validTemplate = false;

		/// <summary>
		/// 延迟删除下拉菜单时所用的协程
		/// </summary>
		private Coroutine _coroutine = null;

		/// <summary>
		/// 空选项静态列表
		/// </summary>
		private static OptionData s_NoOptionData = new OptionData();

		/// <summary>
		/// 当前选项的索引
		/// </summary>
		public int value
		{
			get => _value;
			set => SetValue(value);
		}

		/// <summary>
		/// 设置当前选中的选项而不触发 OnValueChanged 事件
		/// </summary>
		/// <param name="input">要设定的选项的索引</param>
		public void SetValueWithoutNotify(int input) { SetValue(input, false); }

		/// <summary>
		/// 设置选中的选项索引
		/// </summary>
		/// <param name="value">要设定的选项的索引</param>
		/// <param name="sendCallback">是否选中的选项而不触发 OnValueChanged 事件</param>
		private void SetValue(int value, bool sendCallback = true)
		{
			if (Application.isPlaying && (value == _value || options.Count == 0))
				return;

			_value = Mathf.Clamp(value, _noDataObj ? -1 : 0, options.Count - 1);
			RefreshShownValue();

			if (sendCallback)
			{
				// Notify all listeners
				UISystemProfilerApi.AddMarker("Dropdown.value", this);
				_onValueChanged.Invoke(_value);
			}
		}

		/// <summary>
		/// 是否是展开状态
		/// </summary>
		public bool IsExpanded => _dropdown != null;

		protected Dropdown() { }

		protected override void Awake()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				return;
#endif

			if (_captionImage)
				_captionImage.enabled = (_captionImage.sprite != null);

			if (_template)
			{
				_template.gameObject.SetActive(false);

				_verticalLayout = _template.GetComponentInChildren<VerticalLayoutGroup>();
				if (_verticalLayout != null) _verticalLayout.spacing = _itemSpace;
			}
		}

		protected override void Start()
		{
			base.Start();

			RefreshShownValue();

			PlayArrowAnimation(false);
		}

		/// <summary>
		/// 播放箭头动画
		/// </summary>
		/// <param name="isOn">是否是展开状态</param>
		private void PlayArrowAnimation(bool isOn)
		{
			switch (arrowAnimationType)
			{
				case DROPDOWN_ARROW_ANIMATION_TYPE.CHANGE_SPRITE:
					if (!arrow) return;

					arrow.sprite = isOn ? showStateArrowIcon : hideStateArrowIcon;
					break;
				case DROPDOWN_ARROW_ANIMATION_TYPE.FLIP:
					if (!arrow) return;

					if (isOn)
					{
						arrow.ImageType = ImageType.FLIP;
						arrow.FlipType  = arrowFlipType;
					}
					else
					{
						arrow.ImageType = ImageType.NONE;
					}

					break;
				case DROPDOWN_ARROW_ANIMATION_TYPE.CUSTOM:
					onPlayArrowAnimation?.Invoke(isOn);
					break;
				case DROPDOWN_ARROW_ANIMATION_TYPE.NONE:
				default:
					break;
			}
		}

		/// <summary>
		/// 使用指定模板创建下拉对象
		/// </summary>
		/// <param name="itemTemplate">下拉对象模板</param>
		/// <returns>被创建的下拉对象</returns>
		private DropdownItem CreateItem(DropdownItem itemTemplate) { return Instantiate(itemTemplate); }

		/// <summary>
		/// 创建点击时要显示的下拉列表
		/// </summary>
		/// <param name="template">下拉列表模板</param>
		/// <returns>被创建的下拉列表对象</returns>
		private GameObject CreateDropdownList(GameObject template)
		{
			var go = Instantiate(template);

			PlayArrowAnimation(true);

			return go;
		}

		/// <summary>
		/// 删除当前显示的下拉列表
		/// </summary>
		/// <param name="dropdownList">要被删除的下拉列表对象</param>
		private void DestroyDropdownList(GameObject dropdownList)
		{
			if (dropdownList.TryGetComponent<UIFocusBoard>(out var focusBoard))
				focusBoard.onUnfocusedEvent.RemoveAllListeners();

			Destroy(dropdownList);

			PlayArrowAnimation(false);
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();

			if (!IsActive())
				return;

			RefreshShownValue();
		}
#endif

		protected override void OnDisable()
		{
			ImmediateDestroyDropdownList();
			base.OnDisable();
		}

		/// <summary>
		/// 尝试刷新当前选定选项的文本和图像 
		/// </summary>
		/// <remarks>
		/// 如果修改了选项列表，则应该在之后调用此方法，以确保下拉菜单的可视状态与更新后的选项相对应。 
		/// </remarks>
		public void RefreshShownValue()
		{
			var data = s_NoOptionData;

			if (options.Count > 0 && _value >= 0)
				data = options[Mathf.Clamp(_value, 0, options.Count - 1)];

			if (_captionText)
			{
				if (data != null && data.text != null)
					_captionText.text = data.text;
				else
					_captionText.text = "";
			}

			if (_captionImage)
			{
				_captionImage.sprite  = data?.image;
				_captionImage.enabled = (_captionImage.sprite != null);
			}

			if (_noDataObj)
			{
				var isNoData = options.Count == 0 || _value == -1;
				if (isNoData)
				{
					var noDataRT = _noDataObj.rectTransform();
					noDataRT.SetAnchorPresets(AnchorHorizontal.Stretch, AnchorVertical.Stretch);
					noDataRT.sizeDelta = Vector2.zero;
					noDataRT.SetAnchoredPosition(0, 0);
				}

				_noDataObj.SetActive(isNoData);
			}
		}

		/// <summary>
		/// 添加选项(文本描述)
		/// </summary>
		/// <param name="labels">选项内容数组</param>
		public void AddOptions(string[] labels)
		{
			var count = labels.Length;

			for (var i = 0; i < count; i++)
				options.Add(new OptionData(labels[i]));

			RefreshShownValue();
		}

		/// <summary>
		/// 添加选项(图标显示)
		/// </summary>
		/// <param name="sprites"></param>
		public void AddOptions(Sprite[] sprites)
		{
			var count = sprites.Length;

			for (var i = 0; i < count; i++)
				options.Add(new OptionData(sprites[i]));

			RefreshShownValue();
		}

		/// <summary>
		/// 添加选项(文本描述及图标)
		/// </summary>
		/// <param name="labels">选项的文本内容数组</param>
		/// <param name="sprites">选项的图标数组</param>
		public void AddOptions(string[] labels, Sprite[] sprites)
		{
#if UNITY_EDITOR
			if (labels.Length != sprites.Length)
				Debug.LogError("要添加的文本和图片的数量不一致.");
#endif
			var count = Mathf.Max(labels.Length, sprites.Length);
			for (var i = 0; i < count; i++)
			{
				var data = new OptionData(
					labels[Mathf.Min(i, labels.Length)],
					sprites[Mathf.Min(i, sprites.Length)]
				);
				options.Add(data);
			}

			RefreshShownValue();
		}

		/// <summary>
		/// 添加多个选项
		/// </summary>
		/// <param name="options">要添加的选项数据的列表</param>
		public void AddOptions(List<OptionData> options)
		{
			this.options.AddRange(options);
			RefreshShownValue();
		}

		/// <summary>
		/// 添加多个选项
		/// </summary>
		/// <param name="options">要添加的选项文本的列表</param>
		public void AddOptions(List<string> options)
		{
			for (var i = 0; i < options.Count; i++)
				this.options.Add(new OptionData(options[i]));

			RefreshShownValue();
		}

		/// <summary>
		/// 添加多个选项
		/// </summary>
		/// <param name="options">要添加的选项图片的列表</param>
		public void AddOptions(List<Sprite> options)
		{
			for (var i = 0; i < options.Count; i++)
				this.options.Add(new OptionData(options[i]));

			RefreshShownValue();
		}

		/// <summary>
		/// 清除选项列表中的全部选项
		/// </summary>
		public void ClearOptions()
		{
			options.Clear();
			_value = _noDataObj ? -1 : 0;
			RefreshShownValue();
		}

		/// <summary>
		/// 根据模板创建下拉列表
		/// </summary>
		private void SetupTemplate()
		{
			_validTemplate = false;

			if (!_template)
			{
				Debug.LogError("下拉列表的模板对象为 null");
				return;
			}

			var templateGo = _template.gameObject;
			templateGo.SetActive(true);

			var itemToggle = _template.GetComponentInChildren<UIToggle>();

			_validTemplate = true;

			if (!itemToggle || itemToggle.transform == template)
			{
				_validTemplate = false;

				Debug.LogError($"{template} 模板无效: 模板必须有一个带有 Toggle 组件的子物体.");
			}
			else if (!(itemToggle.transform.parent is RectTransform))
			{
				_validTemplate = false;

				Debug.LogError($"{template} 模板无效: Toggle 组件对象的父物体必须有 RectTransform 组件.");
			}
			else if (itemText != null && !itemText.transform.IsChildOf(itemToggle.transform))
			{
				_validTemplate = false;

				Debug.LogError($"{template} 模板无效: 选项的文本对象必须是 Toggle 对象的子物体");
			}
			else if (itemImage != null && !itemImage.transform.IsChildOf(itemToggle.transform))
			{
				_validTemplate = false;

				Debug.LogError($"{template} 模板无效: 选项的图片对象必须是 Toggle 对象的子物体");
			}

			if (!_validTemplate)
			{
				templateGo.SetActive(false);
				return;
			}

			var item = itemToggle.gameObject.AddComponent<DropdownItem>();

			item.text        = _itemText;
			item.image       = _itemImage;
			item.toggleProxy = itemToggle;

			templateGo.ForceGetComponent<UIFocusBoard>();
			templateGo.ForceGetComponent<CanvasGroup>();

			templateGo.SetActive(false);

			_validTemplate = true;
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (IsExpanded)
				Hide();
			else
				Show();
		}

		public virtual void OnSubmit(BaseEventData eventData) { Show(); }

		public virtual void OnCancel(BaseEventData eventData) { Hide(); }

		/// <summary>
		/// 显示下拉菜单
		/// </summary>
		public void Show()
		{
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
				ImmediateDestroyDropdownList();
			}

			if (!IsActive() || !IsInteractable() || _dropdown != null)
				return;

			if (!_validTemplate)
			{
				SetupTemplate();
				if (!_validTemplate)
					return;
			}

			_template.gameObject.SetActive(true);

			// 根据下拉列表模板创建下拉列表对象
			_dropdown = CreateDropdownList(_template.gameObject);
#if UNITY_EDITOR
			_dropdown.name = "Dropdown List";
#endif
			_dropdown.SetActive(true);

			var dropdownRectTransform = _dropdown.rectTransform();
			dropdownRectTransform.SetParent(_template.transform.parent, false);

			// 实例化下拉菜单选项

			var itemTemplate = _dropdown.GetComponentInChildren<DropdownItem>();

			var content              = itemTemplate.rectTransform.parent.gameObject;
			var contentRectTransform = content.transform as RectTransform;

			itemTemplate.rectTransform.gameObject.SetActive(true);

			// 获取下拉列表和选项的矩形大小
			var dropdownContentRect = contentRectTransform.rect;
			var itemTemplateRect    = itemTemplate.rectTransform.rect;

			// 计算选项边缘和背景边缘之间的视觉偏移量 
			var itemTemplateLocalPos = itemTemplate.rectTransform.localPosition;
			var offsetMin            = itemTemplateRect.min - dropdownContentRect.min + (Vector2) itemTemplateLocalPos;
			var offsetMax            = itemTemplateRect.max - dropdownContentRect.max + (Vector2) itemTemplateLocalPos;
			var itemSize             = itemTemplateRect.size;

			_items.Clear();

			UIToggle prevProxy = null;

			for (var i = 0; i < options.Count; ++i)
			{
				var data = options[i];
				var item = AddItem(data, itemTemplate, _items);
				if (item == null)
					continue;

				item.toggleProxy.isOn = value == i;
				item.toggleProxy.onValueChanged.AddListener(x => OnSelectItem(item.toggleProxy));

				if (item.toggleProxy.isOn)
					item.toggleProxy.Native.Select();

				// 设置导航
				if (prevProxy != null)
				{
					var prevNav   = prevProxy.Native.navigation;
					var toggleNav = item.toggleProxy.Native.navigation;
					prevNav.mode   = Navigation.Mode.Explicit;
					toggleNav.mode = Navigation.Mode.Explicit;

					prevNav.selectOnDown   = item.toggleProxy.Native;
					prevNav.selectOnRight  = item.toggleProxy.Native;
					toggleNav.selectOnLeft = prevProxy.Native;
					toggleNav.selectOnUp   = prevProxy.Native;

					prevProxy.Native.navigation        = prevNav;
					item.toggleProxy.Native.navigation = toggleNav;
				}

				prevProxy = item.toggleProxy;
			}

			// 重新定位下拉菜单的位置
			dropdownRectTransform.SetAnchorPresets(AnchorHorizontal.Stretch, AnchorVertical.Bottom);

			var sizeDelta = dropdownRectTransform.sizeDelta;

			sizeDelta.Set(
				0,
				_noDataObj != null && _noDataObj.activeInHierarchy
					? _noDataObj.rectTransform().GetHeight()
					: Mathf.Min(itemSize.y * _items.Count + _itemSpace * (_items.Count - 1) + offsetMin.y - offsetMax.y, _listMaxHeight));

			dropdownRectTransform.sizeDelta = sizeDelta;

			dropdownRectTransform.SetPivot(PivotPresets.TopCenter);

			dropdownRectTransform.SetAnchoredPosition(0, 0);

			AlphaFadeList(_alphaFadeSpeed, 0f, 1f);

			var focusBoard = _dropdown.GetComponent<UIFocusBoard>();
			focusBoard.AddChildBoard(gameObject);
			focusBoard.onUnfocusedEvent.AddListener(Hide);

			_template.gameObject.SetActive(false);
			itemTemplate.gameObject.SetActive(false);
		}

		/// <summary>
		/// 删除已经创建的下拉选项
		/// </summary>
		/// <param name="item">要删除的下拉选项</param>
		protected virtual void DestroyItem(DropdownItem item) { }

		/// <summary>
		/// 添加一个下拉选项
		/// </summary>
		/// <param name="data">下拉选项的数据</param>
		/// <param name="itemTemplate">下拉选项模板</param>
		/// <param name="items">当前被创建出的全部下拉对象</param>
		/// <returns></returns>
		private DropdownItem AddItem(OptionData data, DropdownItem itemTemplate, List<DropdownItem> items)
		{
			var item = CreateItem(itemTemplate);

			item.rectTransform.SetParent(itemTemplate.rectTransform.parent, false);

			item.gameObject.SetActive(true);
#if UNITY_EDITOR
			item.gameObject.name = "Item " + items.Count + (data.text != null ? ": " + data.text : "");
#endif

			if (item.toggleProxy != null)
				item.toggleProxy.isOn = false;

			// 设置选项的数据内容
			if (item.text)
				item.text.text = data.text;

			if (item.image)
			{
				item.image.sprite  = data.image;
				item.image.enabled = (item.image.sprite != null);
			}

			items.Add(item);
			return item;
		}

		/// <summary>
		/// 执行下拉列表的 Alpha 变换
		/// </summary>
		/// <param name="duration">过程时间</param>
		/// <param name="alpha">目标 Alpha 值</param>
		private void AlphaFadeList(float duration, float alpha)
		{
			var group = _dropdown.GetComponent<CanvasGroup>();
			AlphaFadeList(duration, group.alpha, alpha);
		}

		/// <summary>
		/// 执行下拉列表的 Alpha 变换
		/// </summary>
		/// <param name="duration">过程时间</param>
		/// <param name="start">起始 Alpha 值</param>
		/// <param name="end">目标 Alpha 值</param>
		private void AlphaFadeList(float duration, float start, float end)
		{
			if (end.Equals(start)) return;

			if (!_dropdown) return;

			var group = _dropdown.GetComponent<CanvasGroup>();
			_alphaTweener = DOTween.To(() => group.alpha, alpha => group.alpha = alpha, end, duration);
		}

		/// <summary>
		/// 隐藏下拉列表
		/// </summary>
		public void Hide()
		{
			if (_coroutine != null) return;

			if (_dropdown != null)
			{
				AlphaFadeList(_alphaFadeSpeed, 0f);

				if (IsActive())
					_coroutine = StartCoroutine(DelayedDestroyDropdownList(_alphaFadeSpeed));
			}

			Select();
		}

		/// <summary>
		/// 延迟删除下拉列表(等待下拉列表动画结束)
		/// </summary>
		/// <param name="delay">延迟时间</param>
		/// <returns></returns>
		private IEnumerator DelayedDestroyDropdownList(float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			ImmediateDestroyDropdownList();
		}

		/// <summary>
		/// 立即删除下拉列表
		/// </summary>
		private void ImmediateDestroyDropdownList()
		{
			for (var i = 0; i < _items.Count; i++)
			{
				if (_items[i] != null)
					DestroyItem(_items[i]);
			}

			_items.Clear();

			if (_dropdown != null)
				DestroyDropdownList(_dropdown);

			_alphaTweener?.Kill();

			_dropdown  = null;
			_coroutine = null;
		}

		/// <summary>
		/// 修改选中选项并隐藏下拉菜单
		/// </summary>
		/// <param name="toggle">选中选项的 Toggle 对象</param>
		private void OnSelectItem(UIToggle toggle)
		{
			if (!toggle.isOn)
				toggle.isOn = true;

			var selectedIndex = -1;
			var tr            = toggle.transform;
			var parent        = tr.parent;
			for (var i = 0; i < parent.childCount; i++)
			{
				if (parent.GetChild(i) == tr)
				{
					selectedIndex = i - 1;
					break;
				}
			}

			if (selectedIndex < 0)
				return;

			value = selectedIndex;
			Hide();
		}
	}
}